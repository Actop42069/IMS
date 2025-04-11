using Application.Interface;
using Domain.Entities;
using Domain.Enumeration;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.Customers.Command
{
    public class UpdateCustomerCommand : IRequest<UpdateCustomerResponse>
    {
        [JsonIgnore]
        public int CustomerId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        [JsonIgnore]
        public string LastUpdatedBy { get; set; } 

        public IFormFile? Image { get; set; }
    }

    public class UpdateCustomerResponse
    {
        public string Message { get; set; }
    }

    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;
        private readonly IFileService _fileService;

        public UpdateCustomerHandler(IIMSDbContext dbContext,
                                     IErrorLogService errorLogService,
                                     IFileService fileService)
        {
            _dbContext = dbContext;
            _errorLogService = errorLogService;
            _fileService = fileService;
        }

        public async Task<UpdateCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _dbContext.Customer.FindAsync(request.CustomerId, cancellationToken)
                    ?? throw new Exception("Customer not found!");

                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;
                customer.Email = request.Email;
                customer.PhoneNumber = request.PhoneNumber;
                customer.CustomerType = request.CustomerType;
                customer.LastUpdatedBy = request.LastUpdatedBy;
                customer.LastUpdatedAt = DateTimeOffset.UtcNow;

                if (request.Image != null && request.Image.Length > 0)
                {
                    var fileName = $"{request.FirstName}{request.LastName}" + Guid.NewGuid();
                    var fileExtension = Path.GetExtension(fileName);
                    await _fileService.SaveFile($"customers/{fileName}{fileExtension}", request.Image.OpenReadStream());
                    customer.ImageUrl = $"{fileName}{fileExtension}";
                }
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new UpdateCustomerResponse
                {
                    Message = $"Customer with Id: {request.CustomerId} has been updated successfully."
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
