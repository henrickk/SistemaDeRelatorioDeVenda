using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SistemaDeRelatorioDeVenda.Data;
using SistemaDeRelatorioDeVenda.DTO;
using SistemaDeRelatorioDeVenda.Models;
using System.ComponentModel;

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
                    ClienteId = p.ClienteId,
                    NomeCliente = p.Cliente.NomeCliente,
                    Data = p.DataPedido,
                    Total = p.Total,
                    Produtos = p.Itens.Select(i => new ProdutoVendaDto
                    {
                        ProdutoId = i.ProdutoId,
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
                    ClienteId = p.ClienteId,
                    NomeCliente = p.Cliente.NomeCliente,
                    Data = p.DataPedido,
                    Total = p.Total,
                    Produtos = p.Itens.Select(i => new ProdutoVendaDto
                    {
                        ProdutoId = i.ProdutoId,
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

        [HttpGet]
        [Route("relatorio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VendaResponseDto>>> ConsultarRelatorioVendas(
            [FromQuery] int? clienteId,
            [FromQuery] int? produtoId,
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .AsQueryable();

            if (clienteId.HasValue)
                query = query.Where(p => p.ClienteId == clienteId);

            if (dataInicial.HasValue)
                query = query.Where(p => p.DataPedido >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(p => p.DataPedido <= dataFinal.Value);

            if (produtoId.HasValue)
                query = query.Where(p => p.Itens.Any(i => i.ProdutoId == produtoId));

            var vendas = await query
                .Select(p => new VendaResponseDto
                {
                    PedidoId = p.Id,
                    ClienteId = p.ClienteId,
                    NomeCliente = p.Cliente.NomeCliente,
                    Data = p.DataPedido,
                    Total = p.Total,
                    Produtos = p.Itens.Select(i => new ProdutoVendaDto
                    {
                        ProdutoId = i.ProdutoId,
                        NomeProduto = i.Produto.NomeProduto,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                })
                .ToListAsync();

            if (!vendas.Any())
                return NotFound("Nenhuma venda encontrada com os filtros informados.");

            return Ok(vendas);

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

            var itensPedido = new List<ItemPedido>();
            foreach (var p in venda.Produtos)
            {
                var produto = await _context.Produtos.FindAsync(p.ProdutoId);
                if (produto == null)
                    return BadRequest($"Produto com ID {p.ProdutoId} não encontrado.");

                if (!produto.SubtrairEstoque(p.Quantidade))
                    return BadRequest($"Estoque insuficiente para o produto {produto.NomeProduto}.");

                itensPedido.Add(new ItemPedido
                {
                    ProdutoId = p.ProdutoId,
                    Produto = produto,
                    PrecoUnitario = produto.PrecoProduto,
                    Quantidade = p.Quantidade
                });
            }

            var pedido = new Pedido
            {
                DataPedido = DateTime.Now,
                ClienteId = venda.ClienteId,
                Itens = itensPedido
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var vendaResponse = new VendaResponseDto
            {
                PedidoId = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = venda.NomeCliente,
                Data = pedido.DataPedido,
                Total = pedido.Total,
                Produtos = pedido.Itens.Select(i => new ProdutoVendaDto
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto.NomeProduto,
                    QuantidadeEstoque = i.Produto.QuantidadeEstoque,
                    PrecoUnitario = i.Produto.PrecoProduto,
                    Quantidade = i.Quantidade
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

        [HttpGet]
        public async Task<ActionResult> ExportarRelatorioExcel(
            [FromQuery] int? clienteId,
            [FromQuery] int? produtoId,
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p =>p.Itens)
                    .ThenInclude(i => i.Produto)
                .AsQueryable();

            if (clienteId.HasValue)
                query = query.Where(p => p.ClienteId == clienteId);
            
            if (produtoId.HasValue)
                query = query.Where(p => p.Itens.Any(i => i.ProdutoId == produtoId));

            if (dataInicial.HasValue)
                query = query.Where(p => p.DataPedido >= dataInicial.Value);

            if(dataFinal.HasValue)
                query = query.Where(p => p.DataPedido <= dataFinal.Value);

            var vendas = await query.ToListAsync();

            if (!vendas.Any())
                return NotFound("Nenhuma venda encontrada com os filtros informados.");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("RelatorioVendas"); ;

            worksheet.Cells[1, 1].Value = "Pedido ID";
            worksheet.Cells[1, 2].Value = "Cliente";
            worksheet.Cells[1, 3].Value = "Data";
            worksheet.Cells[1, 4].Value = "Produto";
            worksheet.Cells[1, 5].Value = "Quantidade"; 
            worksheet.Cells[1, 6].Value = "Preço Unitário";   
            worksheet.Cells[1, 7].Value = "Total";

            int linha = 2;

            foreach(var venda in vendas)
            {
                foreach(var item in venda.Itens)
                {
                    worksheet.Cells[linha, 1].Value = venda.Id;
                    worksheet.Cells[linha, 2].Value = venda.Cliente.NomeCliente;
                    worksheet.Cells[linha, 3].Value = venda.DataPedido.ToString("dd/MM/yyyy");
                    worksheet.Cells[linha, 4].Value = item.Produto.NomeProduto;
                    worksheet.Cells[linha, 5].Value = item.Quantidade;
                    worksheet.Cells[linha, 6].Value = item.PrecoUnitario;
                    worksheet.Cells[linha, 7].Value = item.Quantidade * item.PrecoUnitario;
                    linha++;
                }
            }

            var excelBytes = package.GetAsByteArray();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RelatorioVendas.xlsx");
        }

    }

}
