using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;
using Vendinha.Core.Data;

namespace Vendinha.Core.Services
{
    public class ClienteService
    {
        private readonly VendinhaDbContext context;

        public ClienteService(VendinhaDbContext context)
        {
            this.context = context;
        }

        public async Task<List<object>> ListarClientesOrdenadosPorDivida(string? nome, int pagina)
        {
            var query = context.Clientes
                .Include(c => c.Dividas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(c => c.NomeCompleto.Contains(nome));
            }

            return await query
                .Select(c => new
                {
                    c.Id,
                    c.NomeCompleto,
                    c.CPF,
                    Idade = CalcularIdade(c.DataNascimento),

                    TotalDevido = c.Dividas
                        .Where(d => !d.EstaPaga)
                        .Sum(d => d.Valor),

                    PossuiPendencia = c.Dividas
                        .Any(d => !d.EstaPaga)
                })
                .OrderByDescending(c => c.TotalDevido)
                .Skip((pagina - 1) * 10)
                .Take(10)
                .ToListAsync<object>();
        }

        public async Task<bool> CadastrarCliente(Cliente cliente)
        {
            if (cliente.CPF.Length != 11)
            {
                return false;
            }

            var cpfExiste = await context.Clientes
                .AnyAsync(c => c.CPF == cliente.CPF);

            if (cpfExiste)
            {
                return false;
            }

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            return true;
        }

        private static int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade))
            {
                idade--;
            }

            return idade;
        }
    }
}
