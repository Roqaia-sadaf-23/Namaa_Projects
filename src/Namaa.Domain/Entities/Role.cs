namespace Namaa.Domain.Entities;

public class Role
{
    public byte Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
