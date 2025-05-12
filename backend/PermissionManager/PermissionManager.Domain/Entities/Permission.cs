using System.Text.Json.Serialization;

namespace PermissionManager.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    
    [JsonPropertyName("NombreEmpleado")]
    public required string EmployeeName { get; set; }
    
    [JsonPropertyName("ApellidoEmpleado")]
    public required string EmployeeSurname { get; set; }
    
    [JsonPropertyName("TipoPermiso")]
    public int PermissionTypeId { get; set; }
    
    [JsonPropertyName("FechaPermiso")]
    public DateTime CreatedAt { get; init; }
    
    // Navigation key
    [JsonIgnore]
    public PermissionType PermissionType { get; set; } = null!;
}