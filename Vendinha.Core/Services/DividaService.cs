using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;
using Vendinha.Core.Data;
using System.ComponentModel.DataAnnotations;

namespace Vendinha.Core.Services
{
    public class DividaService
    {
        private readonly VendinhaDbContext context;

        public DividaService(VendinhaDbContext context)
        {
            this.context = context;
        }

        public bool Validar(Divida divida, out List<ValidationResult> erros)
        {
            var validation = new ValidationContext(divida);

            erros = new List<ValidationResult>();

            Validator.TryValidateObject(
                divida,
                validation,
                erros,
                true
            );

            return erros.Count == 0;
        }
        public bool CriarDivida(
            Divida divida,
            out List<ValidationResult> erros)
        {
            if (!Validar(divida, out erros))
            {
                return false;
            }

            var possuiDividaAberta = context.Dividas
                .Any(d => d.ClienteId == divida.ClienteId && !d.EstaPaga);

            if (possuiDividaAberta)
            {
                erros.Add(
                    new ValidationResult(
                        "Cliente já possui dívida em aberto."
                    )
                );

                return false;
            }

            context.Dividas.Add(divida);
            context.SaveChanges();

            return true;
        }

        public List<object> Listar()
        {
            return context.Dividas
                .Include(d => d.Cliente)
                .Select(d => new
                {
                    d.Id,
                    d.Valor,
                    d.EstaPaga,
                    d.DataCriacao,
                    d.DataPagamento,
                    d.ClienteId,
                    Cliente = new
                    {
                        d.Cliente.Id,
                        d.Cliente.NomeCompleto,
                        d.Cliente.CPF
                    }
                })
                .ToList<object>();
        }

        public Divida? BuscarPorId(int id)
        {
            return context.Dividas
                .Include(d => d.Cliente)
                .FirstOrDefault(d => d.Id == id);
        }

        public bool Remover(int id)
        {
            var divida = context.Dividas.Find(id);

            if (divida == null)
            {
                return false;
            }

            context.Dividas.Remove(divida);
            context.SaveChanges();

            return true;
        }

        public bool Atualizar(Divida divida)
        {
            var existente = context.Dividas
                .FirstOrDefault(d => d.Id == divida.Id);

            if (existente == null)
            {
                return false;
            }

            existente.Valor = divida.Valor;
            existente.EstaPaga = divida.EstaPaga;
            existente.DataCriacao = divida.DataCriacao;
            existente.DataPagamento = divida.DataPagamento;
            existente.ClienteId = divida.ClienteId;

            context.SaveChanges();

            return true;
        }

        public bool PagarDivida(int dividaId)
        {
            var divida = context.Dividas.Find(dividaId);

            if (divida == null || divida.EstaPaga)
            {
                return false;
            }

            divida.EstaPaga = true;
            divida.DataPagamento = DateTime.UtcNow;

            context.SaveChanges();

            return true;
        }
    }
}
