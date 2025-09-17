📌 Projeto: API de Registro de Veículos com JWT e Minimal APIs

Este projeto é uma API REST desenvolvida em .NET utilizando a técnica de Minimal APIs.
O objetivo é realizar o registro e gerenciamento de veículos, garantindo que apenas administradores autenticados possam cadastrar, editar ou excluir registros.

A aplicação conta com:

✅ Autenticação e Autorização com JWT

Login de administradores com geração de token JWT.

Proteção de rotas sensíveis com política de autorização ("AdminPolicy").

✅ Gestão de Veículos

Listagem pública de veículos registrados.

Criação, atualização e remoção disponíveis apenas para administradores autenticados.

✅ Swagger/OpenAPI

Documentação automática da API.

Testes diretos pelo navegador, incluindo suporte ao envio do token JWT pelo botão Authorize.

✅ Banco de Dados InMemory (para testes)

Utilização do Entity Framework Core InMemory para persistência simples.

Fácil migração para SQL Server, PostgreSQL ou SQLite.

✅ Testes de Integração

Testes com xUnit e FluentAssertions para garantir robustez.

Validação do fluxo de login, proteção de rotas e criação de veículos.

🚀 Tecnologias utilizadas

.NET 6/7/8 (Minimal APIs)

Entity Framework Core 

JWT (JSON Web Token)

Swagger / OpenAPI

xUnit + FluentAssertions (testes)
