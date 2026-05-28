using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;
using Vendinha.Core.Data;

namespace Vendinha.Core.Services
{
    public class DividaService
    {
        private readonly VendinhaDbContext context;

        public DividaService(VendinhaDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CriarDivida(Divida divida)
        {
            var possuiDividaAberta = await context.Dividas
                .AnyAsync(d => d.ClienteId == divida.ClienteId && !d.EstaPaga);

            if (possuiDividaAberta)
            {
                return false;
            }

            context.Dividas.Add(divida);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PagarDivida(int dividaId)
        {
            var divida = await context.Dividas.FindAsync(dividaId);

            if (divida == null || divida.EstaPaga)
            {
                return false;
            }

            divida.EstaPaga = true;
            divida.DataPagamento = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }
    }
}
