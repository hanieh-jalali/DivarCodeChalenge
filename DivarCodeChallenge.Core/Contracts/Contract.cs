using DivarCodeChallenge.Domain.Houses;
using DivarCodeChallenge.Domain.Shared;
using DivarCodeChallenge.Domain.Wallets;

namespace DivarCodeChallenge.Domain.Contracts;

public abstract class Contract : AggregateRoot
{
    public Guid HouseId { get; protected set; }

    public Guid OwnerId { get; protected set; }

    public Guid CustomerId { get; protected set; }

    public DateTime ContractDate { get; protected set; }

    public DateTime StartDate { get; protected set; }

    public DateTime EndDate { get; protected set; }

    public Money Amount { get; protected set; }

    public string Status { get; protected set; }

    public string Description { get; protected set; }

    protected Contract()
    {
    }

    protected Contract(
        Guid houseId,
        Guid ownerId,
        Guid customerId,
        DateTime startDate,
        DateTime endDate,
        Money amount,
        string description)
    {
        if (startDate >= endDate)
            throw new InvalidOperationException(
                "Start date cannot be greater than end date.");

        Id = Guid.NewGuid();

        HouseId = houseId;

        OwnerId = ownerId;

        CustomerId = customerId;

        ContractDate = DateTime.UtcNow;

        StartDate = startDate;

        EndDate = endDate;

        Amount = amount;

        Description = description;

        Status = ContractStatuses.Active;

        CreatedDate = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ContractStatuses.Cancelled)
            throw new InvalidOperationException(
                "Contract already cancelled.");

        Status = ContractStatuses.Cancelled;

        ModifiedDate = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == ContractStatuses.Completed)
            throw new InvalidOperationException(
                "Contract already completed.");

        Status = ContractStatuses.Completed;

        ModifiedDate = DateTime.UtcNow;
    }
}
