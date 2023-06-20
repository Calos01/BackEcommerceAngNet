using BackEcommerceAngNet.Models;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

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

        public Cart GetCart(int cartid)
        {
            Cart cart = new Cart();
            using (SqlConnection connection = new SqlConnection(bdconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
                string query = "SELECT * from CartItems WHERE CartId=" + cartid + ";";
                command.CommandText = query;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CartItems item = new CartItems();
                    item.Id = (int)reader["CartItemId"];
                    item.Producto = GetProduct((int)reader["ProductId"]);
                    cart.CartItems.Add(item);
                };
                reader.Close();

                query = "SELECT * from Carts WHERE CartId=" + cartid + ";";
                command.CommandText = query;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    cart.Id = (int)reader["CartId"];
                    cart.User = GetUser((int)reader["UserId"]);
                    cart.Ordered = bool.Parse((string)reader["Ordered"]);
                    cart.OrderedOn = (string)reader["OrderedOn"];
                };
                reader.Close();

                return cart;
            }
        }

        public Cart GetCartActivePorUser(int userid)
        {
            Cart cart = new Cart();
            using (SqlConnection connection = new SqlConnection(bdconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
                string query = "SELECT COUNT(*) from Carts WHERE UserId=" + userid + " AND Ordered='false';";
                command.CommandText = query;
                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    return new Cart();
                }

                query = "SELECT CartId from Carts WHERE UserId=" + userid + " AND Ordered='false';";
                command.CommandText = query;
                int cartid = (int)command.ExecuteScalar();

                query = "SELECT * FROM CartItems where CartId=" + cartid + ";";
                command.CommandText = query;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CartItems item = new CartItems();
                    item.Id = (int)reader["CartItemId"];
                    item.Producto = GetProduct((int)reader["ProductId"]);
                    cart.CartItems.Add(item);
                };
                cart.Id= cartid;
                cart.User = GetUser(userid);
                cart.Ordered = false;
                cart.OrderedOn = "";
                
                return cart;
            }
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

        public List<PaymentMethod> GetPaymentMethods(int payid)
        {
            var results = new List<PaymentMethod>();

            using (SqlConnection connection = new SqlConnection(bdconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM PaymentMethods WHERE PaymentMethodId=" + payid + ";";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PaymentMethod pay=new PaymentMethod();
                    pay.Tipo = (string)reader["Type"];
                    pay.Proveedor = (string)reader["Provider"];
                    pay.Disponible = bool.Parse((string)reader["Available"]);
                    results.Add(pay);
                }
            }
            return results;
        }

        public List<Cart> GetPreviousCart(int userid)
        {
            var carts = new List<Cart>();

            using (SqlConnection connection = new SqlConnection(bdconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT CartId FROM Carts WHERE UserId=" + userid + " AND Ordered='true'";
                command.CommandText = query;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var cartid = (int)reader["CartId"];
                    carts.Add(GetCart(cartid));
                }
            }
            return carts;
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

        public List<Review> GetReviews(int productid)
        {
            var reviews = new List<Review>();
            
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT * FROM Reviews WHERE ProductId=" + productid + ";";
                    command.CommandText = query;
                    
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var review = new Review()
                        {
                            Id = (int)reader["ReviewId"],
                            review = (string)reader["Review"],
                            cretedAt = (string)reader["CreatedAt"]
                        };
                        var userid = (int)reader["UserId"];
                        review.User = GetUser(userid);

                        var prodid = (int)reader["ProductId"];
                        review.Product = GetProduct(prodid);

                        reviews.Add(review);
                    }
                }
                return reviews;
            
            
        }

        public User GetUser(int id)
        {
            var user = new User();
            try
            {
                using (SqlConnection connection = new SqlConnection(bdconnection))
                {
                    SqlCommand command = new()
                    {
                        Connection = connection
                    };
                    string query = "SELECT * FROM Users where UserId=" + id + ";";
                    command.CommandText = query;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        user.UserId = (int)reader["UserId"];
                        user.FirstName = (string)reader["FirstName"];
                        user.LastName = (string)reader["LastName"];
                        user.Email = (string)reader["Email"];
                        user.Address = (string)reader["Address"];
                        user.Mobile = (string)reader["Mobile"];
                        user.Password = (string)reader["Password"];
                        user.CreatedAt = (string)reader["CreatedAt"];
                        user.ModifiedAt = (string)reader["ModifiedAt"];
                    };
                }
                return user;
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
                        command.Parameters.Add("@mo", System.Data.SqlDbType.NVarChar).Value = user.Mobile;
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

        public bool InsertItemCart(int useid, int productid)
        {
            using (SqlConnection connection = new SqlConnection(bdconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();
                string query = "SELECT COUNT(*) from Carts WHERE UserId=" + useid + " AND Ordered='false';";
                command.CommandText = query; 
                int count = (int)command.ExecuteScalar();

                if (count == 0)
                {
                    query = "INSERT INTO Carts(UserId,Ordered,OrderedOn) values("+ useid + ",'false','')";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                query = "SELECT CartId from Carts WHERE UserId=" + useid + " AND Ordered='false';";
                command.CommandText = query;
                int cartid = (int)command.ExecuteScalar();

                query = "INSERT INTO CartItems(CartId,ProductId) values(" + cartid + ","+productid+")";
                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
        }

        public void InsertReview(Review review)
        {
            using (SqlConnection connection = new SqlConnection(bdconnection))
            {
                SqlCommand command = new()
                {
                    Connection = connection
                };
                var query = "INSERT INTO Reviews(UserId,ProductId,Review,CreatedAt) values(@uid,@pid,@re,@cat)";
                command.CommandText = query;
                command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = review.User.UserId;
                command.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = review.Product.Id;
                command.Parameters.Add("@re", System.Data.SqlDbType.NVarChar).Value = review.review;
                command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = review.cretedAt;
                
                connection.Open();
                command.ExecuteNonQuery();
               
            }
        }

        public string UserExist(string email, string password)
        {
            User user = new();
           
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
                        return "";
                    }
                    else
                    {
                        query = "SELECT * FROM Users where Email='" + email + "' AND Password='" + password + "';";
                        command.CommandText = query;

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            user.UserId= (int)reader["UserId"];
                            user.FirstName = (string)reader["FirstName"];
                            user.LastName = (string)reader["LastName"];
                            user.Email = (string)reader["Email"];
                            user.Address = (string)reader["Address"];
                            user.Mobile = (string)reader["Mobile"];
                            user.Password = (string)reader["Password"];
                            user.CreatedAt = (string)reader["CreatedAt"];
                            user.ModifiedAt = (string)reader["ModifiedAt"];
                        };
                    }
                    //implementar jwt
                    string key = "8F9A02D3AB4C5E678EDC2F1BCEA97FEB710DFFA79C0B41E4DAE3A90F659DE72B";
                    string duration = "60";
                    var symetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(symetrickey, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                         new Claim("id",user.UserId.ToString()),
                         new Claim("firstname",user.FirstName),
                         new Claim("lasname",user.LastName),
                         new Claim("email",user.Email),
                         new Claim("address",user.Address),
                         new Claim("mobile",user.Mobile),
                         new Claim("password",user.Password),
                         new Claim("createdat",user.CreatedAt),
                         new Claim("modifiedat",user.ModifiedAt),

                    };
                    var token = new JwtSecurityToken(
                        issuer: "localhost",
                        audience: "localhost",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(Int32.Parse(duration)),
                        signingCredentials:credentials
                        );
                    //convertimos  a string el token
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
        }

    }
}
