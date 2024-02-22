using System.ComponentModel.DataAnnotations;

namespace PetsApi.Model
{
    public class Animal
    {
        [Required]

        [SwaggerSchema(Required = new[] { "Nome" })]

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public bool adotado { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
