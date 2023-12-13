using Azure.Core;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services.DTO;
using sib_api_v3_sdk.Client;
using System.Configuration;

namespace NetCore_Assignemt.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentServices> _logger;

        public PaymentServices(AppDbContext context, ILogger<PaymentServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static IConfigurationRoot Configuration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Set the base path to the directory containing your appsettings.json
            .AddJsonFile("appsettings.json")
            .Build();
        }

        public string Pay(Order order, string? locale, string ipAddress, out bool successFlag)
        {
            try
            {
                var configuration = Configuration();
                //Get Config Info
                string vnp_Returnurl = configuration["VnPay:vnp_Returnurl"];
                string vnp_Url = configuration["VnPay:vnp_Url"];
                string vnp_TmnCode = configuration["VnPay:vnp_TmnCode"];
                string vnp_HashSecret = configuration["VnPay:vnp_HashSecret"];

                if (vnp_Returnurl == null || vnp_Url == null || vnp_TmnCode == null || vnp_HashSecret == null)
                {
                    successFlag = false;
                    _logger.LogWarning("Missing VNPAY configuration!...");
                    return "";
                }
                //Build URL for VNPAY
                VnPay vnpay = new VnPay();

                vnpay.AddRequestData("vnp_Version", VnPay.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                // VnPay Money Ammout format (string) 10000000 = 100,000.00 (100k VND = 1000000)
                vnpay.AddRequestData("vnp_Amount", (order.Total * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", ipAddress);
                if (locale == null)
                {
                    vnpay.AddRequestData("vnp_Locale", "en");
                }
                else if (locale.ToLower() == "vn")
                {
                    vnpay.AddRequestData("vnp_Locale", "vn");
                }
                vnpay.AddRequestData("vnp_OrderInfo", "Billing order: " + order.Id);
                vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", order.Id.ToString()); // Reference Id of Order (Unique in day)
                                                                         //Add Params of 2.1.0 Version
                                                                         //Billing
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                successFlag = true;
                return paymentUrl;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                successFlag = false;
                return "";
            }
        }

        //public void InstantPaymentNotification() {
        //    string returnContent = string.Empty;
        //    if (w.QueryString.Count > 0)
        //    {
        //        string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret key
        //        var vnpayData = Request.QueryString;
        //        VnPayLibrary vnpay = new VnPayLibrary();
        //        foreach (string s in vnpayData)
        //        {
        //            //get all querystring data
        //            if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
        //            {
        //                vnpay.AddResponseData(s, vnpayData[s]);
        //            }
        //        }
        //        //Lay danh sach tham so tra ve tu VNPAY
        //        //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
        //        //vnp_TransactionNo: Ma GD tai he thong VNPAY
        //        //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
        //        //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

        //        long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
        //        long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
        //        long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        //        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //        string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
        //        String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
        //        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
        //        if (checkSignature)
        //        {
        //            //Cap nhat ket qua GD
        //            //Yeu cau: Truy van vao CSDL cua  Merchant => lay ra duoc OrderInfo
        //            //Giả sử OrderInfo lấy ra được như giả lập bên dưới

        //            order.OrderId = orderId;
        //            order.Amount = 100000;
        //            order.PaymentTranId = vnpayTranId;
        //            order.Status = "0"; //0: Cho thanh toan,1: da thanh toan,2: GD loi
        //            //Kiem tra tinh trang Order
        //            if (order != null)
        //            {
        //                if (order.Amount == vnp_Amount)
        //                {
        //                    if (order.Status == "0")
        //                    {
        //                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
        //                        {
        //                            //Thanh toan thanh cong
        //                            log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId,
        //                                vnpayTranId);
        //                            order.Status = "1";
        //                        }
        //                        else
        //                        {
        //                            //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
        //                            //  displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
        //                            log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}",
        //                                orderId,
        //                                vnpayTranId, vnp_ResponseCode);
        //                            order.Status = "2";
        //                        }

        //                        //Thêm code Thực hiện cập nhật vào Database 
        //                        //Update Database

        //                        returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
        //                    }
        //                    else
        //                    {
        //                        returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
        //                    }
        //                }
        //                else
        //                {
        //                    returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
        //                }
        //            }
        //            else
        //            {
        //                returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
        //            }
        //        }
        //        else
        //        {
        //            log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
        //            returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
        //        }
        //    }
        //    else
        //    {
        //        returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
        //    }


        //    Response.ClearContent();
        //    Response.Write(returnContent);
        //    Response.End();
        //}

        public bool CallBackValidate(VnPayCallbackDTO callback, string raw)
        {
            try
            {
                var configuration = Configuration();
                string vnp_HashSecret = configuration["VnPay:vnp_HashSecret"];
                VnPay vnpay = new VnPay();

                _logger.LogInformation(callback.ToString());
                var s = vnpay.ValidateSignature(callback.vnp_SecureHash, vnp_HashSecret, raw);
                return s;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
        }
    }
}
