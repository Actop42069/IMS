using Application.Interface;
using Domain.Entities;
using Domain.Enumeration;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.Products.Command
{
    public class CreateProductCommand : IRequest<CreateProductResponse>
    {
        public int VendorId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public IFormFile? Image { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }

    public class CreateProductResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;
        private readonly IFileService _fileService;

        public CreateProductHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService,
                                    IFileService fileService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService; 
            _fileService = fileService;
        }

        public async Task<CreateProductResponse> Handle (CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = new Product
                {
                    VendorId = command.VendorId,
                    CategoryId = command.CategoryId,
                    Name = command.Name,
                    Description = command.Description,
                    SKU = command.SKU,
                    ProductStatus = command.ProductStatus,
                    ProductType = command.ProductType,
                    Price = command.Price,
                    UnitCost = command.UnitCost,
                    Quantity = command.Quantity,
                    LastUpdatedAt = DateTimeOffset.Now,
                    LastUpdatedBy = command.LastUpdatedBy,
                    IsActive = true
                };

                if (command.Image != null && command.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid();
                    var fileExtension = Path.GetExtension(command.Image.FileName);
                    await _fileService.SaveFile($"products/{fileName}{fileExtension}", command.Image.OpenReadStream());
                    product.ImageUrl = $"{fileName}{fileExtension}"; 
                }

                _dbContext.Product.Add(product);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CreateProductResponse
                {
                    Name = product.Name,
                    Description = product.Description,
                    SKU = product.SKU,
                    ProductStatus = product.ProductStatus,
                    ProductType = product.ProductType,
                    Price = product.Price,
                    UnitCost = product.UnitCost,
                    Quantity = product.Quantity,
                    ImageUrl = product.ImageUrl
                };
            }
            catch(Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}
