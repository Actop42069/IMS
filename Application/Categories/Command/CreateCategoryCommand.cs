using Application.Interface;
using Domain.Entities;
using Domain.Enumeration;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Categories.Command
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public CreateCategoryHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<CreateCategoryResponse> Handle (CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var response = new Category
                {
                    CategoryName = command.CategoryName,
                    Description = command.Description,
                    Status = CategoryStatus.Active,
                    IsActive = true,
                    LastUpdatedAt = DateTimeOffset.Now,
                    LastUpdatedBy = command.CurrentUserName
                };

                _dbContext.Category.Add(response);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CreateCategoryResponse { CategoryId = response.CategoryId};
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }

    public class CreateCategoryCommand : IRequest<CreateCategoryResponse>
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public string CurrentUserName { get; set; } = "a";
    }

    public class CreateCategoryResponse
    {
        public int CategoryId { get; set; }
    }
}
