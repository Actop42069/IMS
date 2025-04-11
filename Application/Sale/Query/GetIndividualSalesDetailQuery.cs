using Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sale.Query
{
    public class GetIndividualSalesDetailQuery : IRequest<GetIndividualSalesDetailResponse>
    {
        public int SalesId { get; set; }
    }

    public class GetIndividualSalesDetailResponse
    {
        public string Message { get; set; }
    }

    public class GetIndividualSalesDetailHandler : IRequestHandler<GetIndividualSalesDetailQuery, GetIndividualSalesDetailResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetIndividualSalesDetailHandler(IIMSDbContext iMSDbContext,
                                                IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<GetIndividualSalesDetailResponse> Handle(GetIndividualSalesDetailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"SELECT s.SalesId, 
                                   p.Name AS ProductName, 
                                   sp.Quantity, 
                                   p.Price, 
                                   p.UnitCost, 
                                   (p.Price - p.UnitCost) * sp.Quantity AS ProfitOrLoss
                            FROM Sales s
                            JOIN SaleProduct sp ON s.SalesId = sp.SalesId
                            JOIN Product p ON sp.ProductId = p.ProductId
                            WHERE s.SalesId = {0}";

                var saleDetails = await _dbContext.Database
                                                   .SqlQueryRaw<SaleDetailDto>(sql, request.SalesId)
                                                   .ToListAsync(cancellationToken);

                if (saleDetails.Count == 0)
                {
                    return new GetIndividualSalesDetailResponse
                    {
                        Message = "No sale details found for the provided SalesId."
                    };
                }

                var totalProfitOrLoss = saleDetails.Sum(sd => sd.ProfitOrLoss);
                var profitOrLossMessage = totalProfitOrLoss >= 0 ?
                                          $"Total Profit: {totalProfitOrLoss}" :
                                          $"Total Loss: {totalProfitOrLoss}";

                return new GetIndividualSalesDetailResponse
                {
                    Message = profitOrLossMessage
                };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }

    public class SaleDetailDto
    {
        public decimal ProfitOrLoss { get; set; }
    }
}
