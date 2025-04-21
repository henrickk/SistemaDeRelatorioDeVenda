using Microsoft.AspNetCore.Mvc;
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
        //[HttpGet]
        //[Route("consultar-vendas")]
        //[ProducesResponseType(typeof(IEnumerable<VendaResponseDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<IEnumerable<VendaResponseDto>>> ConsultarVendas()
        //{ 

        //}
    }
}
