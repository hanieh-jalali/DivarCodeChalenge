namespace DivarCodeChallenge.Application.Wallet.DTOs;

public class WalletAmountRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}
