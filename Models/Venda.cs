namespace SistemaDeRelatorioDeVenda.Models
{
    public class Venda
    {
        private static readonly List<VendaItem> vendaItems = new List<VendaItem>();

        public int Id { get; set; }
        public string? Cliente { get; set; }
        public int MyProperty { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }
        List<VendaItem> Itens { get; set; } = vendaItems;
    }
}