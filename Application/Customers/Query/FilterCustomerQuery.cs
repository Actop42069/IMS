using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Application.Customers.Query
{
    public class FilterCustomerQuery : IRequest<List<FilterCustomerResponse>>
    {
        public string SearchText { get; set; }
    }

    public class FilterCustomerResponse
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

    public class FilterCustomerHandler : IRequestHandler<FilterCustomerQuery, List<FilterCustomerResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public FilterCustomerHandler(IIMSDbContext dbContext,
                                   IErrorLogService errorLogService)
        {
            _dbContext = dbContext;
            _errorLogService = errorLogService;
        }

        public async Task<List<FilterCustomerResponse>> Handle(FilterCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var searchText = request.SearchText?.Trim().ToLower() ?? string.Empty;

                var customers = await _dbContext.Customer
                    .Where(c => string.IsNullOrEmpty(searchText) ||
                               (c.FirstName != null &&
                                c.FirstName.ToLower().Contains(searchText)) ||
                               (c.LastName != null &&
                                c.LastName.ToLower().Contains(searchText)) ||
                               (c.Email != null &&
                                c.Email.ToLower().Contains(searchText)) ||
                               (c.PhoneNumber != null &&
                                c.PhoneNumber.ToLower().Contains(searchText)))
                    .AsNoTracking()
                    .Select(c => new FilterCustomerResponse
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
                    .ToListAsync(cancellationToken);

                return customers;
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}