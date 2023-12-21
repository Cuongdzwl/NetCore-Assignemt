namespace NetCore_Assignemt.Services.DTO
{
    public class OrderDetailDTO
    {
        public BookDTO Book { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }    
    }
}
