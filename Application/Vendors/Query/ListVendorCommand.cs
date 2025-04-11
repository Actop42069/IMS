using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Vendors.Query
{
    public class ListVendorQuery : IRequest<ListVendorResponse>
    {
        public int PageNumber { get; set; } = 1; //default page number
    }

    public class ListVendorResponse
    {
        public List<VendorDto> Vendors { get; set; }
        public ListVendorResponse()
        {
            Vendors = new List<VendorDto>();
        }
    }

    public class VendorDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorPhoneNumber { get; set; }
        public string VendorAddress { get; set; }
        public VendorType VendorType { get; set; }
        public VendorStatus VendorStatus { get; set; }
        public string VendorContactName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string VendorContactEmail { get; set; }
        public string VendorContactPhoneNumber { get; set; }
    }

    public class ListVendorHandler : IRequestHandler<ListVendorQuery, ListVendorResponse>
    {
        private readonly IIMSDbContext _dbContext;

        public ListVendorHandler(IIMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ListVendorResponse> Handle(ListVendorQuery query, CancellationToken cancellationToken)
        {
            const int pageSize = 500;
            var skip = (query.PageNumber - 1) * pageSize;

            var totalRecords = await _dbContext.Vendor.CountAsync(cancellationToken);

            var vendors = await _dbContext.Vendor
                .Include(v => v.VendorContact)
                .AsNoTracking()
                .OrderBy(v => v.VendorId)
                .Skip(skip)
                .Take(pageSize)
                .Select(v => new VendorDto
                {
                    VendorId = v.VendorId,
                    VendorName = v.VendorName,
                    VendorEmail = v.Email,
                    VendorPhoneNumber = v.PhoneNumber,
                    VendorAddress = v.Address,
                    VendorType = v.VendorType,
                    VendorStatus = v.VendorStatus,
                    VendorContactName = v.VendorContact.Name,
                    Position = v.VendorContact.Position,
                    Department = v.VendorContact.Department,
                    VendorContactEmail = v.VendorContact.Email,
                    VendorContactPhoneNumber = v.VendorContact.PhoneNumber
                })
                .ToListAsync(cancellationToken);

            return new ListVendorResponse { Vendors = vendors };
        }
    }
}