using Namaa.Domain.Enums;

namespace Namaa.Api.Contracts.Projects;

public sealed record ChangeProjectStatusRequest(ProjectStatus Status, string? Reason);

