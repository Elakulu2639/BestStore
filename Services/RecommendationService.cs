using Microsoft.ML;
using Microsoft.ML.Data;
using BestStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Trainers;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestStore.Services
{
    public class RecommendationService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
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
                    return new List<Product>();
                }
            });
        }

        private List<Product> TrainAndGetRecommendations(int productId, string? userId, int count)
        {
            // Get order history data
            var query = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product.Id == productId));

            // Add user-specific filtering
            if (!string.IsNullOrEmpty(userId))
            {
                // Get orders for the current user
                var userOrders = _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .Where(o => o.ClientId == userId);

                // Combine product-specific orders and user-specific orders
                query = query.Union(userOrders);
            }

            var orders = query.ToList();

            // Prepare training data
            var trainingData = new List<ProductAssociation>();

            foreach (var order in orders)
            {
                var productIds = order.Items.Select(i => i.Product.Id).ToList();
                foreach (var pair in GenerateProductPairs(productIds))
                {
                    // Weight newer orders higher
                    var ageFactor = (DateTime.Now - order.CreatedAt).TotalDays;
                    var score = (float)Math.Max(0, 1.0 - ageFactor / 30); // Decay over 30 days

                    trainingData.Add(new ProductAssociation
                    {
                        ProductId = pair.Item1,
                        CoPurchaseProductId = pair.Item2,
                        Score = score
                    });
                }
            }

            if (!trainingData.Any())
            {
                _logger.LogWarning("No training data available for product {ProductId}", productId);
                return new List<Product>();
            }

            // Create ML pipeline
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

            // Train model
            _model = pipeline.Fit(dataView);

            // Generate predictions
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductAssociation, ProductAssociationPrediction>(_model);

            return _context.Products
                .Where(p => p.Id != productId)
                .AsEnumerable()
                .Select(p => new {
                    Product = p,
                    predictionEngine.Predict(new ProductAssociation
                    {
                        ProductId = productId,
                        CoPurchaseProductId = p.Id
                    }).Score
                })
                 .Where(p => p.Score > 0.5) // Only show relevant recommendations
                 .OrderByDescending(p => p.Score)
                 .Take(count)
                 .Select(p => p.Product)
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
    }

    public class ProductAssociationPrediction
    {
        public float Score { get; set; }
    }
}
