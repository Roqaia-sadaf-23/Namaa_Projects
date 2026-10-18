using System.ComponentModel.DataAnnotations;

namespace Namaa.Api.Contracts.ProjectFiles;

public sealed class UploadProjectFileRequest
{
    [Required]
    public IFormFile File { get; init; } = null!;

    [StringLength(500)]
    public string? Description { get; init; }
}
