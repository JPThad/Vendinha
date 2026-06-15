using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;
using Vendinha.Core.Data;
using System.ComponentModel.DataAnnotations;

namespace Vendinha.Core.Services
{
    public class ClienteService
    {
        private readonly VendinhaDbContext context;

        public ClienteService(VendinhaDbContext context)
        {
            this.context = context;
        }

        public bool Validar(Cliente cliente, out List<ValidationResult> erros)
        {
            var validation = new ValidationContext(cliente);

            erros = new List<ValidationResult>();

            Validator.TryValidateObject(
                cliente,
                validation,
                erros,
                true
            );

            return erros.Count == 0;
        }

        public List<object> ListarClientesOrdenadosPorDivida(string? nome, int pagina)
        {
            var query = context.Clientes
                .Include(c => c.Dividas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(c => c.NomeCompleto.Contains(nome));
            }

            return query
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
                .ToList<object>();
        }

        public bool CadastrarCliente(
            Cliente cliente,
            out List<ValidationResult> erros)
        {
            if (!Validar(cliente, out erros))
            {
                return false;
            }

            var cpfExiste = context.Clientes
                .Any(c => c.CPF == cliente.CPF);

            if (cpfExiste)
            {
                erros.Add(
                    new ValidationResult("CPF já cadastrado.")
                );

                return false;
            }

            context.Clientes.Add(cliente);
            context.SaveChanges();

            return true;
        }

        public Cliente? BuscarPorId(int id)
        {
            return context.Clientes
                .Include(c => c.Dividas)
                .FirstOrDefault(c => c.Id == id);
        }

        public bool Remover(int id)
        {
            var cliente = context.Clientes.Find(id);

            if (cliente == null)
            {
                return false;
            }

            context.Clientes.Remove(cliente);
            context.SaveChanges();

            return true;
        }

        public bool Atualizar(Cliente cliente)
        {
            var clienteExistente = context.Clientes
                .FirstOrDefault(c => c.Id == cliente.Id);

            if (clienteExistente == null)
            {
                return false;
            }

            clienteExistente.NomeCompleto = cliente.NomeCompleto;
            clienteExistente.CPF = cliente.CPF;
            clienteExistente.DataNascimento = cliente.DataNascimento;
            clienteExistente.Email = cliente.Email;

            context.SaveChanges();

            return true;
        }

        public static int CalcularIdade(DateTime dataNascimento)
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
