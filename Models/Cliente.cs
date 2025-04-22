namespace SistemaDeRelatorioDeVenda.Models;
public class Cliente
{
    public int Id { get; set; }
    public string? NomeCliente { get; set; }

    // Relacionamento
    public List<Pedido>? Pedidos { get; set; }
}
