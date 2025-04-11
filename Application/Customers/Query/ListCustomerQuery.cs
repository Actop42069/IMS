using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Customers.Query
{
    public class ListCustomerQuery : IRequest<ListCustomerResponse>
    {
        public int PageNumber { get; set; } = 1; // Default data
    }

    public class ListCustomerResponse
    {
        public List<CustomerResponseDto> Customers { get; set; }

        public ListCustomerResponse()
        {
            Customers = new List<CustomerResponseDto>();
        }
    }

    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ListCustomerHandler : IRequestHandler<ListCustomerQuery, ListCustomerResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public ListCustomerHandler(IIMSDbContext dbContext,
                                   IErrorLogService errorLogService)
        {
            _dbContext = dbContext;
            _errorLogService = errorLogService;
        }

        public async Task<ListCustomerResponse> Handle(ListCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                const int pageSize = 500;
                var skip = (request.PageNumber - 1) * pageSize;
                var totalRecords = await _dbContext.Customer.CountAsync(cancellationToken);

                var customerData = await _dbContext.Customer.AsNoTracking()
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.CustomerId)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(c => new CustomerResponseDto
                    {
                        CustomerId = c.CustomerId,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber,
                        CustomerType = c.CustomerType,
                        IsActive = c.IsActive,
                        LastUpdatedAt = c.LastUpdatedAt
                    })
                    .ToListAsync(cancellationToken);

                return new ListCustomerResponse { Customers = customerData };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
