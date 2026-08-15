namespace Clientes.DataAccess.Entities;

public class Cliente
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string? ApellidoMaterno { get; set; }
    public string CorreoElectronico { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? CodigoPostal { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaModificacion { get; set; }
}
