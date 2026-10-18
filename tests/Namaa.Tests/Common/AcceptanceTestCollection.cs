namespace Namaa.Tests.Common;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class AcceptanceTestCollection : ICollectionFixture<NamaaApiFixture>
{
    public const string Name = "SQL Server API acceptance";
}
