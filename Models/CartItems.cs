namespace BackEcommerceAngNet.Models
{
    public class CartItems
    {
        public int Id { get; set; }
        public Product Producto { get; set;}=new Product();
    }
}
