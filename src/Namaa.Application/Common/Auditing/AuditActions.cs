namespace Namaa.Application.Common.Auditing;

public static class AuditActions
{
    public const string Created = "Created";
    public const string Updated = "Updated";
    public const string Added = "Added";
    public const string StatusChanged = "StatusChanged";
    public const string Sent = "Sent";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
    public const string Issued = "Issued";
    public const string Posted = "Posted";
    public const string MetadataCreated = "MetadataCreated";
    public const string MetadataUpdated = "MetadataUpdated";
    public const string ReadStateChanged = "ReadStateChanged";

    public static string For(string entityType, string operation) =>
        $"{entityType}.{operation}";
}
