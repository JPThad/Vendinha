using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;
using Vendinha.Core.Data;

namespace Vendinha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DividasController : ControllerBase
    {
        private readonly VendinhaDbContext _context;

        public DividasController(VendinhaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var dividas = await _context.Dividas.Include(d => d.Cliente).ToListAsync();
            return Ok(dividas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var divida = await _context.Dividas
                .Include(d => d.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (divida == null) return NotFound(new { message = "Dívida não encontrada." });

            return Ok(divida);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Divida divida)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Dividas.Add(divida);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarPorId), new { id = divida.Id }, divida);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Divida divida)
        {
            if (id != divida.Id) return BadRequest(new { message = "ID da dívida divergente." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(divida).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(divida);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            var divida = await _context.Dividas.FindAsync(id);
            if (divida == null) return NotFound(new { message = "Dívida não encontrada para remoção." });

            _context.Dividas.Remove(divida);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Dívida removida com sucesso." });
        }
    }
}
