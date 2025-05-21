using System.Text.Json.Serialization;

namespace SistemaDeRelatorioDeVenda.DTO
{
    public class VendaResponseDto
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public string? NomeCliente { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
        public List<ProdutoVendaDto>? Produtos { get; set; }
    }

}
