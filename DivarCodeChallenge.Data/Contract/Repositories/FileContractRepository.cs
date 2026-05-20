using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DivarCodeChallenge.Domain.Contracts;

namespace DivarCodeChallenge.Infrastructure.Persistence;

public sealed class FileContractRepository : IContractRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public FileContractRepository()
    {
        _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Persistence",
            "Files",
            "contracts.json");

        EnsureFileExists();

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    ti =>
                    {
                        if (ti.Type == typeof(Contract))
                        {
                            ti.PolymorphismOptions = new JsonPolymorphismOptions
                            {
                                TypeDiscriminatorPropertyName = "$contractType",
                                IgnoreUnrecognizedTypeDiscriminators = false,
                                DerivedTypes =
                                {
                                    new JsonDerivedType(typeof(RentContract), nameof(RentContract)),
                                    new JsonDerivedType(typeof(SpecialPurchaseContract), nameof(SpecialPurchaseContract)),
                                    new JsonDerivedType(typeof(ImmediateSaleContract), nameof(ImmediateSaleContract)),
                                    new JsonDerivedType(typeof(BuyContract), nameof(BuyContract))
                                }
                            };
                        }
                    }
                }
            }
        };
    }

    public async Task AddAsync(Contract contract)
    {
        var all = (await GetAllAsync()).ToList();
        all.Add(contract);
        var json = JsonSerializer.Serialize(all, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        var json = await File.ReadAllTextAsync(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<Contract>();

        var data = JsonSerializer.Deserialize<List<Contract>>(json, _jsonOptions);
        return data ?? new List<Contract>();
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        var all = await GetAllAsync();
        return all.FirstOrDefault(c => c.Id == id);
    }

    public async Task<IEnumerable<Contract>> GetByUserIdAsync(Guid userId)
    {
        var all = await GetAllAsync();
        return all.Where(c =>
            c.OwnerId == userId ||
            c.CustomerId == userId
        );
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    public async Task UpdateAsync(Contract contract)
    {
        var all = (await GetAllAsync()).ToList();

        var index = all.FindIndex(c => c.Id == contract.Id);
        if (index == -1)
            throw new InvalidOperationException("Contract not found for update.");

        all[index] = contract;

        var json = JsonSerializer.Serialize(all, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }
}
