using Azure.Core;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services.DTO;
using sib_api_v3_sdk.Client;
using System.Collections.ObjectModel;
using System.Configuration;

namespace NetCore_Assignemt.Services
{
    public class VnPayServices : IVnPayServices
    {
        public static readonly Dictionary<string, string> RETURN_RESPONSE_DICTIONARY = new Dictionary<string, string>
        {
            { "00", "Transaction successful" },
            { "07", "Debit successful. Transaction suspected of fraud (related to fraud, suspicious transaction)." },
            { "09", "Transaction unsuccessful due to: Customer's card/account not registered for Internet Banking service at the bank." },
            { "10", "Transaction unsuccessful due to: Customer failed to authenticate card/account information more than 3 times." },
            { "11", "Transaction unsuccessful due to: Payment waiting period has expired. Please retry the transaction." },
            { "12", "Transaction unsuccessful due to: Customer's card/account is locked." },
            { "13", "Transaction unsuccessful due to: Incorrect transaction authentication password (OTP) entered by the customer. Please retry the transaction." },
            { "24", "Transaction unsuccessful due to: Customer canceled the transaction." },
            { "51", "Transaction unsuccessful due to: Insufficient funds in the customer's account to complete the transaction." },
            { "65", "Transaction unsuccessful due to: Customer's account has exceeded the daily transaction limit." },
            { "75", "Payment bank is under maintenance." },
            { "79", "Transaction unsuccessful due to: Customer entered the payment password incorrectly too many times. Please retry the transaction." },
            { "99", "Other errors (remaining errors not listed in the provided error code list)." },
        };

        public static readonly Dictionary<string, string> RETURN_TRANSACTION_DICTIONARY = new Dictionary<string, string>
        {
            { "00", "Transaction successful" },
            { "01", "Transaction incomplete" },
            { "02", "Transaction failed" },
            { "04", "Reversed transaction (Customer was debited by the bank, but the transaction was unsuccessful at VNPAY)" },
            { "05", "VNPAY is processing this transaction (Refund in progress)" },
            { "06", "VNPAY has sent a refund request to the bank (Refund in progress)" },
            { "07", "Transaction suspected of fraud" },
            { "09", "Refund rejected" },
        };

        private readonly ILogger<VnPayServices> _logger;

        public VnPayServices(AppDbContext context, ILogger<VnPayServices> logger)
        {
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

        public string InstantPaymentNotification(VnPayCallbackDTO callback, string raw, Order order)
        {
            string returnContent = string.Empty;
            if (raw.Length > 0)
            {
                bool checkSignature = CallBackValidate(callback, raw);
                if (checkSignature)
                {
                    if (order != null)
                    {
                        if (order.Total == callback.vnp_Amount)
                        {
                            if (order.Status == (int)OrderStatus.Pending)
                            {
                                returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature (Checksum failed)\"}";
                }
            }
            else
            {
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }
            return returnContent;
        }

        public bool CallBackValidate(VnPayCallbackDTO callback, string raw)
        {
            try
            {
                var configuration = Configuration();
                string vnp_HashSecret = configuration["VnPay:vnp_HashSecret"];
                VnPay vnpay = new VnPay();

                _logger.LogInformation(callback.ToString());

                return vnpay.ValidateSignature(callback.vnp_SecureHash, vnp_HashSecret, raw);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return false;
            }
        }
    }
}
