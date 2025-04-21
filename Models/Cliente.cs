namespace SistemaDeRelatorioDeVenda.Models;
public class Cliente
{
    public int Id { get; set; }
    public string? Nome { get; set; }

    // Relacionamento
    public List<Pedido>? Pedidos { get; set; }
}
