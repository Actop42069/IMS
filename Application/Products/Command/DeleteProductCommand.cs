using Application.Interface;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Products.Command
{
    public class DeleteProductCommand : IRequest<DeleteProductResponse>
    {
        public int ProductId { get; set; }
        [JsonIgnore]
        public string LastUpdatedBy { get; set; } = "a";
    }

    public class DeleteProductResponse
    {
        public string Message { get; set; }
    }

    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public DeleteProductHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<DeleteProductResponse> Handle (DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var productData = await _dbContext.Product.FindAsync(request.ProductId, cancellationToken) ?? throw new Exception("Product not found !");
                productData.IsActive = !productData.IsActive;
                productData.LastUpdatedAt = DateTimeOffset.UtcNow;
                productData.LastUpdatedBy = request.LastUpdatedBy;

                return new DeleteProductResponse { Message = $"Product of Id: {request.ProductId} has been deleted."};
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
