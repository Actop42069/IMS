using Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Categories.Command
{
    public class DeleteCategoryCommand : IRequest<DeleteCategoryResponse>
    {
        public int CategoryId { get; set; }
        [JsonIgnore]
        public string UpdatedUser { get; set; } = "a";
    }

    public class DeleteCategoryResponse
    {
        public string Message { get; set; }
    }

    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponse>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _logger;

        public DeleteCategoryHandler(IIMSDbContext iMSDbContext,
                                        IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _logger = errorLogService;
        }

        public async Task<DeleteCategoryResponse> Handle (DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _dbContext.Category.FindAsync(command.CategoryId, cancellationToken) ?? throw new Exception("Category not found");
                data.IsActive = !data.IsActive;
                data.LastUpdatedAt = DateTimeOffset.UtcNow;
                data.LastUpdatedBy = command.UpdatedUser;

                _dbContext.Category.Update(data);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new DeleteCategoryResponse { Message = $"Category of Id: {command.CategoryId} has been deleted" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }
    }
}
