﻿using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public double Total { get; set; }

        [Timestamp]
        public byte[]? CreatedDate { get; set; }
    }
}