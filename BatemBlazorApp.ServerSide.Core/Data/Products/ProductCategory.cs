namespace BatemBlazorApp.Data
{
    public enum ProductCategoryMain { Bikes, Components, Clothing, Accessories }

    public class ProductCategory
    {
        public int SubcategoryID { get; set; }
        public ProductCategoryMain Category { get; set; }
        public string Subcategory { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CategorySubcategory => $"{Category}/{Subcategory}";
    }
}
