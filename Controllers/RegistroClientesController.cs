using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
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

    }
}
