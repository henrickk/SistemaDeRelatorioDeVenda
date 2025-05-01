using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.Models;

namespace SistemaDeRelatorioDeVenda.Controllers
{
    [ApiController]
    [Route("api/RegistroDeProdutos")]
    public class CadastroDeProdutosController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public CadastroDeProdutosController(ApiDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Produto>>> BuscarProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();
            if (produtos == null || !produtos.Any())
            {
                return NotFound("Nenhum produto encontrado.");
            }
            return Ok(produtos);

        }

    }
}
