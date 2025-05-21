using System.Text.Json.Serialization;

namespace SistemaDeRelatorioDeVenda.DTO
{
    public class ProdutoVendaDto
    {
        public int ProdutoId { get; set; }
        public string? NomeProduto { get; set; }
        public int QuantidadeEstoque { get; set; }
        public decimal PrecoUnitario { get; set; }

        public int Quantidade { get; set; }// Quantidade de vendas
    }
}