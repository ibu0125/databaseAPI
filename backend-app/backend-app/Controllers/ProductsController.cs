using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using backend_app.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using Org.BouncyCastle.Bcpg;
using ZstdSharp.Unsafe;

namespace backend_app.Controllers {
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductsController : ControllerBase {
        private readonly string connectionString = "Server=localhost;Database=ProductsDb;User=root;Password=Ibuki0606#;";
        [HttpPost("add")]
        public IActionResult AddProduct([FromBody] Product product) {
            using MySqlConnection sqlConnection = new MySqlConnection(connectionString);
            try {
                sqlConnection.Open();
                string insertQuery = "INSERT INTO Products(Name,Price)  VALUES(@Name,@Price)";
                using(MySqlCommand com = new MySqlCommand(insertQuery, sqlConnection)) {
                    com.Parameters.AddWithValue("@Name", product.Name);
                    com.Parameters.AddWithValue("@Price", product.Price);

                    com.ExecuteNonQuery();

                }
                return Ok(new
                {
                    message = "接続できました！"
                });
            }
            catch(Exception ex) {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }

        }
        [HttpGet("get")]
        public IActionResult getProducts() {
            List<Product> products = new List<Product>();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            try {
                connection.Open();
                string selectQuery = "SELECT * FROM Products";
                using MySqlCommand com = new MySqlCommand(selectQuery, connection);
                using MySqlDataReader reader = com.ExecuteReader();
                while(reader.Read()) {
                    var product = new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };

                    products.Add(product);
                }
                return Ok(products);
            }
            catch(Exception ex) {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }

        [HttpPut("updata/{id}")]
        public IActionResult putProduct(int id, [FromBody] Product product) {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            try {
                connection.Open();
                string updataQuery = "UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id";
                using(MySqlCommand com = new MySqlCommand(updataQuery, connection)) {
                    com.Parameters.AddWithValue("@Id", id);
                    com.Parameters.AddWithValue("@Name", product.Name);
                    com.Parameters.AddWithValue("@Price", product.Price);

                    int rowsAffected = com.ExecuteNonQuery();
                    if(rowsAffected > 0) {
                        return Ok(new
                        {
                            message = "商品が更新されました"
                        });
                    }
                    else {
                        return NotFound(new
                        {
                            message = "商品が見つかりませんでした"
                        });
                    }
                };




            }
            catch(Exception ex) {
                return StatusCode(500, new
                {
                    ex.Message
                });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult deleteProduct(int id) {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            try {
                connection.Open();
                string deleteQuery = "DELETE FROM Products WHERE id = @id";
                using(MySqlCommand com = new MySqlCommand(deleteQuery, connection)) {
                    com.Parameters.AddWithValue("@id", id);
                    int rowsAffected = com.ExecuteNonQuery();
                    if(rowsAffected > 0) {
                        return Ok(new
                        {
                            message = "削除出来ました"
                        });
                    }
                    else {
                        return NotFound(new
                        {
                            message = "商品が見つかりませんでした"
                        });
                    }

                };
            }
            catch(Exception ex) {
                Console.WriteLine($"Error deleting product with ID {id}: {ex.Message}");
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }




    }
}
