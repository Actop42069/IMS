using Application.Interface;
using MediatR;
using Domain.Enumeration;
using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Query
{
    public class GetCustomerQuery : IRequest<GetCustomerResponse>
    {
        public int CustomerId { get; set; }
    }

    public class GetCustomerResponse
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class GetCustomerHandler : IRequestHandler<GetCustomerQuery, GetCustomerResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetCustomerHandler(IIMSDbContext dbContext,
                                  IErrorLogService errorLogService)
        {
            _dbContext = dbContext;
            _errorLogService = errorLogService;
        }

        public async Task<GetCustomerResponse> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customerData = await _dbContext.Customer
                    .Where(c => c.CustomerId == request.CustomerId)
                    .AsNoTracking()  
                    .Select(c => new GetCustomerResponse
                    {
                        CustomerId = c.CustomerId,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber,
                        CustomerType = c.CustomerType,
                        IsActive = c.IsActive,
                        LastUpdatedAt = c.LastUpdatedAt,
                        LastUpdatedBy = c.LastUpdatedBy,
                        ImageUrl = c.ImageUrl
                    })
                    .FirstOrDefaultAsync(cancellationToken) ??
                    throw new Exception($"No customer found with Id: {request.CustomerId}");

                return customerData;
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
