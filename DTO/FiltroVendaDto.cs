namespace SistemaDeRelatorioDeVenda.DTO
{
    public class FiltroVendaDto
    {
        public int ClienteId { get; set; }
        public int ProdutoId { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
    }
}
