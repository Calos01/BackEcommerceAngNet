namespace BackEcommerceAngNet.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public User User { get; set; }= new User();
        public List<CartItems> CartItems { get; set; }= new ();
        public bool Ordered { get; set; }
        public string OrderedOn = string.Empty;
    }
}
