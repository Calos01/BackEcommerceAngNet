namespace BackEcommerceAngNet.Models
{
    public class Payment
    {
        public int Id { get; set; } 
        public User User { get; set;}=new();
        public PaymentMethod PaymentMethod { get; set; } = new();
        public double MontoTotal { get; set;}
        public double MontoDescuento { get; set;}
        public double PrecioPagar { get; set;}
        public double CostoEnvio { get;set;}
        public string CreatedAt { get; set;}=string.Empty;
    }
}
