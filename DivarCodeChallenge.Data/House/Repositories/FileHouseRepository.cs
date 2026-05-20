using System.Text.Json;
using DivarCodeChallenge.Application.Houses.Interfaces;
using DivarCodeChallenge.Domain.Houses;

namespace DivarCodeChallenge.Infrastructure.Persistence;

public sealed class FileHouseRepository : IHouseRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _options =
        new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

    public FileHouseRepository()
    {
        _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Persistence",
            "Files",
            "houses.json");

        EnsureFileExists();
    }

    public async Task<List<House>> GetAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<House>();

        try
        {
            return JsonSerializer.Deserialize<List<House>>(json, _options)
                   ?? new List<House>();
        }
        catch
        {
            return new List<House>();
        }
    }

    public async Task<House?> GetByIdAsync(Guid id)
    {
        var houses = await GetAllAsync();
        return houses.FirstOrDefault(h => h.Id == id);
    }

    public async Task AddAsync(House house)
    {
        var houses = await GetAllAsync();
        houses.Add(house);
        await SaveAsync(houses);
    }

    public async Task UpdateAsync(House house)
    {
        var houses = await GetAllAsync();

        var index = houses.FindIndex(h => h.Id == house.Id);
        if (index == -1)
            throw new Exception("House not found.");

        houses[index] = house;
        await SaveAsync(houses);
    }

    public async Task DeleteAsync(Guid id)
    {
        var houses = await GetAllAsync();

        var house = houses.FirstOrDefault(h => h.Id == id);
        if (house == null)
            throw new Exception("House not found.");

        houses.Remove(house);
        await SaveAsync(houses);
    }

    private async Task SaveAsync(List<House> houses)
    {
        var json = JsonSerializer.Serialize(houses, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }
}
