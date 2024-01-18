namespace PetsApi.Model
{
    public class Animal
    {  
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public bool adotado { get; set; }
        //public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
