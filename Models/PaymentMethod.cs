namespace BackEcommerceAngNet.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Tipo { get; set; }=string.Empty;
        public string Proveedor { get; set; } = string.Empty;   
        public bool Disponible { get; set; }
        public string Razon { get; set; } = string.Empty;
    }
}
