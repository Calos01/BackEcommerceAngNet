namespace BackEcommerceAngNet.Models
{
    public class Review
    {
        public int Id { get; set; }
        public User User{ get; set; } =new User();
        public Product Product { get; set;}=new Product();
        public string review { get; set; } = String.Empty;
        public string cretedAt { get; set; }= String.Empty;
    }
}
