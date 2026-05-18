# 🎮 API de Gerenciamento de Jogos (RAWG Integrated)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

Uma API RESTful robusta desenvolvida em **ASP.NET Core (C#)** para gerenciar um catálogo de videogames. O projeto implementa um sistema completo de operações CRUD (Criação, Leitura, Atualização e Exclusão) e destaca-se por garantir a integridade dos dados através de uma integração em tempo real com a [API oficial do RAWG](https://rawg.io/apidocs).

## 💡 Sobre o Projeto

O objetivo desta API é fornecer um backend seguro e eficiente para catalogação de jogos. Em vez de permitir o cadastro de dados genéricos, o sistema atua de forma inteligente: toda tentativa de registro passa por uma validação externa rigorosa. Isso demonstra a aplicação prática de consumo de APIs de terceiros, tratamento de respostas assíncronas e resiliência contra falhas de rede.

## 🚀 Tecnologias e Arquitetura

* **Linguagem & Framework:** C# 12 com ASP.NET Core Web API (.NET 8).
* **Programação Assíncrona:** Uso intensivo de `async/await` e `Task` para não bloquear a thread principal durante chamadas I/O (Banco de Dados e requisições Web).
* **Injeção de Dependência (DI):** Desacoplamento de código utilizando interfaces e serviços injetados nos controladores (ex: `_jogoService`).
* **Comunicação Web:** Utilização do `HttpClient` e manipulação de dados com `System.Text.Json`.
* **Tratamento de Exceções:** Retornos estruturados baseados nos padrões HTTP (400, 404, 500, 502), garantindo que o cliente sempre receba um JSON padronizado informando o tipo e a mensagem do erro.
* **Documentação:** Swagger (OpenAPI) configurado nativamente.

## 🛡️ Destaque: Validação e Blindagem Externa

O coração desta API é a sua segurança na entrada de dados. O endpoint de criação foi desenvolvido com os seguintes mecanismos:
1. **Sanitização de Input:** Validação de strings vazias, nulas e conversão de caracteres para formato URI (EscapeDataString).
2. **Match Exato (RAWG):** A API busca o título inserido nos servidores do RAWG, varre a lista de resultados e exige uma **combinação exata** (ignorando *case sensitive*). Jogos falsos ou erros de digitação (ex: "teste", "string") são sumariamente bloqueados.
3. **Tolerância a Falhas:** Caso os servidores da API externa enfrentem instabilidades ou fiquem offline, o sistema intercepta o erro (`!response.IsSuccessStatusCode`) e retorna um Status `502 Bad Gateway`, evitando que o processo principal falhe de forma silenciosa ou grave dados corrompidos.

---

## 📌 Documentação dos Endpoints

A rota principal da API é `/api/Jogos`. Abaixo está o resumo das operações disponíveis:

### 📥 1. Criar um Novo Jogo (POST)
**Rota:** `POST /api/Jogos`
* **Descrição:** Valida o nome na API RAWG. Se existir, cria o registro.
* **Corpo Esperado:** JSON com `nome`, `descricao`, `imagemUrl`, `avaliacao` (int) e `classificacao` (int).
* **Retornos:** * `201 Created` (Sucesso, retorna a URI do novo recurso).
  * `400 Bad Request` (Nome vazio ou jogo não encontrado no RAWG).
  * `502 Bad Gateway` (Falha na API externa).

### 🔍 2. Listar Jogos (GET)
**Rota:** `GET /api/Jogos`
* **Descrição:** Retorna a lista completa de todos os jogos cadastrados no banco de dados do sistema.
* **Retornos:** `200 OK`.

### 🎯 3. Buscar Jogo por ID (GET)
**Rota:** `GET /api/Jogos/{id}`
* **Descrição:** Busca os detalhes de um jogo específico pelo seu identificador único.
* **Retornos:** * `200 OK` (Jogo encontrado).
  * `404 Not Found` (ID inexistente).

### ✏️ 4. Atualizar Jogo (PUT)
**Rota:** `PUT /api/Jogos/{id}`
* **Descrição:** Atualiza os dados de um jogo já existente.
* **Retornos:** `200 OK` ou `204 No Content`.

### ❌ 5. Deletar Jogo (DELETE)
**Rota:** `DELETE /api/Jogos/{id}`
* **Descrição:** Remove um jogo do catálogo permanentemente.
* **Retornos:** `200 OK` ou `204 No Content`.

---

## 🛠️ Como Executar o Projeto Localmente

### Pré-requisitos
* **.NET 8 SDK** instalado.
* IDE recomendada: **Visual Studio 2022** ou **VS Code**.
* Chave gratuita de desenvolvedor da [API RAWG](https://rawg.io/apidocs).

### Passos

1. Clone o repositório:
   ```bash
   git clone [https://github.com/NicolasOlim/ApiRawg.git]
