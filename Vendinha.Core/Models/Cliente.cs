using System.ComponentModel.DataAnnotations;

namespace Vendinha.Core.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11,
            MinimumLength = 11,
            ErrorMessage = "O CPF deve conter 11 caracteres")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        public DateTime DataNascimento { get; set; }

        [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
        public string? Email { get; set; }

        public List<Divida> Dividas { get; set; } = new();
    }
}
