using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Query
{
    public class FilterProductQuery : IRequest<List<FilterProductResponse>>
    {
        public string SearchText { get; set; }
    }

    public class FilterProductResponse
    {
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

    public class FilterProductHandler : IRequestHandler<FilterProductQuery, List<FilterProductResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public FilterProductHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<List<FilterProductResponse>> Handle(FilterProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var searchText = request.SearchText?.Trim().ToLower() ?? string.Empty;

                var productData = await _dbContext.Product
                    .Where(a => a.IsActive == true &&
                                (string.IsNullOrEmpty(searchText) ||
                                 (a.Name != null && a.Name.ToLower().Contains(searchText)) ||
                                 (a.Description != null && a.Description.ToLower().Contains(searchText)) ||
                                 (a.SKU != null && a.SKU.ToLower().Contains(searchText))))
                    .AsNoTracking()
                    .Select(a => new FilterProductResponse
                    {
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