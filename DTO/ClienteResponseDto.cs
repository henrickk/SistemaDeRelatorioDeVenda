namespace SistemaDeRelatorioDeVenda.DTO
{
    public class ClienteResponseDto
    {
        public int Id { get; set; }
        public string? NomeCliente { get; set; }
        public List<PedidoResponseDto> PedidosCliente { get; set; } = new();
    }

}
