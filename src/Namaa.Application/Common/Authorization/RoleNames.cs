namespace Namaa.Application.Common.Authorization;

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string ProjectManager = "ProjectManager";
    public const string Sales = "Sales";
    public const string Accountant = "Accountant";
    public const string HR = "HR";
    public const string Procurement = "Procurement";
    public const string Employee = "Employee";

    public static IReadOnlyList<string> All { get; } =
    [
        Administrator,
        ProjectManager,
        Sales,
        Accountant,
        HR,
        Procurement,
        Employee
    ];
}
