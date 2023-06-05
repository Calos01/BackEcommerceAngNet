using BackEcommerceAngNet.Models;

namespace BackEcommerceAngNet.DataAccess
{
    public interface IDataAccess
    {
        List<ProductCategory> GetProductCategories();
    }
}
