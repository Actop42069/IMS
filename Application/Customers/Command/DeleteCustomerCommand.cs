using Application.Interface;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Customers.Command
{
    public class DeleteCustomerCommand : IRequest<DeleteCustomerResponse>
    {
        public int CustomerId { get; set; }
        [JsonIgnore]
        public string LastUpdatedBy { get; set; } = "a";
    }

    public class DeleteCustomerResponse
    {
        public string Message { get; set; }
    }

    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, DeleteCustomerResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public DeleteCustomerHandler(IIMSDbContext iMSDbContext,
                                     IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<DeleteCustomerResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerData = await _dbContext.Customer.FindAsync(request.CustomerId, cancellationToken)
                    ?? throw new Exception("Customer not found!");
                customerData.IsActive = !customerData.IsActive;
                customerData.LastUpdatedAt = DateTimeOffset.UtcNow;
                customerData.LastUpdatedBy = request.LastUpdatedBy;
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new DeleteCustomerResponse { Message = $"Customer with Id: {request.CustomerId} has been updated." };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
