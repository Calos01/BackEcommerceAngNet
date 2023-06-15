using BackEcommerceAngNet.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BackEcommerceAngNet.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string bdconnection;
        private readonly string formatodate;
        public DataAccess(IConfiguration configuration)
        {
            this.configuration = configuration;
            //bdconnection = this.configuration["ConnectionStrings: Conexion"];
            bdconnection = "Server=(localdb)\\ServerSQL; Database=EcommerceAngNetBD; Trusted_Connection=True;";
            formatodate = this.configuration["Constants: FormatoDate"];
        }

        public Offer GetOffer(int id)
        {
            var offer = new Offer();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT * FROM Offers where OfferId="+id+";";
                    command.CommandText = query;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        offer.Id = (int)reader["OfferId"];
                        offer.Title = (string)reader["Title"];
                        offer.Discount = (int)reader["Discount"];                        
                    };
                }
                return offer;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public Product GetProduct(int id)
        {
            var producto = new Product();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT * FROM Products where ProductId=" + id + ";";
                    command.CommandText = query;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        producto.Id = (int)reader["ProductId"];
                        producto.Title = (string)reader["Title"];
                        producto.Description = (string)reader["Description"];

                        var categoryid= (int)reader["CategoryId"];
                        producto.ProductCategory = this.GetProductCategory(categoryid);

                        var offerid = (int)reader["OfferId"];
                        producto.Offer= this.GetOffer(offerid);

                        producto.Price = (double)reader["Price"];
                        producto.Quantity = (int)reader["Quantity"];
                        producto.ImageName = (string)reader["ImageName"];
                    };
                }
                return producto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProductCategory> GetProductCategories()
        {
            var productcategory=new List<ProductCategory>();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT * FROM ProductCategories;";
                    command.CommandText = query;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var category = new ProductCategory()
                        {
                            Id = (int)reader["CategoryId"],
                            Category = (string)reader["Category"],
                            SubCategory = (string)reader["SubCategory"]
                        };
                        productcategory.Add(category);
                    }
                }
                return productcategory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ProductCategory GetProductCategory(int id)
        {
            var prodcategory = new ProductCategory();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT * FROM ProductCategories where CategoryId=" + id + ";";
                    command.CommandText = query;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        prodcategory.Id = (int)reader["CategoryId"];
                        prodcategory.Category = (string)reader["Category"];
                        prodcategory.SubCategory = (string)reader["SubCategory"];
                    };
                }
                return prodcategory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Product> GetProductos(string category, string subcategory, int count)
        {
            var productos = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT TOP "+ count +" * FROM Products where CategoryId=(SELECT CategoryId FROM ProductCategories where Category=@c AND SubCategory=@s) ORDER BY newid();";
                    command.CommandText = query;
                    command.Parameters.Add("@c", System.Data.SqlDbType.NVarChar).Value = category;
                    command.Parameters.Add("@s", System.Data.SqlDbType.NVarChar).Value = subcategory;
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var producto = new Product()
                        {
                            Id = (int)reader["ProductId"],
                            Title = (string)reader["Title"],
                            Description = (string)reader["Description"],
                            Price = (double)reader["Price"],
                            Quantity = (int)reader["Quantity"],
                            ImageName = (string)reader["ImageName"]
                        };
                        var categoryid= (int)reader["CategoryId"];
                        producto.ProductCategory= GetProductCategory(categoryid);

                        var offerid = (int)reader["OfferId"];
                        producto.Offer= GetOffer(offerid);  

                        productos.Add(producto);
                    }
                }
                return productos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InsertarUsuario(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT COUNT(*) from Users WHERE Email='"+user.Email+"';";
                    command.CommandText = query;
                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        connection.Close();
                        return false;
                    }
                    else
                    {
                        query = "INSERT INTO Users(FirstName,LastName,Email,Address,Mobile,Password,CreatedAt,ModifiedAt) values(@fn,@ln,@em,@ad,@mo,@pa,@ca,@ma)";
                        command.CommandText = query;
                        command.Parameters.Add("@fn", System.Data.SqlDbType.NVarChar).Value = user.FirstName;
                        command.Parameters.Add("@ln", System.Data.SqlDbType.NVarChar).Value = user.LastName;
                        command.Parameters.Add("@em", System.Data.SqlDbType.NVarChar).Value = user.Email;
                        command.Parameters.Add("@ad", System.Data.SqlDbType.NVarChar).Value = user.Address;
                        command.Parameters.Add("@mo", System.Data.SqlDbType.Int).Value = user.Mobile;
                        command.Parameters.Add("@pa", System.Data.SqlDbType.NVarChar).Value = user.Password;
                        command.Parameters.Add("@ca", System.Data.SqlDbType.NVarChar).Value = user.CreatedAt;
                        command.Parameters.Add("@ma", System.Data.SqlDbType.NVarChar).Value = user.ModifiedAt;
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UserExist(string email, string password)
        {
            User user = new();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT COUNT(*) from Users WHERE Email='" + email+"' AND Password='"+password+"';";
                    command.CommandText = query;
                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    if (count == 0)
                    {
                        connection.Close();
                    }
                    else
                    {
                        query = "SELECT * FROM User where Email='" + email + "' AND Password='" + password + "';";
                        command.CommandText = query;

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            user.UserId= (int)reader["UserId"];
                            user.FirstName = (string)reader["FirstName"];
                            user.LastName = (string)reader["LastName"];
                            user.Email = (string)reader["Email"];
                            user.Address = (string)reader["Address"];
                            user.Mobile = (int)reader["Mobile"];
                            user.Password = (string)reader["Password"];
                            user.CreatedAt = (string)reader["CreatedAt"];
                            user.ModifiedAt = (string)reader["ModifiedAt"];
                        };
                    }
                    //implementar jwt
                    string key = "9yW%h5#pMv64zAV69#lE";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
