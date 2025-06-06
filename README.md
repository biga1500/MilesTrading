# Backend .NET para Sistema de Compra e Venda de Milhas Aéreas

Este diretório contém o código-fonte do backend em .NET para o sistema de compra e venda de milhas aéreas.

## Estrutura do Projeto

O backend segue uma arquitetura em camadas, conforme definido no documento de arquitetura:

- **API**: Controllers e DTOs para exposição dos endpoints REST
- **Business**: Serviços e regras de negócio
- **Data**: Repositórios e acesso ao banco de dados MySQL
- **Models**: Entidades do domínio
- **Infrastructure**: Configurações, segurança e utilitários

## Tecnologias Utilizadas

- ASP.NET Core 6+
- Entity Framework Core
- MySQL Connector
- JWT para autenticação
- AutoMapper
- FluentValidation

## Configuração do Ambiente

1. Instalar o .NET SDK 6.0 ou superior
2. Instalar o MySQL Server
3. Executar o script SQL para criação do banco de dados
4. Configurar a string de conexão no arquivo appsettings.json
5. Executar o comando `dotnet restore` para restaurar os pacotes
6. Executar o comando `dotnet run` para iniciar a aplicação

## Endpoints da API

### Autenticação
- POST /api/auth/register - Registro de novo usuário
- POST /api/auth/login - Login de usuário

### Usuários
- GET /api/users - Listar usuários
- GET /api/users/{id} - Obter usuário por ID
- GET /api/users/{id}/miles - Obter saldo de milhas do usuário
- PUT /api/users/{id} - Atualizar usuário

### Programas de Fidelidade
- GET /api/loyalty-programs - Listar programas de fidelidade
- GET /api/loyalty-programs/{id} - Obter programa por ID

### Ofertas
- GET /api/offers - Listar ofertas
- GET /api/offers/{id} - Obter oferta por ID
- POST /api/offers - Criar nova oferta
- PUT /api/offers/{id} - Atualizar oferta
- DELETE /api/offers/{id} - Cancelar oferta

### Transações
- GET /api/transactions - Listar transações
- GET /api/transactions/{id} - Obter transação por ID
- POST /api/transactions - Criar nova transação
- PUT /api/transactions/{id}/status - Atualizar status da transação

## Segurança

O sistema utiliza JWT (JSON Web Tokens) para autenticação e autorização. Todos os endpoints, exceto os de autenticação, requerem um token válido.

## Regras de Negócio

- Mínimo de 10.000 milhas por transação
- Saldo de milhas não pode ficar negativo
- Ofertas só podem ser canceladas se não houver transações associadas
- Transações alteram automaticamente o saldo de milhas e financeiro dos usuários
