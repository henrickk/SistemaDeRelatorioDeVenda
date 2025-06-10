# Sistema de Relatório de Venda

Este projeto é uma API desenvolvida em ASP.NET Core (.NET 8) para gerenciamento de vendas, clientes, produtos e geração de relatórios em Excel. Utiliza Entity Framework Core para persistência de dados e a biblioteca EPPlus para exportação de relatórios.

## Funcionalidades

- Cadastro, consulta e exclusão de clientes
- Cadastro e consulta de produtos
- Registro de pedidos e itens de pedido
- Filtros por cliente, produto e período
- Exportação de relatórios de vendas em formato Excel (.xlsx)
- Autenticação e autorização via Identity

## Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core
- EPPlus (geração de arquivos Excel)
- SQL Server (ou outro banco compatível)
- Identity para autenticação

## Como Executar

1. Clone o repositório:
  git clone https://github.com/seu-usuario/seu-repositorio.git

2. Configure a string de conexão no arquivo 
  `appsettings.json`.
3. Execute as migrações do banco de dados:
  dotnet ef database update

4. Inicie a aplicação:
  dotnet run


## Exemplos de Endpoints

- `GET /api/RegistroClientes/consultar-clientes`  
  Lista todos os clientes cadastrados.

- `POST /api/RegistroClientes/cadastrar-cliente`  
  Cadastra um novo cliente.

- `GET /api/RegistroClientes/exportar-relatorio-por-cliente-excel`  
  Exporta relatório de pedidos por cliente em Excel.

- `GET /api/Vendas/exportar-relatorio-excel`  
  Exporta relatório de vendas filtrando por cliente, produto e datas.

## Observações

- O projeto utiliza a licença não comercial do EPPlus.
- Para acessar os endpoints protegidos, é necessário autenticação.

## Contribuição

Contribuições são bem-vindas! Abra uma issue ou envie um pull request.

---

> Feito com 💻 por Henrick Adrian  
> Licença: MIT
