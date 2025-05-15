using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.DTO;
using SistemaDeRelatorioDeVenda.Models;

namespace SistemaDeRelatorioDeVenda.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/vendas")]
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

        [HttpGet]
        [Route("consultar-venda/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VendaResponseDto>> ConsultarVendaPorId(int id)
        {
            var venda = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => p.Id == id)
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
                }).FirstOrDefaultAsync();
            if (venda == null)
            {
                return NotFound();
            }
            return Ok(venda);
        }

        [HttpPost]
        [Route("registrar-venda")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VendaResponseDto>> RegistrarVenda(VendaResponseDto venda)
        {
            if (venda == null)
            {
                return BadRequest();
            }

            var pedido = new Pedido
            {
                DataPedido = DateTime.Now,
                ClienteId = venda.ClienteId,
                Itens = venda.Produtos.Select(p => new ItemPedido
                {
                    ProdutoId = p.ProdutoId,
                    Quantidade = p.Quantidade,
                    PrecoUnitario = p.PrecoUnitario
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            var vendaResponse = new VendaResponseDto
            {
                PedidoId = pedido.Id,
                NomeCliente = venda.NomeCliente,
                Data = pedido.DataPedido,
                Total = pedido.Total,
                Produtos = pedido.Itens.Select(i => new ProdutoVendaDto
                {
                    NomeProduto = i.Produto.NomeProduto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };
            return CreatedAtAction(nameof(ConsultarVendaPorId), new { id = pedido.Id }, vendaResponse);
        }

        [HttpPut]
        [Route("atualizar-venda/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VendaResponseDto>> AtualizarVenda(int id, VendaResponseDto venda)
        {
            if (venda == null || id != venda.PedidoId)
            {
                return BadRequest();
            }
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            string? nomeCliente = pedido.Cliente.NomeCliente;
            pedido.ClienteId = venda.PedidoId;
            pedido.DataPedido = DateTime.Now;
            pedido.Itens = venda.Produtos.Select(p => new ItemPedido
            {
                ProdutoId = p.ProdutoId,
                Quantidade = p.Quantidade,
                PrecoUnitario = p.PrecoUnitario
            }).ToList();
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
            var vendaResponse = new VendaResponseDto
            {
                NomeCliente = venda.NomeCliente,
                Data = pedido.DataPedido,
                Total = pedido.Total,
                Produtos = pedido.Itens.Select(i => new ProdutoVendaDto
                {
                    NomeProduto = i.Produto.NomeProduto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };
            return Ok(vendaResponse);
        }

        [HttpDelete]
        [Route("deletar-venda/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarVenda(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
