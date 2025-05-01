using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.DTO;
using SistemaDeRelatorioDeVenda.Models;

namespace SistemaDeRelatorioDeVenda.Controllers
{
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
            var clientes = await _context.Clientes.ToListAsync();
            if (clientes == null || !clientes.Any())
            {
                return NotFound("Nenhum cliente encontrado.");
            }
            return Ok(clientes);
        }

        [HttpPost]
        [Route("cadastrar-cliente")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Cliente>> CadastrarCliente([FromBody] ClienteCreateDto clienteDto)
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
