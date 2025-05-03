using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.DTO;
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
        [Route("consultar-produtos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Produto>>> BuscarProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();
            if (produtos == null || !produtos.Any())
            {
                return NotFound("Nenhum produto encontrado.");
            }
            return Ok(produtos);
        }

        [HttpPost]
        [Route("cadastrar-produto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProdutoVendaDto>> CadastrarProduto()
        {
            var produto = new Produto
            {
                NomeProduto = "Produto Exemplo",
                PrecoProduto = 10.00m
            };
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            var produtoDto = new ProdutoVendaDto
            {
                NomeProduto = produto.NomeProduto,
                Quantidade = 1,
                PrecoUnitario = produto.PrecoProduto
            };
            return CreatedAtAction(nameof(BuscarProdutos), new { id = produto.Id }, produtoDto);
        }
    }
}
