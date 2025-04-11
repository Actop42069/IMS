using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Query
{
    public class ListAllProductQuery : IRequest<ListAllProductResponse>
    {
        public int PageNumber { get; set; } = 1; //default
    }

    public class ListAllProductResponse
    {
        public List<ProductResponseDto> Products { get; set; }

        public ListAllProductResponse()
        {
            Products = new List<ProductResponseDto>();
        }
    }

    public class ProductResponseDto
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

    public class ListAllProductHandler : IRequestHandler<ListAllProductQuery, ListAllProductResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public ListAllProductHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<ListAllProductResponse> Handle (ListAllProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                const int pageSize = 500;
                var skip = (request.PageNumber - 1) * pageSize;
                var totalRecords = await _dbContext.Product.CountAsync(cancellationToken);

                var productData = await _dbContext.Product.AsNoTracking()
                                                            .Where(a => a.IsActive == true)
                                                            .OrderBy(a => a.ProductId)
                                                            .Skip(skip)
                                                            .Take(pageSize)
                                                            .Select(a => new ProductResponseDto
                                                            {
                                                                VendorId = a.VendorId,
                                                                CategoryId = a.CategoryId,
                                                                Name = a.Name,
                                                                ProductStatus = a.ProductStatus,
                                                                Description = a.Description,
                                                                SKU = a.SKU,
                                                                ProductType = a.ProductType,
                                                                Price = a.Price,
                                                                UnitCost = a.UnitCost,
                                                                Quantity = a.Quantity,
                                                                ImageUrl = a.ImageUrl
                                                            })
                                                            .ToListAsync(cancellationToken);

                return new ListAllProductResponse {Products = productData};
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
