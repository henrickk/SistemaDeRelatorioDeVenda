using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeRelatorioDeVenda.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirRelacionamentoProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensPedido_Produtos_ProdutoId1",
                table: "ItensPedido");

            migrationBuilder.DropIndex(
                name: "IX_ItensPedido_ProdutoId1",
                table: "ItensPedido");

            migrationBuilder.DropColumn(
                name: "ProdutoId1",
                table: "ItensPedido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoId1",
                table: "ItensPedido",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedido_ProdutoId1",
                table: "ItensPedido",
                column: "ProdutoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPedido_Produtos_ProdutoId1",
                table: "ItensPedido",
                column: "ProdutoId1",
                principalTable: "Produtos",
                principalColumn: "Id");
        }
    }
}
