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
    }
}
