using Application.Interface;
using Domain.Entities;
using Domain.Enumeration;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.Customers.Command
{
    public class CreateCustomerCommand : IRequest<CreateCustomerResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerType CustomerType { get; set; }
        public IFormFile? Image { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }

    public class CreateCustomerResponse
    {
        public string Message { get; set; }
    }

    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;
        private readonly IFileService _fileService;

        public CreateCustomerHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService,
                                    IFileService fileService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
            _fileService = fileService;
        }

        public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userData = new Customer
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CustomerType = request.CustomerType,
                    IsActive = true,
                    LastUpdatedAt = DateTimeOffset.Now,
                    LastUpdatedBy = request.LastUpdatedBy
                };

                if (request.Image != null && request.Image.Length > 0)
                {
                    var fileName = $"{request.FirstName}{request.LastName}" + Guid.NewGuid();
                    var fileExtension = Path.GetExtension(fileName);
                    await _fileService.SaveFile($"customers/{fileName}{fileExtension}", request.Image.OpenReadStream());
                    userData.ImageUrl = $"{fileName}{fileExtension}";
                }

                _dbContext.Customer.Add(userData);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new CreateCustomerResponse { Message = $"User of name {request.FirstName} has been added to the database." };
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
