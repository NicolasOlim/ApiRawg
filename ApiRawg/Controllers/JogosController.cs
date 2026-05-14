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
                return Ok(new {mensagem = $"Jogo Atualizado com Sucesso"});
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
    
    }

}
