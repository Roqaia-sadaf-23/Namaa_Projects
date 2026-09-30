namespace Namaa.Domain.Entities;

public class Role
{
    private Role()
    {
    }

    private Role(string name, string? description)
    {
        Name = DomainGuard.Required(name, nameof(name));
        Description = description;
    }

    public byte Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public ICollection<User> Users { get; private set; } = new List<User>();

    public static Role Create(string name, string? description = null) => new(name, description);

    public void Update(string name, string? description = null)
    {
        Name = DomainGuard.Required(name, nameof(name));
        Description = description;
    }
}
