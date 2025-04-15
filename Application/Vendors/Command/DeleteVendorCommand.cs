using Application.Interface;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Vendors.Command
{
    public class DeleteVendorCommand : IRequest<DeleteVendorResponse>
    {
        public int VendorId { get; set; }
        [JsonIgnore]
        public string LastUpdatedUser { get; set; } 
    }

    public class DeleteVendorResponse
    {
        public string Message { get; set; }
    }

    public class DeleteVendorHandler : IRequestHandler<DeleteVendorCommand, DeleteVendorResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public DeleteVendorHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<DeleteVendorResponse> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vendor = await _dbContext.Vendor.FindAsync(request.VendorId, cancellationToken) ?? throw new Exception($"Vendor with ID {request.VendorId} not found.");
                vendor.IsActive = !vendor.IsActive;
                vendor.LastUpdatedAt = DateTimeOffset.UtcNow;
                vendor.LastUpdatedBy = request.LastUpdatedUser;

                _dbContext.Vendor.Update(vendor);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new DeleteVendorResponse { Message = $"{vendor.VendorName} Vendor has been deleted." };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
