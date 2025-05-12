using System.Text.Json.Serialization;

namespace PermissionManager.Domain.Entities;

/// <summary>
/// Represents a Permission granted to an employee
/// </summary>
public class Permission
{
    /// <summary>
    /// The id of the permission
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The employee name
    /// </summary>
    [JsonPropertyName("NombreEmpleado")]
    public required string EmployeeName { get; set; }
    
    /// <summary>
    /// The employee surname
    /// </summary>
    [JsonPropertyName("ApellidoEmpleado")]
    public required string EmployeeSurname { get; set; }
    
    /// <summary>
    /// The permission type id
    /// </summary>
    [JsonPropertyName("TipoPermiso")]
    public int PermissionTypeId { get; set; }
    
    /// <summary>
    /// The date and time when it was created
    /// </summary>
    [JsonPropertyName("FechaPermiso")]
    public DateTime CreatedAt { get; init; }
    
    // Navigation key
    [JsonIgnore]
    public PermissionType PermissionType { get; set; } = null!;
}