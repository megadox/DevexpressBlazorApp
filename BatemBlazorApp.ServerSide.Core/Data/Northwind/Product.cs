using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BatemBlazorApp.Data.Northwind {
    [Guid("25717B5C-9DEB-4C21-BC1D-838ED44AEBBE")]
    public partial class Product {
        public Product() {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public bool InStock => !Discontinued;

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
