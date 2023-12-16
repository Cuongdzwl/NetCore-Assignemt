namespace NetCore_Assignemt.Services.DTO
{
    public class BillDTO
    {
        public ICollection<CartDTO> Cart { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
