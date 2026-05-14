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
     //   public async Task<IActionResult> GetById(string id)
        {
            

        }

        }
}
