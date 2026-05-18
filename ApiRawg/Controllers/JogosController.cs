using ApiRawg.Models;
using ApiRawg.Service;
using Microsoft.AspNetCore.Mvc;

namespace ApiRawg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly JogoService _jogoService;

        public JogosController(JogoService jogoService)
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Retorna a lista completa de jogos cadastrados no banco de dados.
        /// </summary>
        /// <returns>Uma lista contendo todos os jogos.</returns>
        /// <response code="200">Retorna a lista de jogos com sucesso.</response>
        /// <response code="400">Se houver algum problema com os parâmetros de consulta.</response>
        /// <response code="404">Se a coleção de jogos não for encontrada.</response>
        /// <response code="500">Se ocorrer um erro interno no servidor (ex: falha no Firebase).</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var jogos = await _jogoService.Listar();
                return Ok(jogos);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Erro = "Dados de entrada inválidos",
                        Mensagem = ex.Message
                    });
                }
                else if (ex is KeyNotFoundException)
                {

                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        Erro = "Recurso não encontrado",
                        Mensagem = ex.Message
                    });

                }
                else
                {
                    Console.WriteLine($"Erro ao listar jogos: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Erro = "Falha Interna no Servidor",
                        Mensagem = ex.Message
                    });
                }
            }

        }

        /// <summary>
        /// Busca um jogo específico pelo seu ID.
        /// </summary>
        /// <param name="id">O identificador único do jogo (String).</param>
        /// <returns>Os detalhes do jogo correspondente ao ID informado.</returns>
        /// <response code="200">Retorna o jogo encontrado com sucesso.</response>
        /// <response code="400">Se o ID fornecido for nulo ou vazio.</response>
        /// <response code="404">Se nenhum jogo for encontrado com o ID especificado.</response>
        /// <response code="500">Se ocorrer um erro interno no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Erro = "ID do jogo é obrigatório",
                        Mensagem = "O ID do jogo não pode ser nulo ou vazio."
                    });
                }

                var jogo = await _jogoService.ObterPorId(id);
                if (jogo == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        Erro = "Jogo não encontrado",
                        Mensagem = $"Nenhum jogo encontrado com o ID: {id}"
                    });
                }
                return Ok(new { mensagem = "Localização encontrada: ", jogo });
            }
            catch (Exception ex)
            {

                if (ex is ArgumentNullException || ex is ArgumentException)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Erro = "Dados de entrada inválidos",
                        Mensagem = ex.Message
                    });
                }
                else if (ex is KeyNotFoundException)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        Erro = "Recurso não encontrado",
                        Mensagem = ex.Message
                    });
                }
                else
                {
                    Console.WriteLine($"Erro ao obter jogo por ID: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Erro = "Falha Interna no Servidor",
                        Mensagem = ex.Message
                    });

                }


            }

        }

        /// <summary>
        /// Cria e salva um novo jogo no banco de dados.
        /// </summary>
        /// <param name="jogo">O objeto JSON contendo os dados do jogo a ser criado.</param>
        /// <returns>O jogo recém-criado junto com a rota para acessá-lo.</returns>
        /// <response code="201">Retorna o jogo criado e o cabeçalho Location com a URI de acesso.</response>
        /// <response code="400">Se os dados enviados forem nulos ou o nome do jogo estiver vazio.</response>
        /// <response code="500">Se ocorrer um erro interno no servidor ao tentar salvar.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] Jogo jogo)
        {

            if (jogo == null)
            {
                return BadRequest(new
                {
                    Erro = "Dados de entrada inválidos",
                    Mensagem = "O corpo da requisição não pode ser nulo."
                });
            }

            if (string.IsNullOrWhiteSpace(jogo.Nome))
            {
                return BadRequest(new
                {
                    Erro = "Campo Obrigatório",
                    Mensagem = "Os campos Nome não pode ser vazio."
                });
            }

           
            var todosOsJogos = await _jogoService.Listar();

            
            bool jogoJaExiste = todosOsJogos.Any(j => j.Nome.Equals(jogo.Nome, StringComparison.OrdinalIgnoreCase));

            if (jogoJaExiste)
            {
            
                return Conflict(new
                {
                    Erro = "Jogo Duplicado",
                    Mensagem = $"O jogo '{jogo.Nome}' já está cadastrado no sistema!"
                });
            }

            try
            {

                var jogoCriado = await _jogoService.Criar(jogo);
                return CreatedAtAction(nameof(GetById), new { id = jogoCriado.Id }, jogoCriado);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Erro = "Dados de entrada inválidos",
                        Mensagem = ex.Message
                    });
                }
                else
                {
                    Console.WriteLine($"Erro ao criar jogo: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Erro = "Falha Interna no Servidor",
                        Mensagem = ex.Message
                    });
                }
            }
        }

        /// <summary>
        /// Atualiza integralmente os dados de um jogo existente.
        /// </summary>
        /// <param name="id">O ID do jogo que será atualizado.</param>
        /// <param name="jogo">O objeto JSON contendo os novos dados do jogo.</param>
        /// <returns>Uma mensagem de sucesso confirmando a atualização.</returns>
        /// <response code="200">Se o jogo for atualizado com sucesso.</response>
        /// <response code="400">Se o corpo da requisição for nulo ou inválido.</response>
        /// <response code="404">Se o jogo com o ID especificado não for encontrado.</response>
        /// <response code="500">Se ocorrer um erro interno no servidor ao tentar atualizar.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(string id, [FromBody] Jogo jogo)
        {
            if (jogo == null)
            {
                return BadRequest(new
                {
                    Erro = "Dados de entrada inválidos",
                    Mensagem = "O corpo da requisição não pode ser nulo."
                });
            }

            try
            {
                var existente = await _jogoService.ObterPorId(id);

                if (existente == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        Erro = "Jogo não encontrado",
                        Mensagem = $"Nenhum jogo encontrado com o ID: {id}"
                    });
                }

                existente.Nome = jogo.Nome;
                existente.Descricao = jogo.Descricao;
                existente.Avaliacao = jogo.Avaliacao;
                existente.Classificacao = jogo.Classificacao;
                existente.ImagemUrl = jogo.ImagemUrl;
                existente.Upload = jogo.Upload;

                await _jogoService.Atualizar(id, existente);
                return Ok(new { mensagem = $"Jogo Atualizado com Sucesso" });
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new
                    {
                        Erro = "Dados de entrada inválidos",
                        Mensagem = ex.Message
                    });
                }
                else
                {
                    Console.WriteLine($"Erro ao criar jogo: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Erro = "Falha Interna no Servidor",
                        Mensagem = ex.Message
                    });
                }
            }
        }

        /// <summary>
        /// Exclui um jogo do banco de dados.
        /// </summary>
        /// <param name="id">O ID do jogo que será excluído.</param>
        /// <returns>Uma mensagem de sucesso confirmando a exclusão.</returns>
        /// <response code="200">Se o jogo for excluído com sucesso.</response>
        /// <response code="404">Se o jogo com o ID especificado não for encontrado.</response>
        /// <response code="500">Se ocorrer um erro interno no servidor ao tentar excluir.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _jogoService.ObterPorId(id);
                if (existente == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        Erro = "Jogo não encontrado",
                        Mensagem = $"Nenhum jogo encontrado com o ID: {id}"
                    });
                }
                await _jogoService.Excluir(id);
                return Ok(new { mensagem = $"Jogo Deletado com Sucesso" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar jogo: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Erro = "Falha Interna no Servidor",
                    Mensagem = ex.Message
                });
            }

        }
    }

}