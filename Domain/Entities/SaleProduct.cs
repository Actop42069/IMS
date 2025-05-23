﻿namespace Domain.Entities
{
    public class SaleProduct
    {
        public int SalesId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Sales Sales { get; set; }
        public virtual Product Product { get; set; }
    }
}
