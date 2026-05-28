using Microsoft.EntityFrameworkCore;
using Vendinha.Core.Models;

namespace Vendinha.Core.Data
{
    public class VendinhaDbContext : DbContext
    {
        public VendinhaDbContext(DbContextOptions<VendinhaDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Divida> Dividas => Set<Divida>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var modelCliente = modelBuilder.Entity<Cliente>();
            modelCliente.ToTable("clientes");
            modelCliente.Property(c => c.Id).HasColumnName("id");
            modelCliente.Property(c => c.NomeCompleto).HasColumnName("nome_completo");
            modelCliente.Property(c => c.CPF).HasColumnName("cpf");
            modelCliente.Property(c => c.DataNascimento).HasColumnName("data_nascimento");
            modelCliente.Property(c => c.Email).HasColumnName("email");
            modelCliente.HasKey(c => c.Id);

            var modelDivida = modelBuilder.Entity<Divida>();
            modelDivida.ToTable("dividas");
            modelDivida.Property(d => d.Id).HasColumnName("id");
            modelDivida.Property(d => d.Valor).HasColumnName("valor");
            modelDivida.Property(d => d.EstaPaga).HasColumnName("esta_paga");
            modelDivida.Property(d => d.DataCriacao).HasColumnName("data_criacao");
            modelDivida.Property(d => d.DataPagamento).HasColumnName("data_pagamento");
            modelDivida.Property(d => d.ClienteId).HasColumnName("cliente_id");
            modelDivida.HasOne(d => d.Cliente)
                .WithMany(c => c.Dividas)
                .HasForeignKey(d => d.ClienteId);
            modelDivida.HasKey(d => d.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}