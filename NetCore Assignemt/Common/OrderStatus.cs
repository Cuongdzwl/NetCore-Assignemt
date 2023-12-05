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
}
