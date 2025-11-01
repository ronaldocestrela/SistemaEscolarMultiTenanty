# ABCSchool Project / Projeto ABCSchool

[English](#english) | [Português](#português)

---

## English

### 📚 Overview

ABCSchool is a multi-tenant school management system built with .NET 9.0 and following Clean Architecture principles. The project provides a robust foundation for managing school-related operations with support for multiple tenants, identity management, and database migrations.

### 🏗️ Architecture

The project follows **Clean Architecture** with clear separation of concerns:

- **Domain Layer**: Core business entities and rules
- **Application Layer**: Use cases and application logic
- **Infrastructure Layer**: Data access, external services, and framework-specific implementations
- **WebAPI Layer**: RESTful API endpoints and presentation logic

### 🚀 Technologies Used

#### Backend Framework
- **.NET 9.0** - Latest version of Microsoft's development platform
- **ASP.NET Core Web API** - For building RESTful APIs
- **Entity Framework Core 9.0.9** - Object-Relational Mapping (ORM)

#### Database
- **Microsoft SQL Server** - Primary database
- **Entity Framework Migrations** - Database versioning and schema management

#### Multi-tenancy
- **Finbuckle.MultiTenant 9.4.0** - Multi-tenant application support
  - Header Strategy for tenant identification
  - Claims Strategy for tenant identification
  - EF Core Store for tenant configuration

#### Identity & Security
- **ASP.NET Core Identity** - User authentication and authorization
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.9** - Identity with EF Core integration

#### Development & Deployment
- **Docker & Docker Compose** - Containerization and orchestration
- **OpenAPI/Swagger** - API documentation and testing

#### Project Structure
- **Clean Architecture** - Separation of concerns with Domain, Application, Infrastructure, and WebAPI layers
- **Dependency Injection** - Built-in DI container

### 📦 Project Structure

```
ABCSchool/
├── src/
│   ├── core/
│   │   ├── Domain/          # Business entities and domain logic
│   │   │   └── Entities/    # Domain entities (School, etc.)
│   │   └── Application/     # Use cases and application services
│   ├── Infrastructure/      # Data access and external services
│   │   ├── Contexts/        # Database contexts
│   │   ├── Identity/        # Identity models and configuration
│   │   ├── Migrations/      # EF Core migrations
│   │   ├── Tenancy/         # Multi-tenant configuration
│   │   └── Constants/       # Application constants
│   └── WebAPI/             # API controllers and configuration
├── docker-compose.yaml     # Docker orchestration
└── ABCSchool.sln          # Visual Studio solution file
```

### 🔧 Features

#### Multi-tenancy Support
- **Tenant Isolation**: Complete data isolation between different school instances
- **Flexible Tenant Strategy**: Support for header-based and claims-based tenant identification
- **Dynamic Tenant Configuration**: Runtime tenant management and configuration

#### Identity Management
- **User Authentication**: Secure user login and session management
- **Role-Based Authorization**: Hierarchical permission system
- **Password Security**: Configurable password policies and validation

#### Database Management
- **Code-First Migrations**: Automatic database schema management
- **Database Seeding**: Initial data population and configuration
- **Connection Management**: Configurable database connections

#### API Features
- **RESTful Endpoints**: Standard HTTP methods and status codes
- **OpenAPI Documentation**: Interactive API documentation
- **CORS Support**: Cross-origin resource sharing configuration

### 🚀 Getting Started

#### Prerequisites
- .NET 9.0 SDK
- Docker & Docker Compose
- SQL Server (or use Docker container)

#### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ABCSchool
   ```

2. **Start the database**
   ```bash
   docker-compose up -d mssql-incubadora
   ```

3. **Update database connection**
   - Update `appsettings.json` with your database connection string

4. **Run migrations**
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/WebAPI
   ```

#### Docker Setup
```bash
# Start database only
docker-compose up -d mssql-incubadora

# Build and run entire application (when uncommented)
docker-compose up --build
```

### 📊 Database Configuration

**Connection String Example:**
```
Server=localhost,3313;Database=ABCSchoolSharedDb;User Id=sa;Password=Ma!s4best4doQu&Ch0ra;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

### 🧪 API Testing

The project includes OpenAPI/Swagger documentation available at:
- Development: `https://localhost:5001/swagger`
- API endpoints can be tested using the included `WebAPI.http` file

### 🔐 Security Features

- **Password Requirements**: Minimum 8 characters, mixed case, numbers, and symbols
- **Unique Email Validation**: Ensures email uniqueness across the system
- **Secure Authentication**: Industry-standard identity management
- **Multi-tenant Security**: Tenant-isolated data access

### 📈 Scalability

- **Multi-tenant Architecture**: Supports multiple school instances
- **Clean Architecture**: Maintainable and testable codebase
- **Docker Support**: Easy deployment and scaling
- **Database Migrations**: Version-controlled schema changes

---

## Português

### 📚 Visão Geral

ABCSchool é um sistema de gerenciamento escolar multi-tenant construído com .NET 9.0 seguindo os princípios da Arquitetura Limpa. O projeto fornece uma base robusta para gerenciar operações relacionadas à escola com suporte para múltiplos tenants, gerenciamento de identidade e migrações de banco de dados.

### 🏗️ Arquitetura

O projeto segue a **Arquitetura Limpa** com clara separação de responsabilidades:

- **Camada de Domínio**: Entidades e regras de negócio principais
- **Camada de Aplicação**: Casos de uso e lógica de aplicação
- **Camada de Infraestrutura**: Acesso a dados, serviços externos e implementações específicas do framework
- **Camada WebAPI**: Endpoints da API RESTful e lógica de apresentação

### 🚀 Tecnologias Utilizadas

#### Framework Backend
- **.NET 9.0** - Versão mais recente da plataforma de desenvolvimento da Microsoft
- **ASP.NET Core Web API** - Para construção de APIs RESTful
- **Entity Framework Core 9.0.9** - Mapeamento Objeto-Relacional (ORM)

#### Banco de Dados
- **Microsoft SQL Server** - Banco de dados principal
- **Migrações Entity Framework** - Versionamento de banco de dados e gerenciamento de schema

#### Multi-tenancy
- **Finbuckle.MultiTenant 9.4.0** - Suporte para aplicações multi-tenant
  - Estratégia de Header para identificação de tenant
  - Estratégia de Claims para identificação de tenant
  - EF Core Store para configuração de tenant

#### Identidade e Segurança
- **ASP.NET Core Identity** - Autenticação e autorização de usuário
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.9** - Identity com integração EF Core

#### Desenvolvimento e Deploy
- **Docker & Docker Compose** - Containerização e orquestração
- **OpenAPI/Swagger** - Documentação e teste de API

#### Estrutura do Projeto
- **Arquitetura Limpa** - Separação de responsabilidades com camadas Domain, Application, Infrastructure e WebAPI
- **Injeção de Dependência** - Container DI integrado

### 📦 Estrutura do Projeto

```
ABCSchool/
├── src/
│   ├── core/
│   │   ├── Domain/          # Entidades de negócio e lógica de domínio
│   │   │   └── Entities/    # Entidades de domínio (School, etc.)
│   │   └── Application/     # Casos de uso e serviços de aplicação
│   ├── Infrastructure/      # Acesso a dados e serviços externos
│   │   ├── Contexts/        # Contextos de banco de dados
│   │   ├── Identity/        # Modelos e configuração de identidade
│   │   ├── Migrations/      # Migrações EF Core
│   │   ├── Tenancy/         # Configuração multi-tenant
│   │   └── Constants/       # Constantes da aplicação
│   └── WebAPI/             # Controllers da API e configuração
├── docker-compose.yaml     # Orquestração Docker
└── ABCSchool.sln          # Arquivo de solução Visual Studio
```

### 🔧 Funcionalidades

#### Suporte Multi-tenant
- **Isolamento de Tenant**: Isolamento completo de dados entre diferentes instâncias de escola
- **Estratégia de Tenant Flexível**: Suporte para identificação de tenant baseada em header e claims
- **Configuração Dinâmica de Tenant**: Gerenciamento e configuração de tenant em tempo de execução

#### Gerenciamento de Identidade
- **Autenticação de Usuário**: Login seguro de usuário e gerenciamento de sessão
- **Autorização Baseada em Função**: Sistema de permissão hierárquica
- **Segurança de Senha**: Políticas de senha configuráveis e validação

#### Gerenciamento de Banco de Dados
- **Migrações Code-First**: Gerenciamento automático de schema de banco de dados
- **Seeding de Banco de Dados**: População inicial de dados e configuração
- **Gerenciamento de Conexão**: Conexões de banco de dados configuráveis

#### Recursos da API
- **Endpoints RESTful**: Métodos HTTP padrão e códigos de status
- **Documentação OpenAPI**: Documentação interativa da API
- **Suporte CORS**: Configuração de compartilhamento de recursos entre origens

### 🚀 Primeiros Passos

#### Pré-requisitos
- .NET 9.0 SDK
- Docker & Docker Compose
- SQL Server (ou usar container Docker)

#### Instalação

1. **Clonar o repositório**
   ```bash
   git clone <repository-url>
   cd ABCSchool
   ```

2. **Iniciar o banco de dados**
   ```bash
   docker-compose up -d mssql-incubadora
   ```

3. **Atualizar conexão do banco de dados**
   - Atualize `appsettings.json` com sua string de conexão do banco de dados

4. **Executar migrações**
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/WebAPI
   ```

5. **Executar a aplicação**
   ```bash
   dotnet run --project src/WebAPI
   ```

#### Configuração Docker
```bash
# Iniciar apenas o banco de dados
docker-compose up -d mssql-incubadora

# Construir e executar toda a aplicação (quando descomentado)
docker-compose up --build
```

### 📊 Configuração do Banco de Dados

**Exemplo de String de Conexão:**
```
Server=localhost,3313;Database=ABCSchoolSharedDb;User Id=sa;Password=Ma!s4best4doQu&Ch0ra;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

### 🧪 Teste da API

O projeto inclui documentação OpenAPI/Swagger disponível em:
- Desenvolvimento: `https://localhost:5001/swagger`
- Endpoints da API podem ser testados usando o arquivo `WebAPI.http` incluído

### 🔐 Recursos de Segurança

- **Requisitos de Senha**: Mínimo 8 caracteres, maiúsculas/minúsculas, números e símbolos
- **Validação de Email Único**: Garante unicidade de email no sistema
- **Autenticação Segura**: Gerenciamento de identidade padrão da indústria
- **Segurança Multi-tenant**: Acesso a dados isolado por tenant

### 📈 Escalabilidade

- **Arquitetura Multi-tenant**: Suporta múltiplas instâncias de escola
- **Arquitetura Limpa**: Base de código mantível e testável
- **Suporte Docker**: Deploy e escalonamento fáceis
- **Migrações de Banco de Dados**: Mudanças de schema controladas por versão

---

## 🤝 Contributing / Contribuindo

Contributions are welcome! Please feel free to submit a Pull Request.

Contribuições são bem-vindas! Sinta-se à vontade para submeter um Pull Request.

## 📄 License / Licença

This project is licensed under the MIT License.

Este projeto está licenciado sob a Licença MIT.