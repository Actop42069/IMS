using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Vendors.Command
{
    public class UpdateVendorCommand : IRequest<UpdateVendorResponse>
    {
        public int VendorId { get; set; }
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

        [JsonIgnore]
        public string UpdatedUserName { get; set; } = "a";
    }

    public class UpdateVendorResponse
    {
        public string Message { get; set; }
    }

    public class UpdateVendorHandler : IRequestHandler<UpdateVendorCommand, UpdateVendorResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public UpdateVendorHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<UpdateVendorResponse> Handle (UpdateVendorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vendorData = await _dbContext.Vendor.Include(a => a.VendorContact)
                                                            .Where(a => a.VendorId == request.VendorId)
                                                            .FirstOrDefaultAsync(cancellationToken) ??
                                                            throw new Exception("Vendor not found !");
                // Vendor Entities Update
                vendorData.VendorName = request.VendorName;
                vendorData.Email = request.VendorEmail;
                vendorData.PhoneNumber = request.VendorPhoneNumber;
                vendorData.Address = request.VendorAddress;
                vendorData.VendorType = request.VendorType;
                vendorData.VendorStatus = request.VendorStatus;
                vendorData.LastUpdatedAt = DateTimeOffset.UtcNow;
                vendorData.LastUpdatedBy = request.UpdatedUserName;

                // VendorContact Entities Update
                vendorData.VendorContact.Name = request.VendorContactName;
                vendorData.VendorContact.Position = request.Position;
                vendorData.VendorContact.Department = request.Department;
                vendorData.VendorContact.Email = request.VendorContactEmail;
                vendorData.VendorContact.PhoneNumber = request.VendorContactPhoneNumber;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return new UpdateVendorResponse
                {
                    Message = $"Vendor {vendorData.VendorName} updated successfully"
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
