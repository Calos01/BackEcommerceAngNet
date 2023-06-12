namespace BackEcommerceAngNet.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public int Mobile { get; set; } = 0;
        public string Password { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string ModifiedAt { get; set; } = "";

    }
}
