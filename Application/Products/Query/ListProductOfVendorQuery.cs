using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Query
{
    public class ListProductOfVendorQuery : IRequest<ListProductOfVendorResponse>
    {
        public int VendorId { get; set; }
    }

    public class ListProductOfVendorResponse
    {
        public List<ProductDtos> Products { get; set; }
        public ListProductOfVendorResponse()
        {
            Products = new List<ProductDtos>();
        }
    }

    public class ProductDtos
    {
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

    public class ListProductOfVendorHandler : IRequestHandler<ListProductOfVendorQuery, ListProductOfVendorResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public ListProductOfVendorHandler(IIMSDbContext iMSDbContext,
                                            IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<ListProductOfVendorResponse> Handle(ListProductOfVendorQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var productData = await _dbContext.Product
                    .AsNoTracking()
                    .Where(a => a.IsActive == true && a.VendorId == request.VendorId)
                    .Select(a => new ProductDtos
                    {
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
                    .ToListAsync(cancellationToken);

                return new ListProductOfVendorResponse
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
