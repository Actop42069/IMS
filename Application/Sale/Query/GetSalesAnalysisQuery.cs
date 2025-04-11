using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sale.Query
{
    public class GetSalesAnalysisQuery : IRequest<List<SalesAnalysisResponse>>
    {
        public DateTimeOffset FilterDate { get; set; } = DateTimeOffset.Now;
        public TimePeriod Period { get; set; }
    }

    public class SalesAnalysisResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPercentage { get; set; }
    }

    public class GetSalesAnalysisHandler : IRequestHandler<GetSalesAnalysisQuery, List<SalesAnalysisResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetSalesAnalysisHandler(IIMSDbContext iMSDbContext,
                                     IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        private static (DateTimeOffset startDate, DateTimeOffset endDate) CalculateDateRange(DateTimeOffset date, TimePeriod period)
        {
            var startDate = date;
            var endDate = date;

            switch (period)
            {
                case TimePeriod.Day:
                    startDate = date.Date;
                    endDate = startDate.AddDays(1).AddTicks(-1);
                    break;
                case TimePeriod.Week:
                    startDate = date.AddDays(-(int)date.DayOfWeek).Date;
                    endDate = startDate.AddDays(7).AddTicks(-1);
                    break;
                case TimePeriod.Month:
                    startDate = new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, date.Offset);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
                case TimePeriod.Quarter:
                    var quarter = (date.Month - 1) / 3;
                    startDate = new DateTimeOffset(date.Year, quarter * 3 + 1, 1, 0, 0, 0, date.Offset);
                    endDate = startDate.AddMonths(3).AddTicks(-1);
                    break;
                case TimePeriod.Year:
                    startDate = new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, date.Offset);
                    endDate = startDate.AddYears(1).AddTicks(-1);
                    break;
            }

            return (startDate, endDate);
        }

        public async Task<List<SalesAnalysisResponse>> Handle(GetSalesAnalysisQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var dateRange = CalculateDateRange(request.FilterDate, request.Period);

                var salesData = await _dbContext.Sales
                    .AsNoTracking()
                    .Where(s => s.PaymentDate >= dateRange.startDate &&
                               s.PaymentDate <= dateRange.endDate &&
                               s.PaymentStatus == PaymentStatus.Completed)
                    .Join(
                        _dbContext.SaleProduct,
                        sale => sale.SalesId,
                        saleProduct => saleProduct.SalesId,
                        (sale, saleProduct) => new { sale, saleProduct }
                    )
                    .Join(
                        _dbContext.Product.Include(p => p.Category),
                        joined => joined.saleProduct.ProductId,
                        product => product.ProductId,
                        (joined, product) => new
                        {
                            product.CategoryId,
                            product.Category.CategoryName,
                            product.ProductId,
                            ProductName = product.Name,
                            joined.saleProduct.Quantity,
                            Revenue = joined.saleProduct.Quantity * product.Price,
                            Cost = joined.saleProduct.Quantity * product.UnitCost
                        }
                    )
                    .GroupBy(x => new
                    {
                        x.CategoryId,
                        x.CategoryName,
                        x.ProductId,
                        x.ProductName
                    })
                    .Select(g => new SalesAnalysisResponse
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        TotalQuantitySold = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Revenue),
                        TotalCost = g.Sum(x => x.Cost),
                        Profit = g.Sum(x => x.Revenue - x.Cost),
                        ProfitPercentage = g.Sum(x => x.Revenue) == 0 ? 0 :
                            ((g.Sum(x => x.Revenue - x.Cost) / g.Sum(x => x.Revenue)) * 100)
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