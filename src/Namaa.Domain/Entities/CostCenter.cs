namespace Namaa.Domain.Entities;

public class CostCenter
{
    private CostCenter()
    {
    }

    private CostCenter(
        string costCenterCode,
        string name,
        string? description,
        long? parentCostCenterId,
        long? branchId)
    {
        CostCenterCode = DomainGuard.Required(costCenterCode, nameof(costCenterCode));
        Name = DomainGuard.Required(name, nameof(name));
        DomainGuard.OptionalPositive(parentCostCenterId, nameof(parentCostCenterId));
        DomainGuard.OptionalPositive(branchId, nameof(branchId));
        ParentCostCenterId = parentCostCenterId;
        BranchId = branchId;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string CostCenterCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public long? ParentCostCenterId { get; private set; }
    public long? BranchId { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public CostCenter? ParentCostCenter { get; private set; }
    public ICollection<CostCenter> ChildCostCenters { get; private set; } = new List<CostCenter>();
    public Branch? Branch { get; private set; }
    public ICollection<JournalEntryLine> JournalEntryLines { get; private set; } = new List<JournalEntryLine>();

    public static CostCenter Create(
        string costCenterCode,
        string name,
        string? description = null,
        long? parentCostCenterId = null,
        long? branchId = null) =>
        new(costCenterCode, name, description, parentCostCenterId, branchId);

    public void Update(string name, string? description = null, long? branchId = null)
    {
        var validatedName = DomainGuard.Required(name, nameof(name));
        DomainGuard.OptionalPositive(branchId, nameof(branchId));
        Name = validatedName;
        Description = description;
        BranchId = branchId;
        UpdatedAt = DateTime.Now;
    }

    public void MoveUnder(long? parentCostCenterId)
    {
        DomainGuard.OptionalPositive(parentCostCenterId, nameof(parentCostCenterId));

        if (parentCostCenterId == Id && Id > 0)
            throw new InvalidOperationException("A cost center cannot be its own parent.");

        ParentCostCenterId = parentCostCenterId;
        UpdatedAt = DateTime.Now;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.Now;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.Now;
    }
}
