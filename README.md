# 🎮 API de Jogos com Integração RAWG

Uma API RESTful construída em **ASP.NET Core (C#)** para gerenciar um catálogo de jogos. O grande diferencial deste projeto é a sua integração rigorosa com a [API oficial do RAWG](https://rawg.io/apidocs), garantindo que apenas jogos reais sejam salvos no banco de dados.

## ✨ Funcionalidades

* **Cadastro Validador:** Ao tentar cadastrar um novo jogo, a API faz uma requisição HTTP em tempo real para os servidores do RAWG.
* **Verificação de Nome Exato:** O sistema varre os resultados da API externa e bloqueia tentativas de salvar jogos com nomes inventados (ex: "string" ou "teste").
* **Prevenção de Falhas Externas:** O código é blindado contra instabilidades do servidor do RAWG. Se a API oficial cair, o sistema retorna um erro amigável (Status 502 Bad Gateway) em vez de corromper o banco de dados.
* **Documentação Interativa:** Interface completa gerada via Swagger para testar os endpoints diretamente pelo navegador.

## 🚀 Tecnologias Utilizadas

* **C# / .NET** (ASP.NET Core Web API)
* **HttpClient** (Para requisições externas)
* **System.Text.Json** (Para manipulação e extração de dados JSON)
* **Swagger/OpenAPI** (Para documentação dos endpoints)

## 🛠️ Como executar o projeto

### Pré-requisitos
* SDK do .NET instalado na sua máquina.
* Uma chave de desenvolvedor (API Key) gratuita do site [RAWG](https://rawg.io/apidocs).

### Passo a Passo

1. **Clone este repositório:**
   ```bash
   git clone [https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git](https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git)
