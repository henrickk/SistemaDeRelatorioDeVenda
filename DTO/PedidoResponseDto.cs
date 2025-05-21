namespace SistemaDeRelatorioDeVenda.DTO
{
    public class PedidoResponseDto
    {
        public int PedidoId { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal Total { get; set; }
    }
}
