using Application.Interface;
using Common.Helpers;
using Domain.Entities;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Sale.Command
{
    public class CreateSalesCommand : IRequest<CreateSalesResponse>
    {
        public int ProductId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderType PaymentStatus { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public string LastUpdatedBy { get; set; } 
    }

    public class CreateSalesResponse
    {
        public string Message { get; set; }
    }

    public class CreateSalesHandler : IRequestHandler<CreateSalesCommand, CreateSalesResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public CreateSalesHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<CreateSalesResponse> Handle(CreateSalesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var productData = await _dbContext.Product.FirstOrDefaultAsync(a => a.ProductId == request.ProductId, cancellationToken)
                                  ?? throw new Exception($"No product found with Id: {request.ProductId}");

                if (productData.Quantity < request.Quantity)
                {
                    throw new Exception($"Insufficient stock for product {request.ProductId}");
                }

                string transactionData = StringHelper.RandomString(10, includeCharacters: true, includeNumber: true, includeSymbols: false);
                var salesData = new Sales
                {
                    ProductId = request.ProductId,
                    PaymentDate = DateTimeOffset.UtcNow,
                    PaymentMethod = request.PaymentMethod,
                    TranscationId = transactionData,
                    PaymentStatus = request.PaymentStatus,
                    Quantity = request.Quantity,
                    IsActive = true,
                    LastUpdatedAt = DateTimeOffset.UtcNow,
                    LastUpdatedBy = request.LastUpdatedBy
                };
                _dbContext.Sales.Add(salesData);

                // Create SaleProduct record
                var saleProductData = new SaleProduct
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = productData.Price, 
                    Sales = salesData 
                };
                _dbContext.SaleProduct.Add(saleProductData);

                productData.Quantity -= request.Quantity; 
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new CreateSalesResponse { Message = $"Sale created successfully with TransactionId: {transactionData}" };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
