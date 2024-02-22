using Microsoft.AspNetCore.Mvc;
using PetsApi.Model;
using System.Data;
using Npgsql;


namespace PetsApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly string _conString = "Host=LocalHost;Username=postgres;Database=Kevin;Port=5432;Password=kevingomes123;SSLMode=Prefer";

        [HttpGet("Listar-animais")]
        public IActionResult ListarTodos()
        {
            List<Animal> retorno = new List<Animal>();

            using (var conexao = new NpgsqlConnection(_conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("SELECT * FROM pet.tabela_pet WHERE adotado = false", conexao))
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

        [HttpPost("Cadastrar-Animal")]
        public IActionResult Inserir([FromBody] Animal animal)
        {
            try
            {
                if (string.IsNullOrEmpty(animal.Nome) || string.IsNullOrEmpty(animal.Raca))
                {
                    return BadRequest("Nome e Raça são campos obrigatórios.");
                }

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
                return StatusCode(500, "Ocorreu um erro ao processar a solicitação.");
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

                    using (var comando = new NpgsqlCommand("UPDATE pet.tabela_pet SET nome = @nome, raca = @raca, adotado = @adotado WHERE id = @codigo", conexao))
                    {
                        comando.Parameters.AddWithValue("codigo", id);
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

                    using (var verificarComando = new NpgsqlCommand("SELECT COUNT(*) FROM pet.tabela_pet WHERE id = @id", conexao))
                    {
                        verificarComando.Parameters.AddWithValue("id", id);
                        int count = Convert.ToInt32(verificarComando.ExecuteScalar());

                        if (count == 0)
                        {
                            return NotFound(new {aviso = "ID não encontrado. Não foi possível excluir o animal." });
                        }
                    }
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

        [HttpGet("[action]")]
        public IActionResult ListarAdotados()
        {
            List<Animal> retorno = new List<Animal>();

            try
            {
                using (var conexao = new NpgsqlConnection(_conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("SELECT * FROM pet.tabela_pet WHERE adotado = true", conexao))
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
            catch (Exception ex)
            {
                return BadRequest(new { erro = true, mensagem = ex.Message });
            }
        }

        [HttpPatch("[action]")]
        public IActionResult AdotarAnimal(int id, bool adotado)
        {
            using (var connection = new NpgsqlConnection(_conString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("UPDATE pet.tabela_pet SET adotado = @adotado WHERE id = @id", connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("id", id);
                        command.Parameters.AddWithValue("adotado", adotado);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Cadastro modificado com sucesso!");

                        }
                        else
                        {
                            return BadRequest("Nenhuma cadastro encontrado com o ID especificado.");

                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                }
            }
        }
        [HttpPatch("[action]")]
        public IActionResult atualizarNome(int id, string nome)
        {
            using (var connection = new NpgsqlConnection(_conString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("UPDATE pet.tabela_pet SET nome = @nome WHERE id = @id", connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("id", id);
                        command.Parameters.AddWithValue("nome", nome);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Cadastro modificado com sucesso!");

                        }
                        else
                        {
                            return BadRequest("Nenhuma cadastro encontrado com o ID especificado.");

                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);

                    }
                }
            }
        }
        [HttpPatch("[action]")]
        public IActionResult atualizarRaça(int id, string raca)
        {
            using (var connection = new NpgsqlConnection(_conString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("UPDATE pet.tabela_pet SET raca = @raca WHERE id = @id", connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("id", id);
                        command.Parameters.AddWithValue("raca", raca);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Cadastro modificado com sucesso!");

                        }
                        else
                        {
                            return BadRequest("Nenhuma cadastro encontrado com o ID especificado.");

                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);

                    }
                }
            }
        }
    }
}