using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Vendors.Query
{
    public class GetVendorQuery : IRequest<GetVendorResponse>
    {
        public int VendorId { get; set; }
    }

    public class GetVendorResponse
    {
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorPhoneNumber { get; set; }
        public string VendorAddress { get; set; }
        public VendorType VendorType { get; set; }
        public VendorStatus VendorStatus { get; set; }
        public string VendorContactName { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string VendorContactEmail { get; set; }
        public string VendorContactPhoneNumber { get; set; }
    }

    public class GetVendorHandler : IRequestHandler<GetVendorQuery, GetVendorResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public GetVendorHandler(IIMSDbContext iMSDbContext,
                                IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<GetVendorResponse> Handle (GetVendorQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var vendorData = await _dbContext.Vendor
                     .Include(v => v.VendorContact)
                     .FirstOrDefaultAsync(v => v.VendorId == request.VendorId, cancellationToken) ??
                     throw new Exception($"Vendor with ID {request.VendorId} not found!");

                return new GetVendorResponse
                {
                    VendorName = vendorData.VendorName,
                    VendorEmail = vendorData.Email,
                    VendorPhoneNumber = vendorData.PhoneNumber,
                    VendorAddress = vendorData.Address,
                    VendorType = vendorData.VendorType,
                    VendorStatus = vendorData.VendorStatus,
                    VendorContactName = vendorData.VendorContact?.Name ?? string.Empty,
                    Position = vendorData.VendorContact?.Position,
                    Department = vendorData.VendorContact?.Department,
                    VendorContactEmail = vendorData.VendorContact?.Email ?? string.Empty,
                    VendorContactPhoneNumber = vendorData.VendorContact?.PhoneNumber ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
