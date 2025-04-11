using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sale.Query
{
    public class GetSalesFromVendorQuery : IRequest<List<GetSalesFromVendorResponse>>
    {
        public int VendorId { get; set; }
    }

    public class GetSalesFromVendorResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public int TotalRemaining { get; set; }
        public decimal TotalProfit { get; set; }
        public DateTimeOffset PurchaseDate { get; set; }
    }

    public class GetSalesFromVendorHandler : IRequestHandler<GetSalesFromVendorQuery, List<GetSalesFromVendorResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetSalesFromVendorHandler(IIMSDbContext iMSDbContext,
                                            IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<List<GetSalesFromVendorResponse>> Handle(GetSalesFromVendorQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var salesData = await _dbContext.Sales
                                                   .AsNoTracking()
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
                                                            Product = product,
                                                            joined.sale,
                                                            joined.saleProduct
                                                        }
                                                    )
                                                   .Where(a => a.Product.VendorId == request.VendorId)
                                                   .GroupBy(a => new
                                                   {
                                                       a.Product.ProductId,
                                                       a.Product.CategoryId,
                                                       a.Product.Category.CategoryName,
                                                       ProductName = a.Product.Name,
                                                       a.Product.Quantity,
                                                       PurchaseDate = a.Product.Category.LastUpdatedAt
                                                   }
                                                    )
                                                   .Select(a => new GetSalesFromVendorResponse
                                                   {
                                                       CategoryId = a.Key.CategoryId,
                                                       CategoryName = a.Key.CategoryName,
                                                       ProductId = a.Key.ProductId,
                                                       ProductName = a.Key.ProductName,
                                                       TotalSold = a.Sum(a => a.saleProduct.Quantity),
                                                       TotalRemaining = a.Key.Quantity,
                                                       TotalProfit = a.Sum(a => (a.saleProduct.UnitPrice - a.Product.UnitCost) * a.saleProduct.Quantity),
                                                       PurchaseDate = a.Key.PurchaseDate,
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
