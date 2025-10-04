namespace JewelryApi.Models
{
    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal PopularityScore { get; set; }
        public decimal Weight { get; set; }
        public Dictionary<string, string> Images { get; set; } = new();
    }

    public class ProductResponse
    {
        public string Name { get; set; } = string.Empty;
        public decimal PopularityScore { get; set; }
        public decimal Weight { get; set; }
        public Dictionary<string, string> Images { get; set; } = new();
        public string Price { get; set; } = string.Empty;
        public string PopularityOutOf5 { get; set; } = string.Empty;
        public string GoldPrice { get; set; } = string.Empty;
    }

    public class GoldPriceResponse
    {
        public decimal Price { get; set; }
    }
}