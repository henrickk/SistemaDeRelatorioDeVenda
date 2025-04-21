namespace SistemaDeRelatorioDeVenda.Models;
public class Pedido
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public List<ItemPedido>? Itens { get; set; }

    public decimal Total => Itens?.Sum(i => i.Quantidade * i.PrecoUnitario) ?? 0;
}
