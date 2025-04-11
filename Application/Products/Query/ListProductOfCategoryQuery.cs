using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Query
{
    public class ListProductOfCategoryQuery : IRequest<ListProductOfCategoryResponse>
    {
        public int CategoryId { get; set; }
    }

    public class ListProductOfCategoryResponse
    {
        public List<ProductDto> Products { get; set; }
        public ListProductOfCategoryResponse()
        {
            Products = new List<ProductDto>();
        }
    }

    public class ProductDto
    {
        public int VendorId { get; set; }
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

    public class ListProductOfCategoryHandler : IRequestHandler<ListProductOfCategoryQuery, ListProductOfCategoryResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public ListProductOfCategoryHandler(IIMSDbContext iMSDbContext,
                                            IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<ListProductOfCategoryResponse> Handle (ListProductOfCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var productData = await _dbContext.Product
                    .AsNoTracking()
                    .Where(a => a.IsActive == true && a.CategoryId == request.CategoryId)
                    .Select(a => new ProductDto
                    {
                        VendorId = a.VendorId,
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
                    .ToListAsync(cancellationToken);

                return new ListProductOfCategoryResponse
                {
                    Products = productData
                };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
