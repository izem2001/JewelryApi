using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using JewelryApi.Models;

namespace JewelryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly List<Product> _products;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _products = GetSampleProducts();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            try
            {
                var goldPrice = await GetGoldPrice();
                var productsWithPrice = _products.Select(product =>
                {
                    var price = (product.PopularityScore + 1) * product.Weight * goldPrice;
                    var popularityOutOf5 = (product.PopularityScore * 5).ToString("0.0");

                    return new ProductResponse
                    {
                        Name = product.Name,
                        PopularityScore = product.PopularityScore,
                        Weight = product.Weight,
                        Images = product.Images,
                        Price = price.ToString("0.00"),
                        PopularityOutOf5 = popularityOutOf5,
                        GoldPrice = goldPrice.ToString("0.00")
                    };
                });

                return Ok(productsWithPrice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async  Task<ActionResult<IEnumerable<ProductResponse>>> GetFilteredProducts(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] decimal? minPopularity)
        {
            try
            {
                var goldPrice = await GetGoldPrice();
                var productsWithPrice = _products.Select(product =>
                {
                    var price = (product.PopularityScore + 1) * product.Weight * goldPrice;
                    var popularityOutOf5 = (product.PopularityScore * 5).ToString("0.0");

                    return new ProductResponse
                    {
                        Name = product.Name,
                        PopularityScore = product.PopularityScore,
                        Weight = product.Weight,
                        Images = product.Images,
                        Price = price.ToString("0.00"),
                        PopularityOutOf5 = popularityOutOf5,
                        GoldPrice = goldPrice.ToString("0.00")
                    };
                }).ToList();

                // Fiyat filtresi
                if (minPrice.HasValue)
                {
                    productsWithPrice = productsWithPrice
                        .Where(p => decimal.Parse(p.Price) >= minPrice.Value)
                        .ToList();
                }

                if (maxPrice.HasValue)
                {
                    productsWithPrice = productsWithPrice
                        .Where(p => decimal.Parse(p.Price) <= maxPrice.Value)
                        .ToList();
                }

                
                if (minPopularity.HasValue)
                {
                    productsWithPrice = productsWithPrice
                        .Where(p => decimal.Parse(p.PopularityOutOf5) >= minPopularity.Value)
                        .ToList();
                }

                return Ok(productsWithPrice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<decimal> GetGoldPrice()
        {
            try
            {
                
                var response = await _httpClient.GetAsync("https://api.metals.live/v1/spot/gold");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var goldData = JsonSerializer.Deserialize<List<GoldPriceResponse>>(content);
                    if (goldData != null && goldData.Count > 0)
                    {
                        // USD per troy ounce'u grama çevir (1 troy ounce = 31.1035 gram)
                        return goldData[0].Price / 31.1035m;
                    }
                }

                
                return 65.0m;
            }
            catch
            {
                return 65.0m; 
            }
        }

        private List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Name = "Engagement Ring 1",
                    PopularityScore = 0.85m,
                    Weight = 2.1m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG085-100P-Y.jpg?v=1696588368",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG085-100P-R.jpg?v=1696588406",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG085-100P-W.jpg?v=1696588402"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 2",
                    PopularityScore = 0.51m,
                    Weight = 3.4m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG012-Y.jpg?v=1707727068",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG012-R.jpg?v=1707727068",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG012-W.jpg?v=1707727068"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 3",
                    PopularityScore = 0.92m,
                    Weight = 3.8m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG020-100P-Y.jpg?v=1683534032",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG020-100P-R.jpg?v=1683534032",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG020-100P-W.jpg?v=1683534032"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 4",
                    PopularityScore = 0.88m,
                    Weight = 4.5m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG022-100P-Y.jpg?v=1683532153",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG022-100P-R.jpg?v=1683532153",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG022-100P-W.jpg?v=1683532153"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 5",
                    PopularityScore = 0.80m,
                    Weight = 2.5m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG074-100P-Y.jpg?v=1696232035",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG074-100P-R.jpg?v=1696927124",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG074-100P-W.jpg?v=1696927124"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 6",
                    PopularityScore = 0.82m,
                    Weight = 1.8m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG075-100P-Y.jpg?v=1696591786",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG075-100P-R.jpg?v=1696591802",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG075-100P-W.jpg?v=1696591798"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 7",
                    PopularityScore = 0.70m,
                    Weight = 5.2m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG094-100P-Y.jpg?v=1696589183",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG094-100P-R.jpg?v=1696589214",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG094-100P-W.jpg?v=1696589210"
                    }
                },
                new Product
                {
                    Name = "Engagement Ring 8",
                    PopularityScore = 0.90m,
                    Weight = 3.7m,
                    Images = new Dictionary<string, string>
                    {
                        ["yellow"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG115-100P-Y.jpg?v=1696596076",
                        ["rose"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG115-100P-R.jpg?v=1696596151",
                        ["white"] = "https://cdn.shopify.com/s/files/1/0484/1429/4167/files/EG115-100P-W.jpg?v=1696596147"
                    }
                }
            };
        }
    }
}