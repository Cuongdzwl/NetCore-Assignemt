using System.ComponentModel;

namespace NetCore_Assignemt.Common
{
    public enum OrderStatus
    {
        Canceled = (int)-1,
        Pending = (int)0,
        Packaging = (int)1,
        Shipping = (int)2,
        Completed = (int)3,
    }

    public static class OrderStatusCommon
    {
        public static string? ToString(OrderStatus s)
        {
            if ((int)s == -1)
            {
                return "Canceled";
            }
            if ((int)s == 0)
            {
                return "Pending";
            }
            if ((int)s == 1)
            {
                return "Packaging";
            }
            if ((int)s == 2)
            {
                return "Shipping";
            }
            if ((int)s == 3)
            {
                return "Completed";
            }

            return null;
        }
    }
}
