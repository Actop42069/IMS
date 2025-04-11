using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Query
{
    public class GetProductQuery : IRequest<GetProductResponse>
    {
        public int ProductId { get; set; }
    }

    public class GetProductResponse
    {
        public int VendorId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class GetProductHandler : IRequestHandler<GetProductQuery, GetProductResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetProductHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<GetProductResponse> Handle (GetProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var productData = await _dbContext.Product.Where(a => a.ProductId == request.ProductId)
                                                            .AsNoTracking()
                                                            .Select(a => new GetProductResponse
                                                            {
                                                                VendorId = a.VendorId,
                                                                CategoryId = a.CategoryId,
                                                                Name = a.Name,
                                                                Description = a.Description,
                                                                SKU = a.SKU,
                                                                ProductStatus = a.ProductStatus,
                                                                ProductType = a.ProductType,
                                                                Price = a.Price,
                                                                UnitCost = a.UnitCost,
                                                                Quantity = a.Quantity,
                                                                ImageUrl = a.ImageUrl
                                                            })
                                                            .FirstOrDefaultAsync(cancellationToken) ??
                                                            throw new Exception($"No product found of Id: {request.ProductId} ");
                return productData;
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
