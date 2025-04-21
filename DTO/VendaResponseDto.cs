namespace SistemaDeRelatorioDeVenda.DTO
{
    public class VendaResponseDto
    {
        public int PedidoId { get; set; }
        public string? Cliente { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public List<ProdutoVendaDto>? Produtos { get; set; }
    }

}
