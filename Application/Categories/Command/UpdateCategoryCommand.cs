using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Application.Categories.Command
{
    public class UpdateCategoryCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int CategoryId { get; set; } 
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public CategoryStatus Status { get; set; }

        [JsonIgnore]
        public string UpdateUserName { get; set; } = "a";
    }

    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Unit>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _logger;

        public UpdateCategoryHandler(IIMSDbContext iMSDbContext,
                                    IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _logger = errorLogService;
        }

        public async Task<Unit> Handle (UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _dbContext.Category.FirstOrDefaultAsync(a => a.CategoryId == command.CategoryId, cancellationToken) ?? throw new Exception("No Category found !");

                data.CategoryName = command.CategoryName;
                data.Description = command.Description;
                data.Status = command.Status;
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }
    }
}
