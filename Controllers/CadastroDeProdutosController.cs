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
        public async Task<ActionResult<ProdutoVendaDto>> CadastrarProduto(ProdutoVendaDto produto)
        {
            if (produto == null || string.IsNullOrWhiteSpace(produto.NomeProduto))
            {
                return BadRequest("Nome do produto é obrigatório.");
            }
            var novoProduto = new Produto
            {
                NomeProduto = produto.NomeProduto,
                PrecoProduto = produto.PrecoUnitario,
                QuantidadeEstoque = produto.QuantidadeEstoque
            };
            _context.Produtos.Add(novoProduto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(BuscarProdutos), new { id = novoProduto.Id }, novoProduto);
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
