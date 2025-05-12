using System.Text.Json.Serialization;

namespace PermissionManager.Domain.Entities;

public class PermissionType
{
    public int Id { get; set; }
    
    public required string Description { get; set; }

    // Navigation
    [JsonIgnore]
    public ICollection<Permission> Permissions { get; set; } = null!;
}