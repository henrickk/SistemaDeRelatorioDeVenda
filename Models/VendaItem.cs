namespace SistemaDeRelatorioDeVenda.Models
{
    public class VendaItem
    {
        public int Íd { get; set; }
        public int VendaId { get; set; }
        public string? Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}