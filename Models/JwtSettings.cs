namespace SistemaDeRelatorioDeVenda.Models
{
    public class JwtSettings
    {
        public string? Segredo { get; set; }
        public int ExpiracaoHoras { get; set; }
        public string? Emissor { get; set; }
        public string? Audience { get; set; }
    }
}
