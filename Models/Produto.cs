namespace SistemaDeRelatorioDeVenda.Models;
public class Produto
{
    public int Id { get; set; }
    public string? NomeProduto { get; set; }
    public decimal PrecoProduto { get; set; }
    public int QuantidadeEstoque { get; set; }

    public bool SubtrairEstoque(int quantidade)
    {
        if (quantidade > QuantidadeEstoque)
            return false;

            QuantidadeEstoque -= quantidade;
            return true;
    }
}
