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

                using (var comando = new NpgsqlCommand("SELECT * FROM public.tabela_pet", conexao))
                {
                    var leitor = comando.ExecuteReader();
                    while (leitor.Read())
                    {
                        var animal = new Animal()
                        {
                            Id = leitor.GetInt32(0),
                            Nome = leitor.GetString(1),
                            Raca = leitor["nome"].ToString(),
                        };
                        retorno.Add(animal);
                    }
                }
            }

            return Ok(retorno);
        }

        [HttpGet("{id}")]
        public IActionResult PesquisarPorCodigo(int id)
        {
            Animal animal = null;

            using (var conexao = new NpgsqlConnection(_conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand($"SELECT * FROM animal WHERE codigo = {id}", conexao))
                {
                    var leitor = comando.ExecuteReader();
                    while (leitor.Read())
                    {
                        animal = new Animal()
                        {
                            Id = leitor.GetInt32(0),
                            Raca = leitor.GetString(1),
                            Nome = leitor["nome"].ToString(),
                            adotado = leitor.GetBoolean(2),
                        };
                    }
                }
            }

            return Ok(animal);
        }

        [Route("raca")]
        [HttpGet]
        public IActionResult PesquisarPorRaca([FromQuery] string raca)
        {
            try
            {
                Animal animal = null;

                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand($"SELECT * FROM animal WHERE raca = '{raca}'", conexao))
                    {
                        var leitor = comando.ExecuteReader();
                        while (leitor.Read())
                        {
                            animal = new Animal()
                            {
                                Id = leitor.GetInt32(0),
                                Raca = leitor.GetString(1),
                                Nome = leitor["nome"].ToString(),
                            };
                        }
                    }
                }

                return Ok(animal);
            }
            catch (Exception ex)
            {
                
                return BadRequest($"Ocorreu uma falha: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Inserir([FromBody] Animal animal)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("INSERT INTO public.tabela_pet (raca, nome) VALUES (@raca, @nome)", conexao))
                    {
                        comando.Parameters.AddWithValue("raca", animal.Raca);
                        comando.Parameters.AddWithValue("nome", animal.Nome);

                        comando.ExecuteNonQuery();
                    }

                    using (var sqlCodigo = new NpgsqlCommand("SELECT max(codigo) FROM animal", conexao))
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
        public IActionResult Alterar([FromBody] Animal animal)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("UPDATE animal SET nome = @nome, raca = @raca WHERE codigo = @codigo", conexao))
                    {
                        comando.Parameters.AddWithValue("nome", animal.Nome);
                        comando.Parameters.AddWithValue("raca", animal.Raca);
                        comando.Parameters.AddWithValue("id", animal.Id);

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

                    using (var comando = new NpgsqlCommand("DELETE FROM animal WHERE codigo = @id", conexao))
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

