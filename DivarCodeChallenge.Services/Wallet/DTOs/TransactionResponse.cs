public class TransactionResponse
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
