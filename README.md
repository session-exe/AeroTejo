# AeroTejo - Sistema de Gestão de Agência de Viagens Aéreas

## Descrição

O **AeroTejo** é um sistema completo de gestão de agência de viagens aéreas desenvolvido em ASP.NET Core 8.0 MVC com MySQL. O sistema permite a gestão de voos, hotéis e reservas, com funcionalidades distintas para passageiros e administradores.

## Tecnologias Utilizadas

- **Framework:** ASP.NET Core 8.0 (Model-View-Controller)
- **Linguagem:** C#
- **Base de Dados:** MySQL
- **ORM:** Entity Framework Core com Pomelo.EntityFrameworkCore.MySql
- **Frontend:** Bootstrap 5
- **Gráficos:** Chart.js
- **PDF:** QuestPDF
- **Autenticação:** Sistema manual com Password Hashing e Salt

## Características Principais

### Tema Visual
- **Dark Mode** predominante com esquema de cores Roxo e Preto
- **Light Mode** disponível através de botão de alternância
- Interface responsiva e moderna

### Funcionalidades para Visitantes
- Pesquisa de voos por destino
- Visualização de catálogo de voos e hotéis
- Registo de nova conta (papel de "Passageiro" atribuído automaticamente)
- Login com e-mail e password

### Funcionalidades para Passageiros
- Pesquisa e reserva de voos
- Seleção de hotéis no destino do voo
- Escolha de assento através de dropdown
- Introdução de dados de passageiros
- Checkout com dados de pagamento (mock) e faturação
- Visualização do histórico de reservas pessoais
- Geração de fatura em PDF

### Funcionalidades para Administradores
- Dashboard com estatísticas e gráficos:
  - Total de vendas/reservas por mês
  - Destinos mais populares
  - Receita total acumulada
- CRUD completo de Voos
- CRUD completo de Hotéis
- Gestão de Reservas
- Visualização de Utilizadores

## Requisitos do Sistema

- .NET 8.0 SDK
- MySQL Server 8.0 ou superior
- Navegador web moderno

## Instalação e Configuração

### 1. Configurar a Base de Dados MySQL

Certifique-se de que o MySQL está instalado e em execução. Por padrão, o sistema usa:
- **Server:** localhost
- **Database:** aerotejo
- **User:** root
- **Password:** root
- **Port:** 3306

Para alterar estas configurações, edite o ficheiro `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=aerotejo;User=root;Password=root;Port=3306;"
  }
}
```

### 2. Restaurar Pacotes NuGet

```bash
dotnet restore
```

### 3. Criar a Base de Dados

O sistema cria automaticamente a base de dados e as tabelas na primeira execução através do método `EnsureCreated()`.

### 4. Popular com Dados Iniciais

O sistema inclui um seeder que popula automaticamente a base de dados com:
- Utilizador Administrador
- Utilizador Passageiro de teste
- Voos de exemplo
- Hotéis de exemplo
- Assentos para cada voo

### 5. Executar a Aplicação

```bash
dotnet run
```

A aplicação estará disponível em:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001

## Credenciais de Acesso

### Administrador
- **E-mail:** admin@aerotejo.pt
- **Password:** admin123

### Passageiro
- **E-mail:** joao.silva@email.pt
- **Password:** pass123

## Estrutura do Projeto

```
AeroTejo/
├── Controllers/          # Controllers MVC
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── HomeController.cs
│   ├── HotelController.cs
│   ├── ReservaController.cs
│   └── VooController.cs
├── Data/                 # Contexto da BD e Seeder
│   ├── AeroTejoContext.cs
│   └── DbSeeder.cs
├── Helpers/              # Classes auxiliares
│   └── PasswordHelper.cs
├── Models/               # Modelos de dados
│   ├── User.cs
│   ├── Voo.cs
│   ├── Hotel.cs
│   ├── Assento.cs
│   ├── Reserva.cs
│   └── Faturacao.cs
├── ViewModels/           # ViewModels
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── ReservaConfigViewModel.cs
│   └── CheckoutViewModel.cs
├── Views/                # Views Razor
│   ├── Account/
│   ├── Admin/
│   ├── Home/
│   ├── Hotel/
│   ├── Reserva/
│   ├── Voo/
│   └── Shared/
└── wwwroot/              # Ficheiros estáticos
    ├── css/
    │   └── aerotejo.css
    └── js/
        └── theme-toggle.js
```

## Fluxo de Utilizador (Funil de Vendas)

1. **Pesquisa:** Utilizador pesquisa um destino na página inicial
2. **Voos:** Sistema apresenta voos disponíveis para o destino
3. **Seleção de Voo:** Utilizador seleciona um voo
4. **Hotéis:** Sistema filtra e mostra hotéis na cidade de destino
5. **Seleção de Hotel:** Utilizador seleciona hotel (opcional)
6. **Login:** Sistema requer autenticação para continuar
7. **Configuração:** Utilizador seleciona assento e introduz dados de passageiros
8. **Checkout:** Utilizador introduz dados de pagamento e faturação
9. **Confirmação:** Sistema gera fatura em PDF e confirma reserva

## Segurança

- **Password Hashing:** Implementação manual com SHA256
- **Salt:** Cada password tem um salt único de 32 bytes
- **Sessões:** Gestão de sessões para autenticação
- **Validação:** Validação de dados no cliente e servidor
- **Proteção CSRF:** Tokens anti-falsificação em formulários

## Notas Importantes

- O sistema de pagamento é **simulado** para fins académicos
- Nenhum pagamento real é processado
- As faturas PDF são geradas mas não enviadas por e-mail (funcionalidade mock)
- O sistema usa `EnsureCreated()` em vez de Migrations para simplicidade

## Autor

Desenvolvido para o Projeto Módulo 5 - TGPSI
Sistema de Gestão de Agência de Viagens Aéreas

## Licença

Este projeto é académico e destina-se apenas a fins educacionais.
