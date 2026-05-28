using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;
using Vendinha.Core.Services;
using Vendinha.Core.Data;

namespace Vendinha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly VendinhaDbContext _context;
        private readonly ClienteService _clienteService;

        public ClientesController(VendinhaDbContext context, ClienteService clienteService)
        {
            _context = context;
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(string? nome, int pagina = 1)
        {
            var clientes = await _clienteService.ListarClientesOrdenadosPorDivida(nome, pagina);
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Dividas)
                .FirstOrDefaultAsync(c => c.Id == id);

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

                Idade = DateTime.Today.Year - cliente.DataNascimento.Year,

                cliente.Email,
                cliente.Dividas
            };

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sucesso = await _clienteService.CadastrarCliente(cliente);

            if (!sucesso)
            {
                return BadRequest(new
                {
                    message = "CPF inválido ou já cadastrado."
                });
            }

            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.Id) return BadRequest(new { message = "ID do cliente divergente." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound(new { message = "Cliente não encontrado para remoção." });

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cliente removido com sucesso." });
        }
    }
}
