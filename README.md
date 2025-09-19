## Índice

- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuração)
- [Execução](#execução)
- [Docker](#docker)

## Tecnologias

- .NET 8
- C#
- Entity Framework Core
- Dapper
- Cache
- MySQL (ou outro banco relacional)
- Docker (opcional para containerização)
- xUnit / Moq (para testes unitários)

---

## Pré-requisitos

Antes de executar o projeto, você precisa ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MySQL](https://dev.mysql.com/downloads/installer/) ou outro banco compatível
- [Docker](https://www.docker.com/get-started) (opcional)

---
## Configuração

- Clone o repositório:
- Crie o banco(o script de criacao do banco esta no arquivo raiz de cada Api)
- Abra cada projeto respectivamente e copie o conteúdo do arquivo: secrets.json
- Cole esse conteúdo nos Secrets da aplicação e substitua a connectionString pela sua conexão de banco.

---
## Docker

Cada api possue um Dockerfile e um script para criacao do banco no MySql

E na raiz do projeto tem um docker-compose.yml, execute o comando:
``bash
docker-compose up --build``

---
