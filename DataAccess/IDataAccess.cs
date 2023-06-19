using BackEcommerceAngNet.Models;

namespace BackEcommerceAngNet.DataAccess
{
    public interface IDataAccess
    {
        List<ProductCategory> GetProductCategories();
        ProductCategory GetProductCategory(int id);
        Offer GetOffer(int id);
        List<Product> GetProductos(string category,string subcategory, int count);
        Product GetProduct(int id);
        Boolean InsertarUsuario(User user);
        String UserExist(string email, string password);
        void InsertReview(Review review);
        List<Review> GetReviews(int productid);
        User GetUser(int id);
        bool InsertItemCart(int useid, int productid);
    }
}
