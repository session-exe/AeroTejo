# Manual de Utilização - AeroTejo
## Sistema de Gestão de Agência de Viagens Aéreas

---

## Índice

1. [Introdução](#introdução)
2. [Requisitos do Sistema](#requisitos-do-sistema)
3. [Instalação e Configuração](#instalação-e-configuração)
4. [Como Registar-se no Sistema](#como-registar-se-no-sistema)
5. [Como Efetuar Login](#como-efetuar-login)
6. [Funcionalidades para Passageiros](#funcionalidades-para-passageiros)
7. [Funcionalidades para Administradores](#funcionalidades-para-administradores)
8. [Processos de Reserva e Gestão de Voos](#processos-de-reserva-e-gestão-de-voos)
9. [Alternância entre Modo Claro e Escuro](#alternância-entre-modo-claro-e-escuro)
10. [Tabela de Verificação de Requisitos](#tabela-de-verificação-de-requisitos)
11. [Resolução de Problemas](#resolução-de-problemas)
12. [Contactos e Suporte](#contactos-e-suporte)

---

## 1. Introdução

O **AeroTejo** é um sistema completo de gestão de agência de viagens aéreas desenvolvido em ASP.NET Core 8.0 MVC. O sistema permite aos utilizadores pesquisar voos, reservar alojamentos e gerir todas as suas viagens de forma integrada e segura.

### Principais Características

- **Interface Moderna:** Tema Roxo e Preto em Dark Mode com opção de Light Mode
- **Segurança:** Sistema de autenticação com password hashing e salt
- **Gestão Completa:** CRUD de voos, hotéis e reservas
- **Estatísticas:** Dashboard administrativo com gráficos interativos
- **Faturação:** Geração automática de faturas em PDF

---

## 2. Requisitos do Sistema

### Software Necessário

- **.NET 8.0 SDK** ou superior
- **MySQL Server 8.0** ou superior
- **Navegador Web Moderno** (Chrome, Firefox, Edge, Safari)

### Requisitos de Hardware Mínimos

- **Processador:** Dual-core 2.0 GHz
- **Memória RAM:** 4 GB
- **Espaço em Disco:** 500 MB livres
- **Resolução de Ecrã:** 1280x720 pixels

---

## 3. Instalação e Configuração

### Passo 1: Configurar o MySQL

1. Certifique-se de que o MySQL está instalado e em execução
2. Crie um utilizador com permissões adequadas (ou use o root)
3. O sistema criará automaticamente a base de dados `aerotejo` na primeira execução

### Passo 2: Configurar a Connection String

Edite o ficheiro `appsettings.json` se necessário:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=aerotejo;User=root;Password=root;Port=3306;"
  }
}
```

### Passo 3: Executar a Aplicação

Abra uma linha de comandos na pasta do projeto e execute:

```bash
dotnet run
```

A aplicação estará disponível em:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001

### Credenciais de Acesso Iniciais

**Administrador:**
- E-mail: `admin@aerotejo.pt`
- Password: `admin123`

**Passageiro de Teste:**
- E-mail: `joao.silva@email.pt`
- Password: `pass123`

---

## 4. Como Registar-se no Sistema

### Passo a Passo

1. **Aceda à Página Inicial**
   - Abra o navegador e vá para http://localhost:5000

2. **Clique em "Registo"**
   - No menu superior, clique no botão "Registo"

3. **Preencha o Formulário**
   - **Nome Completo:** Introduza o seu nome completo
   - **Idade:** Deve ter pelo menos 18 anos
   - **E-mail:** Introduza um e-mail válido (será usado para login)
   - **Telemóvel:** Número de contacto
   - **Password:** Mínimo 6 caracteres
   - **Confirmar Password:** Repita a password

4. **Submeta o Formulário**
   - Clique em "Criar Conta"
   - Será automaticamente atribuído o papel de "Passageiro"

5. **Faça Login**
   - Após o registo, será redirecionado para a página de login
   - Use o e-mail e password que acabou de criar

### Notas Importantes

- O e-mail deve ser único no sistema
- A password é armazenada de forma segura com hashing e salt
- Todos os novos utilizadores recebem automaticamente o papel de "Passageiro"

---

## 5. Como Efetuar Login

### Passo a Passo

1. **Aceda à Página de Login**
   - Clique em "Login" no menu superior

2. **Introduza as Credenciais**
   - **E-mail:** O e-mail usado no registo
   - **Password:** A sua password

3. **Clique em "Entrar"**
   - Se as credenciais estiverem corretas, será redirecionado para:
     - **Passageiros:** Página inicial
     - **Administradores:** Dashboard administrativo

### Recuperação de Acesso

Se esquecer a password, contacte o administrador do sistema para redefinição.

---

## 6. Funcionalidades para Passageiros

### 6.1 Pesquisar Voos

1. Na página inicial, introduza o destino desejado no campo de pesquisa
2. Clique em "Pesquisar"
3. Será apresentada uma lista de voos disponíveis para esse destino

### 6.2 Visualizar Catálogo de Voos

- Aceda ao menu "Voos" para ver todos os voos disponíveis
- Cada voo apresenta:
  - Companhia aérea
  - Origem e destino
  - Data e hora de partida
  - Preço
  - Número de lugares disponíveis

### 6.3 Consultar Hotéis

- Aceda ao menu "Hotéis" para ver alojamentos disponíveis
- Informações apresentadas:
  - Nome do hotel
  - Localização (cidade)
  - Preço por noite
  - Descrição

### 6.4 Fazer uma Reserva

#### Passo 1: Selecionar Voo
1. Pesquise ou navegue pelos voos disponíveis
2. Clique em "Selecionar Voo" no voo desejado

#### Passo 2: Selecionar Hotel (Opcional)
1. O sistema apresenta hotéis na cidade de destino do voo
2. Clique em "Selecionar Hotel" ou "Continuar sem Hotel"

#### Passo 3: Login (se necessário)
- Se não estiver autenticado, será redirecionado para a página de login

#### Passo 4: Configurar Reserva
1. **Selecionar Assento:** Escolha um assento disponível no dropdown
2. **Datas:** Introduza as datas de check-in e check-out
3. **Dados dos Passageiros:** Introduza os dados de todos os passageiros
   ```
   Nome: João Silva
   Passaporte: PT123456
   Data Nascimento: 01/01/1990
   ```
4. Clique em "Avançar para Checkout"

#### Passo 5: Checkout
1. **Dados de Pagamento (Mock):**
   - Número do cartão
   - Nome no cartão
   - Validade (MM/AA)
   - CVV

2. **Dados de Faturação:**
   - NIF
   - Morada
   - Nome para faturação

3. Clique em "Confirmar e Pagar"

#### Passo 6: Confirmação
- Será apresentada a confirmação da reserva
- Pode visualizar todos os detalhes
- A fatura está disponível para download

### 6.5 Ver Histórico de Reservas

1. Aceda ao menu "Minhas Reservas"
2. Visualize todas as suas reservas anteriores
3. Clique em "Ver Detalhes" para ver informações completas de cada reserva

---

## 7. Funcionalidades para Administradores

### 7.1 Aceder ao Painel Administrativo

1. Faça login com credenciais de administrador
2. Será automaticamente redirecionado para o Dashboard
3. Ou clique em "Dashboard" no menu superior

### 7.2 Dashboard com Estatísticas

O dashboard apresenta:

- **Estatísticas Rápidas:**
  - Total de voos
  - Total de hotéis
  - Total de reservas
  - Total de utilizadores
  - Receita total acumulada

- **Gráficos Interativos:**
  - Reservas e receita por mês (gráfico de barras)
  - Destinos mais populares (gráfico circular)

### 7.3 Gestão de Voos (CRUD)

#### Listar Voos
- Aceda a "Gestão de Voos" no menu
- Visualize todos os voos registados

#### Criar Novo Voo
1. Clique em "Novo Voo"
2. Preencha o formulário:
   - Companhia aérea
   - Origem
   - Destino
   - Data e hora
   - Preço
   - Total de lugares
3. Clique em "Guardar"
4. Os assentos são criados automaticamente

#### Editar Voo
1. Na lista de voos, clique no ícone de editar
2. Altere os campos desejados
3. Clique em "Atualizar"

#### Eliminar Voo
1. Na lista de voos, clique no ícone de eliminar
2. Confirme a eliminação

### 7.4 Gestão de Hotéis (CRUD)

#### Listar Hotéis
- Aceda a "Gestão de Hotéis" no menu
- Visualize todos os hotéis registados

#### Criar Novo Hotel
1. Clique em "Novo Hotel"
2. Preencha o formulário:
   - Nome do hotel
   - Cidade
   - Preço por noite
   - Descrição
3. Clique em "Guardar"

#### Editar Hotel
1. Na lista de hotéis, clique no ícone de editar
2. Altere os campos desejados
3. Clique em "Atualizar"

#### Eliminar Hotel
1. Na lista de hotéis, clique no ícone de eliminar
2. Confirme a eliminação

### 7.5 Gestão de Reservas

- Aceda a "Reservas" no menu
- Visualize todas as reservas do sistema
- Informações apresentadas:
  - ID da reserva
  - Nome do cliente
  - Voo (origem → destino)
  - Hotel (se aplicável)
  - Data da reserva
  - Valor total

### 7.6 Visualização de Utilizadores

- Aceda a "Utilizadores" no menu
- Visualize todos os utilizadores registados
- Informações apresentadas:
  - ID
  - Nome completo
  - E-mail
  - Telemóvel
  - Idade
  - Papel (Administrador/Passageiro)

---

## 8. Processos de Reserva e Gestão de Voos

### Fluxo Completo de Reserva (Funil de Vendas)

```
1. Pesquisa de Destino
   ↓
2. Listagem de Voos Disponíveis
   ↓
3. Seleção de Voo
   ↓
4. Sugestão de Hotéis no Destino
   ↓
5. Seleção de Hotel (opcional)
   ↓
6. Login/Autenticação
   ↓
7. Configuração da Reserva
   - Seleção de assento
   - Datas de check-in/out
   - Dados dos passageiros
   ↓
8. Checkout
   - Dados de pagamento
   - Dados de faturação
   ↓
9. Confirmação e Geração de Fatura
```

### Gestão de Voos pelo Administrador

```
1. Criar Voo
   ↓
2. Sistema Cria Assentos Automaticamente
   ↓
3. Voo Fica Disponível para Reserva
   ↓
4. Passageiros Fazem Reservas
   ↓
5. Assentos São Marcados como Ocupados
   ↓
6. Administrador Pode Editar/Eliminar Voo
```

### Transição de Dados

O sistema evidencia claramente a passagem de dados:

1. **View → Controller:**
   - Formulários HTML com `asp-action` e `asp-controller`
   - ViewModels para validação

2. **Controller → Base de Dados:**
   - Entity Framework Core
   - DbContext com SaveChangesAsync()

3. **Base de Dados → View:**
   - Queries com Include() para relacionamentos
   - ViewBag e Models para passar dados

---

## 9. Alternância entre Modo Claro e Escuro

### Como Alternar o Tema

1. Localize o botão circular no canto inferior direito da página
2. Clique no botão para alternar entre:
   - **Dark Mode:** Tema Roxo e Preto (padrão)
   - **Light Mode:** Tema claro

### Persistência da Preferência

- A preferência de tema é guardada no navegador
- Ao voltar ao site, o tema escolhido será mantido

---

## 10. Tabela de Verificação de Requisitos

| Requisito | Implementado | Observações |
|-----------|--------------|-------------|
| Sistema de registo de utilizadores | ✅ Sim | Formulário completo com validação |
| Atribuição automática do papel "Passageiro" no registo | ✅ Sim | Implementado no AccountController |
| Sistema de login com email e password | ✅ Sim | Com hashing manual e salt |
| Controlo de acessos baseado em roles | ✅ Sim | Sessões com verificação de papel |
| CRUD de voos (apenas administradores) | ✅ Sim | Create, Read, Update, Delete completo |
| Gestão de reservas (administradores) | ✅ Sim | Visualização de todas as reservas |
| Visualização de catálogo de voos (passageiros) | ✅ Sim | Lista com filtro por destino |
| Consulta de horários e destinos (passageiros) | ✅ Sim | Informações detalhadas de cada voo |
| Sistema de reservas de voos (passageiros) | ✅ Sim | Fluxo completo de reserva |
| Visualização de histórico de reservas pessoais (passageiros) | ✅ Sim | Página "Minhas Reservas" |
| CRUD de hotéis (administradores) | ✅ Sim | Create, Read, Update, Delete completo |
| Sugestão de hotéis baseada no destino do voo | ✅ Sim | Filtro automático por cidade |
| Seleção de assento via dropdown | ✅ Sim | Lista de assentos disponíveis |
| Formulário para dados de passageiros | ✅ Sim | Campo de texto para múltiplos passageiros |
| Checkout com dados de pagamento (mock) | ✅ Sim | Formulário completo simulado |
| Dados de faturação (NIF, Morada, Nome) | ✅ Sim | Guardados na base de dados |
| Geração de fatura em PDF | ✅ Sim | QuestPDF integrado |
| Dashboard administrativo com estatísticas | ✅ Sim | Estatísticas e gráficos Chart.js |
| Gráfico de vendas/reservas por mês | ✅ Sim | Gráfico de barras interativo |
| Gráfico de destinos mais populares | ✅ Sim | Gráfico circular |
| Receita total acumulada | ✅ Sim | Apresentada no dashboard |
| Header dinâmico por papel de utilizador | ✅ Sim | Visitante, Passageiro, Admin |
| Tema Roxo e Preto (Dark Mode) | ✅ Sim | Tema padrão |
| Modo Claro (Light Mode) | ✅ Sim | Botão de alternância |
| Password Hashing com Salt | ✅ Sim | Implementação manual SHA256 |
| MySQL como base de dados | ✅ Sim | Pomelo.EntityFrameworkCore.MySql |
| Bootstrap 5 | ✅ Sim | Framework CSS utilizado |
| ASP.NET Core 8.0 MVC | ✅ Sim | Framework base do projeto |
| Código comentado em Português | ✅ Sim | Comentários XML em todos os métodos |

---

## 11. Resolução de Problemas

### Problema: Não consigo fazer login

**Solução:**
- Verifique se o e-mail e password estão corretos
- O e-mail é case-sensitive
- Se for um utilizador novo, certifique-se de que completou o registo

### Problema: Erro de ligação à base de dados

**Solução:**
- Verifique se o MySQL está em execução
- Confirme as credenciais no `appsettings.json`
- Certifique-se de que a porta 3306 está disponível

### Problema: Não vejo voos disponíveis

**Solução:**
- Verifique se a base de dados foi populada com dados iniciais
- Execute o seeder novamente se necessário
- Verifique se os voos têm datas futuras

### Problema: O tema não muda

**Solução:**
- Limpe o cache do navegador
- Verifique se o JavaScript está ativado
- Tente outro navegador

### Problema: Erro ao criar voo

**Solução:**
- Verifique se todos os campos obrigatórios estão preenchidos
- A data deve ser futura
- O preço deve ser maior que zero
- O total de lugares deve ser entre 1 e 500

---

## 12. Contactos e Suporte

### Suporte Técnico

Para questões técnicas ou problemas com o sistema:
- **E-mail:** suporte@aerotejo.pt
- **Telefone:** +351 210 000 000

### Documentação Adicional

- **README.md:** Instruções técnicas de instalação
- **Código Fonte:** Comentado em Português (Portugal)

### Feedback

O seu feedback é importante para melhorar o sistema. Envie sugestões para:
- **E-mail:** feedback@aerotejo.pt

---

## Conclusão

O **AeroTejo** é um sistema completo e intuitivo para gestão de viagens aéreas. Este manual cobre todas as funcionalidades principais, mas não hesite em explorar o sistema para descobrir recursos adicionais.

**Boas viagens com o AeroTejo!** ✈️

---

**Versão do Manual:** 1.0  
**Data:** Janeiro 2026  
**Desenvolvido para:** Projeto Módulo 5 - TGPSI
