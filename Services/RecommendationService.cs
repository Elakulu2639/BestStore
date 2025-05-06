using System;
using System.Collections.Generic;
using System.Linq;
using BestStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace BestStore.Services
{
    public class RecommendationService
    {
        private readonly MLContext _mlContext;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<RecommendationService> logger)
        {
            _mlContext = new MLContext();
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public List<Product> GetFrequentlyBoughtTogether(int productId, string? userId = null, int count = 4)
        {
            var cacheKey = $"recs_{productId}_{userId ?? "global"}";

            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                try
                {
                    return TrainAndGetRecommendations(productId, userId, count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate recommendations for product {ProductId}", productId);
                    // Fallback to category-based recommendations
                    return GetCategoryBasedRecommendations(productId, count);
                }
            });
        }

        private List<Product> TrainAndGetRecommendations(int productId, string? userId, int count)
        {
            // Load orders containing the target product
            var productOrders = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product.Id == productId))
                .ToList();

            // If personalized, load user orders separately
            List<Order> userOrdersList = new();
            if (!string.IsNullOrEmpty(userId))
            {
                userOrdersList = _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .Where(o => o.ClientId == userId)
                    .ToList();
            }

            // Combine orders for per-order co-purchase pairs
            var combinedOrders = productOrders.Union(userOrdersList).ToList();

            var trainingData = new List<ProductAssociation>();

            // Generate per-order co-purchase pairs (with recency weight)
            foreach (var order in combinedOrders)
            {
                var productIds = order.Items.Select(i => i.Product.Id).Distinct().ToList();
                foreach (var pair in GenerateProductPairs(productIds))
                {
                    var ageDays = (DateTime.Now - order.CreatedAt).TotalDays;
                    var score = (float)Math.Max(0, 1.0 - ageDays / 90); // Increased to 90 days

                    trainingData.Add(new ProductAssociation
                    {
                        ProductId = pair.Item1,
                        CoPurchaseProductId = pair.Item2,
                        Score = score
                    });
                }
            }

            // For personalized recommendations, add cross-order pairs
            if (!string.IsNullOrEmpty(userId) && userOrdersList.Any())
            {
                var distinctUserProductIds = userOrdersList
                    .SelectMany(o => o.Items.Select(i => i.Product.Id))
                    .Distinct()
                    .ToList();

                if (distinctUserProductIds.Count > 1)
                {
                    foreach (var pair in GenerateProductPairs(distinctUserProductIds))
                    {
                        trainingData.Add(new ProductAssociation
                        {
                            ProductId = pair.Item1,
                            CoPurchaseProductId = pair.Item2,
                            Score = 1f
                        });
                    }
                }
            }

            // If no training data, return category-based recommendations
            if (!trainingData.Any())
            {
                _logger.LogWarning("No training data available for product {ProductId}", productId);
                return GetCategoryBasedRecommendations(productId, count);
            }

            // Cache the trained model
            var modelCacheKey = $"model_{productId}_{userId ?? "global"}";
            ITransformer model = _cache.GetOrCreate(modelCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

                // Build ML.NET matrix factorization pipeline
                var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                            outputColumnName: "ProductIdKey",
                            inputColumnName: nameof(ProductAssociation.ProductId))
                        .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                            outputColumnName: "CoPurchaseProductIdKey",
                            inputColumnName: nameof(ProductAssociation.CoPurchaseProductId)))
                        .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                            new MatrixFactorizationTrainer.Options
                            {
                                MatrixColumnIndexColumnName = "ProductIdKey",
                                MatrixRowIndexColumnName = "CoPurchaseProductIdKey",
                                LabelColumnName = nameof(ProductAssociation.Score),
                                NumberOfIterations = 20,
                                ApproximationRank = 100
                            }));

                return pipeline.Fit(dataView);
            });

            // Predict scores for other products
            var predictor = _mlContext.Model
                .CreatePredictionEngine<ProductAssociation, ProductAssociationPrediction>(model);

            var results = _context.Products
                .Where(p => p.Id != productId)
                .AsEnumerable()
                .Select(p => new
                {
                    Product = p,
                    Score = predictor.Predict(new ProductAssociation
                    {
                        ProductId = productId,
                        CoPurchaseProductId = p.Id
                    }).Score
                })
                .Where(x => x.Score > 0.1f) // Lowered threshold
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => x.Product)
                .ToList();

            // Fallback to category-based recommendations if results are insufficient
            if (results.Count < count)
            {
                var additional = GetCategoryBasedRecommendations(productId, count - results.Count);
                results.AddRange(additional);
                results = results.Take(count).ToList();
            }

            return results;
        }

        private List<Product> GetCategoryBasedRecommendations(int productId, int count)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return new List<Product>();
            }

            return _context.Products
                .Where(p => p.Category == product.Category && p.Id != productId)
                .OrderByDescending(p => p.Id) // Newest first
                .Take(count)
                .ToList();
        }

        private IEnumerable<Tuple<int, int>> GenerateProductPairs(List<int> productIds)
        {
            for (int i = 0; i < productIds.Count; i++)
            {
                for (int j = i + 1; j < productIds.Count; j++)
                {
                    yield return Tuple.Create(productIds[i], productIds[j]);
                    yield return Tuple.Create(productIds[j], productIds[i]);
                }
            }
        }

        public class ProductAssociationPrediction
        {
            public float Score { get; set; }
        }
    }
}