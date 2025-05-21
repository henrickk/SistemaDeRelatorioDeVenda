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
    [Route("api/[controller]")]
    public class RegistroClientesController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public RegistroClientesController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("consultar-clientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Cliente>>> ConsultarClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.Itens)
                .Select(c => new ClienteResponseDto
                {
                    Id = c.Id,
                    NomeCliente = c.NomeCliente,
                    PedidosCliente = c.Pedidos.Select(p => new PedidoResponseDto
                    {
                        PedidoId = p.Id,
                        DataPedido = p.DataPedido,
                        Total = p.Total
                    }).ToList()
                }).ToListAsync();

            if (clientes == null || !clientes.Any())
            {
                return NotFound("Nenhum cliente encontrado.");
            }
            return Ok(clientes);
        }

        [HttpGet]
        [Route("consultar-cliente-por-nome")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteResponseDto>> ConsultarClientePorNome(string nomeCliente)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                .Where(c => c.NomeCliente == nomeCliente)
                .Select(c => new ClienteResponseDto
                {
                    Id = c.Id,
                    NomeCliente = c.NomeCliente,
                    PedidosCliente = c.Pedidos.Select(p => new PedidoResponseDto
                    {
                        PedidoId = p.Id,
                        DataPedido = p.DataPedido,
                        Total = p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            return Ok(cliente);
        }


        [HttpPost]
        [Route("cadastrar-cliente")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Cliente>> CadastrarCliente(NomeClienteDto clienteDto)
        {
            if (clienteDto == null || string.IsNullOrWhiteSpace(clienteDto.NomeCliente))
            {
                return BadRequest("Nome do cliente é obrigatório.");
            }

            var cliente = new Cliente { NomeCliente = clienteDto.NomeCliente };
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ConsultarClientes), new { id = cliente.Id }, cliente);
        }

        [HttpDelete]
        [Route("deletar-cliente/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok("Cliente deletado com sucesso.");
        }

    }
}
