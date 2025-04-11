﻿using Application.Interface;
using Domain.Entities;
using Domain.Enumeration;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Vendors.Command
{
    public class CreateVendorCommand : IRequest<CreateVendorResponse>
    {
        public string VendorName  { get; set; }
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
    
    public class CreateVendorResponse
    {
        public int VendorId { get; set; }
    }

    public class CreateVendorHandler : IRequestHandler <CreateVendorCommand, CreateVendorResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public CreateVendorHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<CreateVendorResponse> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vendorData = new Vendor
                {
                    VendorName = request.VendorName,
                    Email = request.VendorEmail,
                    PhoneNumber = request.VendorPhoneNumber,
                    Address = request.VendorAddress,
                    VendorType = request.VendorType,
                    VendorStatus = request.VendorStatus,
                    LastUpdatedAt = DateTimeOffset.UtcNow,
                    LastUpdatedBy = request.UpdatedUserName,
                    IsActive = true
                };

                _dbContext.Vendor.Add(vendorData);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var vendorContactData = new VendorContact
                {
                    VendorId = vendorData.VendorId, 
                    Name = request.VendorContactName,
                    Position = request.Position,
                    Department = request.Department,
                    Email = request.VendorContactEmail,
                    PhoneNumber = request.VendorContactPhoneNumber
                };

                _dbContext.VendorContact.Add(vendorContactData);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CreateVendorResponse { VendorId = vendorData.VendorId };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }

    }
}
