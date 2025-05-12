using System.Text.Json.Serialization;

namespace PermissionManager.Domain.Entities;

/// <summary>
/// Represents a type of permission that can be granted to an employee
/// </summary>
public class PermissionType
{
    /// <summary>
    /// The id of the permission type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The description of the permission type
    /// </summary>
    public required string Description { get; set; }

    // Navigation
    [JsonIgnore]
    public ICollection<Permission> Permissions { get; set; } = null!;
}