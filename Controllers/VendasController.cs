using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.DTO;
using SistemaDeRelatorioDeVenda.Models;

namespace SistemaDeRelatorioDeVenda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public VendasController(ApiDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("consultar-vendas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VendaResponseDto>>> ConsultarVendas()
        {   
            var vendas = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Select(p => new VendaResponseDto
                {
                    PedidoId = p.Id,
                    NomeCliente = p.Cliente.NomeCliente,
                    Data = p.DataPedido,
                    Total = p.Total,
                    Produtos = p.Itens.Select(i => new ProdutoVendaDto
                    {
                        NomeProduto = i.Produto.NomeProduto,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                })
                .ToListAsync();
            return Ok(vendas);
    }
}
}
