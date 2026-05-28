using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vendinha.Core.Models
{
    public class Divida
    {
        public int Id { get; set; }

        [Required]
        public decimal Valor { get; set; }

        public bool EstaPaga { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataPagamento { get; set; }

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }
    }
}
