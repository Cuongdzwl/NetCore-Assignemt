using System.Web;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using SQLitePCL;

namespace NetCore_Assignemt.Common
{
    public class VnPay
    {
        public const string VERSION = "2.1.0";
        private SortedList<String, String> _requestData = new SortedList<String, String>(new VnPayCompare());
        private SortedList<String, String> _responseData = new SortedList<String, String>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            string retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }

        #region Request

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            String signData = queryString;
            if (signData.Length > 0)
            {

                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }



        #endregion

        #region Response process

        public bool ValidateSignature(string inputHash, string secretKey, string raw)
        {
            // Process raw
            raw = raw.Trim();
            raw = RemoveParameterFromQueryString(raw, "vnp_SecureHash");
            raw = RemoveParameterFromQueryString(raw, "vnp_SecureHashType");
            raw = raw.Substring(1); // remove first ?

            string myChecksum = Utils.HmacSHA512(secretKey, raw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        static string RemoveParameterFromQueryString(string queryString, string parameterName)
        {
            int index = queryString.IndexOf($"{parameterName}=");

            if (index != -1)
            {
                // Find the position of the next "&" after the parameter
                int endIndex = queryString.IndexOf('&', index);

                // Remove the parameter and its value from the string
                if (endIndex != -1)
                {
                    queryString = queryString.Remove(index, endIndex - index + 1);
                }
                else
                {
                    // If it's the last parameter, simply remove it
                    queryString = queryString.Substring(0, index).TrimEnd('?');
                }
                // Remove & 
                if (!string.IsNullOrEmpty(queryString) && queryString.EndsWith("&"))
                {
                    queryString = queryString.Substring(0, queryString.Length - 1);
                }

            }

            return queryString;
        }

        #endregion
    }

    public class Utils
    {


        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
