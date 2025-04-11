using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Products.Command
{
    public class UpdateProductCommand : IRequest<UpdateProductResponse>
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public IFormFile Image { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }

    public class UpdateProductResponse
    {
        public string Message { get; set; }
    }

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;
        private readonly IFileService _fileService;

        public UpdateProductHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService,
                                    IFileService fileService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
            _fileService = fileService;
        }

        public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _dbContext.Product.FindAsync(request.ProductId, cancellationToken) ?? throw new Exception("Product not found!");

                product.Name = request.Name;
                product.Description = request.Description;
                product.ProductStatus = request.ProductStatus;
                product.ProductType = request.ProductType;
                product.Price = request.Price;
                product.UnitCost = request.UnitCost;
                product.Quantity = request.Quantity;
                product.LastUpdatedAt = DateTimeOffset.Now;
                product.LastUpdatedBy = request.LastUpdatedBy;

                if (request.Image != null && request.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid();
                    var fileExtension = Path.GetExtension(request.Image.FileName);
                    await _fileService.SaveFile($"products/{fileName}{fileExtension}", request.Image.OpenReadStream());
                    product.ImageUrl = $"{fileName}{fileExtension}";
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                return new UpdateProductResponse
                {
                    Message = "Product updated successfully"
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
