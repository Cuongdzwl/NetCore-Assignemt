using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IPaymentServices
    {
        public string Pay(Order order, string? locale, string ipAddress, out bool successFlag);
    }
}
