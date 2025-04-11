using Application.Interface;
using Domain.Entities;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sale.Query
{
    public class GetSalesFromCategoryQuery : IRequest<List<GetSalesFromCategoryResponse>>
    {
        public int CategoryId { get; set; }
    }

    public class GetSalesFromCategoryResponse
    {
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
    }

    public class GetSalesFromCategoryHandler : IRequestHandler<GetSalesFromCategoryQuery, List<GetSalesFromCategoryResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetSalesFromCategoryHandler(IIMSDbContext iMSDbContext,
                                            IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<List<GetSalesFromCategoryResponse>> Handle (GetSalesFromCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var salesData = await _dbContext.Sales.AsNoTracking()
                                                        .Where(a => a.PaymentStatus == PaymentStatus.Completed)
                                                        .Join(
                                                               _dbContext.SaleProduct,
                                                               sale => sale.SalesId,
                                                               saleProduct => saleProduct.SalesId,
                                                               (sale, saleProduct) => new { sale, saleProduct }
                                                           )
                                                        .Join(
                                                            _dbContext.Product.Include(a => a.Category),
                                                            joined => joined.saleProduct.ProductId,
                                                            product => product.ProductId,
                                                            (joined, product) => new
                                                            {
                                                                product.Category.CategoryId,
                                                                product.Category.CategoryName,
                                                                product.ProductId,
                                                                ProductName = product.Name,
                                                                joined.saleProduct.Quantity,
                                                                Revenue = joined.saleProduct.Quantity * product.Price,
                                                                Cost = joined.saleProduct.Quantity * product.UnitCost
                                                            }
                                                        )
                                                        .Where(a => a.CategoryId == request.CategoryId)
                                                        .GroupBy(a => new
                                                        {
                                                            a.CategoryName,
                                                            a.ProductId,
                                                            a.ProductName
                                                        })
                                                        .Select(a => new GetSalesFromCategoryResponse
                                                        {
                                                            CategoryName = a.Key.CategoryName,
                                                            ProductId = a.Key.ProductId,
                                                            ProductName = a.Key.ProductName,
                                                            TotalSold = a.Sum(a => a.Quantity),
                                                            TotalCost = a.Sum(a => a.Cost),
                                                            TotalRevenue = a.Sum(a => a.Revenue),
                                                            Profit = a.Sum(a => a.Revenue - a.Cost)
                                                        })
                                                        .ToListAsync(cancellationToken);
                return salesData;
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }  
}
