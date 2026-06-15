using Microsoft.AspNetCore.Mvc;
using Vendinha.Core.Models;
using Vendinha.Core.Services;

namespace Vendinha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DividasController : ControllerBase
    {
        private readonly DividaService _dividaService;

        public DividasController(DividaService dividaService)
        {
            _dividaService = dividaService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var dividas = _dividaService.Listar();
            return Ok(dividas);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var divida = _dividaService.BuscarPorId(id);

            if (divida == null)
            {
                return NotFound(new
                {
                    message = "Dívida não encontrada."
                });
            }

            return Ok(divida);
        }

        [HttpPost]
        public IActionResult Criar([FromBody] Divida divida)
        {
            var sucesso = _dividaService.CriarDivida(
                divida,
                out var erros
            );

            if (!sucesso)
            {
                return BadRequest(erros);
            }

            return CreatedAtAction(nameof(BuscarPorId), new { id = divida.Id }, divida);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Divida divida)
        {
            if (id != divida.Id)
            {
                return BadRequest(new
                {
                    message = "ID da dívida divergente."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sucesso = _dividaService.Atualizar(divida);

            if (!sucesso)
            {
                return NotFound(new
                {
                    message = "Dívida não encontrada."
                });
            }

            return Ok(divida);
        }

        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var sucesso = _dividaService.Remover(id);

            if (!sucesso)
            {
                return NotFound(new
                {
                    message = "Dívida não encontrada para remoção."
                });
            }

            return Ok(new
            {
                message = "Dívida removida com sucesso."
            });
        }

        [HttpPut("{id}/pagar")]
        public IActionResult Pagar(int id)
        {
            var sucesso = _dividaService.PagarDivida(id);

            if (!sucesso)
            {
                return BadRequest(new
                {
                    message = "A dívida não existe ou já está paga."
                });
            }

            return Ok(new
            {
                message = "Dívida paga com sucesso pelo cliente."
            });
        }
    }
}