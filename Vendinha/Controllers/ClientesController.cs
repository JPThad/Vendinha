using Microsoft.AspNetCore.Mvc;
using Vendinha.Core.Models;
using Vendinha.Core.Services;
using Vendinha.Core.Data;

namespace Vendinha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }
        [HttpGet]
        public IActionResult Listar(string? nome, int pagina = 1)
        {
            var clientes = _clienteService.ListarClientesOrdenadosPorDivida(nome, pagina);
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var cliente = _clienteService.BuscarPorId(id);

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente não encontrado." });
            }

            var resultado = new
            {
                cliente.Id,
                cliente.NomeCompleto,
                cliente.CPF,
                cliente.DataNascimento,

                Idade = ClienteService.CalcularIdade(cliente.DataNascimento),

                cliente.Email,

                Dividas = cliente.Dividas.Select(d => new
                {
                    d.Id,
                    d.Valor,
                    d.EstaPaga,
                    d.DataCriacao,
                    d.DataPagamento,
                    d.ClienteId
                })
            };

            return Ok(resultado);
        }

        [HttpPost]
        public IActionResult Criar([FromBody] Cliente cliente)
        {
            var sucesso = _clienteService.CadastrarCliente(
                cliente,
                out var erros
            );

            if (!sucesso)
            {
                return BadRequest(erros);
            }

            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest(new
                {
                    message = "ID do cliente divergente."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sucesso = _clienteService.Atualizar(cliente);

            if (!sucesso)
            {
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });
            }

            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var sucesso = _clienteService.Remover(id);

            if (!sucesso)
            {
                return NotFound(new
                {
                    message = "Cliente não encontrado para remoção."
                });
            }

            return Ok(new
            {
                message = "Cliente removido com sucesso."
            });
        }
    }
}
