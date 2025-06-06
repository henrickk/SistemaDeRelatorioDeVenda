using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        [HttpGet]
        [Route("exportar-relatorio-por-cliente-excel")]
        public async Task<ActionResult> ExportarRelatorioPorClienteExcel(
            [FromQuery] int? clienteId,
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var query = _context.Clientes
                .Include(c => c.Pedidos)
                    .ThenInclude(i => i.Itens)
                .AsQueryable();

            if (clienteId.HasValue)
                query = query.Where(c => c.Id == clienteId);


            if (dataInicial.HasValue)
                query = query.Where(c => c.Pedidos.Any(p => p.DataPedido >= dataInicial));

            if (dataFinal.HasValue)
                query = query.Where(c => c.Pedidos.Any(p => p.DataPedido <= dataFinal));

            var pedidos = await query.ToListAsync();

            if (!pedidos.Any())
                return NotFound("Nenhum pedido encontrado com os filtros informados.");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("RelatorioVendas");

            worksheet.Cells[1, 1].Value = "Cliente ID";
            worksheet.Cells[1, 2].Value = "Cliente";
            worksheet.Cells[1, 3].Value = "Pedido ID";
            worksheet.Cells[1, 4].Value = "Data";
            worksheet.Cells[1, 5].Value = "Total";
            
            int linha = 2;

            foreach (var pedido in pedidos)
            {
                foreach (var item in pedido.Pedidos)
                {
                    worksheet.Cells[linha, 1].Value = pedido.Id;
                    worksheet.Cells[linha, 2].Value = pedido.NomeCliente;
                    worksheet.Cells[linha, 3].Value = item.Id;
                    worksheet.Cells[linha, 4].Value = item.DataPedido.ToString("dd/MM/yyyy");
                    worksheet.Cells[linha, 5].Value = item.Total;
                    linha++;
                }
            }

            var excelBytes = package.GetAsByteArray();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportarRelatorioPorClienteExcel.xlsx");

        }
    }
}
