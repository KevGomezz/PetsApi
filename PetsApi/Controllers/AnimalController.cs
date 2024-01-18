using Microsoft.AspNetCore.Mvc;
using PetsApi.Model;
using System.Data;
using Npgsql;

namespace PetsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly string _conString = "Host=LocalHost;Username=postgres;Database=Kevin;Port=5432;Password=kevingomes123;SSLMode=Prefer";

        [HttpGet]
        public IActionResult ListarTodos()
        {
            List<Animal> retorno = new List<Animal>();

            using (var conexao = new NpgsqlConnection(_conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("SELECT * FROM pet.tabela_pet", conexao))
                {
                    var leitor = comando.ExecuteReader();
                    while (leitor.Read())
                    {
                        var animal = new Animal()
                        {
                            Id = leitor.GetInt32(0),
                            Nome = leitor.GetString(1),
                            Raca = leitor.GetString(2),
                            adotado = leitor.GetBoolean(3)
                        };
                        retorno.Add(animal);
                    }
                }
            }

            return Ok(retorno);
        }


        [HttpPost]
        public IActionResult Inserir([FromBody] Animal animal)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("INSERT INTO pet.tabela_pet (nome, raca, adotado) VALUES (@nome, @raca, @adotado)", conexao))
                    {
                        comando.Parameters.AddWithValue("nome", animal.Nome);
                        comando.Parameters.AddWithValue("raca", animal.Raca);
                        comando.Parameters.AddWithValue("adotado", animal.adotado);
                       

                        comando.ExecuteNonQuery();
                    }

                    using (var sqlCodigo = new NpgsqlCommand("SELECT max(id) FROM pet.tabela_pet", conexao))
                    {
                        var leitor = sqlCodigo.ExecuteReader();
                        while (leitor.Read())
                        {
                            animal.Id = leitor.GetInt32(0);
                        }
                    }
                }

                return Ok(animal);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = true, mensagem = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult Alterar(int id, [FromBody] Animal animal)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    // Correção na consulta SQL
                    using (var comando = new NpgsqlCommand("UPDATE pet.tabela_pet SET nome = @nome, raca = @raca, adotado = @adotado WHERE id = @codigo", conexao))
                    {
                        comando.Parameters.AddWithValue("codigo", id); // Adicionei o parâmetro para o ID
                        comando.Parameters.AddWithValue("nome", animal.Nome);
                        comando.Parameters.AddWithValue("raca", animal.Raca);
                        comando.Parameters.AddWithValue("adotado", animal.adotado);

                        comando.ExecuteNonQuery();
                    }
                }

                return Ok(animal);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = true, mensagem = ex.Message });
            }
        }
       

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("DELETE FROM pet.tabela_pet WHERE id = @id", conexao))
                    {
                        comando.Parameters.AddWithValue("id", id);
                        comando.ExecuteNonQuery();
                    }
                }

                return Ok("Animal excluído");
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = true, mensagem = ex.Message });
            }
        }
    }
}

