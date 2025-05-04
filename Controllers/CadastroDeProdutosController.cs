using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.DTO;
using SistemaDeRelatorioDeVenda.Models;

namespace SistemaDeRelatorioDeVenda.Controllers
{
    [ApiController]
    [Route("api/CadastroDeProdutos")]
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

        [HttpGet]
        [Route("consultar-produto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Produto>> BuscarProdutoPorId(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }
            return Ok(produto);
        }

        [HttpPost]
        [Route("cadastrar-produto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProdutoVendaDto>> CadastrarProduto()
        {
            var produto = new Produto();

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

        [HttpDelete]
        [Route("deletar-produto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletarProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return Ok("Produto deletado com sucesso.");
        }
    }
}
