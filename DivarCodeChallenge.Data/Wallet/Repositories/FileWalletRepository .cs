using System.Globalization;
using DivarCodeChallenge.Application.Wallet.Interface;
using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Wallets;
using DivarCodeChallenge.Domain.Wallets.ValueObjects;

namespace DivarCodeChallenge.Infrastructure.Persistence;

public sealed class FileWalletRepository : IWalletRepository
{
    private readonly string _walletFile;
    private readonly string _transactionFile;

    public FileWalletRepository()
    {
        var baseDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Persistence",
            "Files");

        if (!Directory.Exists(baseDir))
            Directory.CreateDirectory(baseDir);

        _walletFile = Path.Combine(baseDir, "wallets.txt");
        _transactionFile = Path.Combine(baseDir, "transactions.txt");

        EnsureFileExists(_walletFile);
        EnsureFileExists(_transactionFile);
    }

    public async Task AddAsync(Wallet wallet)
    {
        var lines = new List<string>();

        if (File.Exists(_walletFile))
            lines = (await File.ReadAllLinesAsync(_walletFile)).ToList();

        lines.RemoveAll(l =>
        {
            var p = l.Split('|');
            return p.Length >= 1 &&
                   Guid.TryParse(p[0], out var id) &&
                   id == wallet.Id;
        });

        lines.Add($"{wallet.Id}|{wallet.UserId}|{wallet.Balance.Amount.ToString(CultureInfo.InvariantCulture)}");

        await File.WriteAllLinesAsync(_walletFile, lines);

        await SaveTransactions(wallet);
    }

    public async Task<Wallet?> GetByUserIdAsync(Guid userId)
    {
        if (!File.Exists(_walletFile))
            return null;

        var lines = await File.ReadAllLinesAsync(_walletFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split('|');
            if (parts.Length < 3)
                continue;

            if (!Guid.TryParse(parts[0], out var walletId))
                continue;

            if (!Guid.TryParse(parts[1], out var walletUserId))
                continue;

            if (walletUserId != userId)
                continue;

            if (!decimal.TryParse(
                    parts[2],
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var balance))
                continue;

            var wallet = Wallet.Load(
                walletUserId,
                walletId,
                new Money(balance));

            await LoadTransactions(wallet);

            return wallet;
        }

        return null;
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        var updatedLines = new List<string>();

        if (File.Exists(_walletFile))
        {
            var lines = await File.ReadAllLinesAsync(_walletFile);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('|');
                if (parts.Length < 3)
                    continue;

                if (!Guid.TryParse(parts[0], out var existingId))
                    continue;

                if (existingId != wallet.Id)
                    updatedLines.Add(line);
            }
        }

        updatedLines.Add($"{wallet.Id}|{wallet.UserId}|{wallet.Balance.Amount.ToString(CultureInfo.InvariantCulture)}");

        await File.WriteAllLinesAsync(_walletFile, updatedLines);

        await SaveTransactions(wallet);
    }

    private async Task LoadTransactions(Wallet wallet)
    {
        if (!File.Exists(_transactionFile))
            return;

        var lines = await File.ReadAllLinesAsync(_transactionFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split('|');
            if (parts.Length < 5)
                continue;

            if (!Guid.TryParse(parts[0], out var transactionId))
                continue;

            if (!Guid.TryParse(parts[1], out var walletId))
                continue;

            if (walletId != wallet.Id)
                continue;

            if (!decimal.TryParse(parts[2],
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var amount))
                continue;

            var type = parts[3];

            if (!DateTime.TryParse(parts[4],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var createdAt))
                continue;

            wallet.LoadTransaction(
                transactionId,
                new Money(amount),
                type,
                createdAt);
        }
    }

    private async Task SaveTransactions(Wallet wallet)
    {
        var linesToKeep = new List<string>();

        if (File.Exists(_transactionFile))
        {
            var existingLines = await File.ReadAllLinesAsync(_transactionFile);

            foreach (var line in existingLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('|');
                if (parts.Length < 5)
                    continue;

                if (!Guid.TryParse(parts[1], out var walletId))
                    continue;

                if (walletId != wallet.Id)
                    linesToKeep.Add(line);
            }
        }

        foreach (var tx in wallet.Transactions)
        {
            linesToKeep.Add(
                $"{tx.Id}|{wallet.Id}|{tx.Amount.Amount.ToString(CultureInfo.InvariantCulture)}|{tx.Type}|{tx.CreatedDate:o}");
        }

        await File.WriteAllLinesAsync(_transactionFile, linesToKeep);
    }

    private void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
            File.WriteAllText(path, string.Empty);
    }
}
