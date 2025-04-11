using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Vendors.Query
{
    public class FilterVendorQuery : IRequest<List<FilterVendorResponse>>
    {
        public string SearchText { get; set; }
    }

    public class FilterVendorResponse
    {
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorPhoneNumber { get; set; }
        public string VendorAddress { get; set; }
        public VendorType VendorType { get; set; }
        public VendorStatus VendorStatus { get; set; }
        public string VendorContactName { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class FilterVendorHandler : IRequestHandler<FilterVendorQuery, List<FilterVendorResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public FilterVendorHandler(IIMSDbContext iMSDbContext,
                                 IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<List<FilterVendorResponse>> Handle(FilterVendorQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var searchText = query.SearchText?.Trim().ToLower() ?? string.Empty;
                var vendors = await _dbContext.Vendor
                    .Include(v => v.VendorContact)
                    .Where(v => string.IsNullOrEmpty(searchText) ||
                               (v.VendorName != null &&
                                v.VendorName.ToLower().Contains(searchText)) ||
                               (v.Email != null &&
                                v.Email.ToLower().Contains(searchText)) ||
                               (v.PhoneNumber != null &&
                                v.PhoneNumber.ToLower().Contains(searchText)) ||
                               (v.VendorContact.Name != null &&
                                v.VendorContact.Name.ToLower().Contains(searchText)))
                    .AsNoTracking()
                    .Select(v => new FilterVendorResponse
                    {
                        VendorName = v.VendorName,
                        VendorEmail = v.Email,
                        VendorPhoneNumber = v.PhoneNumber,
                        VendorAddress = v.Address,
                        VendorType = v.VendorType,
                        VendorStatus = v.VendorStatus,
                        VendorContactName = v.VendorContact.Name,
                        LastUpdatedAt = v.LastUpdatedAt,
                        LastUpdatedBy = v.LastUpdatedBy
                    })
                    .ToListAsync(cancellationToken);

                if (!vendors.Any())
                {
                    throw new Exception("No vendors found matching the search criteria!");
                }

                return vendors;
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}