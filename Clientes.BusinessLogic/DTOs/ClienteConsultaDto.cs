namespace Clientes.BusinessLogic.DTOs;

public class ClienteConsultaDto
{
    public int Pagina { get; set; } = 1;
    public int TamanioPagina { get; set; } = 10;
    public string? Buscar { get; set; }
    public bool? Activo { get; set; }
    public string? OrdenarPor { get; set; }
    public bool Descendente { get; set; }
}
