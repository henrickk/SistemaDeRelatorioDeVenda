namespace SistemaDeRelatorioDeVenda.Models;
public class Produto
{
    public int Id { get; set; }
    public string? NomeProduto { get; set; }
    public decimal PrecoProduto { get; set; }

    public List<ItemPedido>? ItensPedido { get; set; }
}
