using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services.DTO;

namespace NetCore_Assignemt.Services
{
    public interface IPaymentServices
    {
        public string Pay(Order order, string? locale, string ipAddress, out bool successFlag);
        public bool CallBackValidate(VnPayCallbackDTO callback,string raw);
        public string InstantPaymentNotification(VnPayCallbackDTO callback, string raw, Order order);
    }
}
