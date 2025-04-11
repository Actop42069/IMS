using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Query
{
    public class GetCategoryQuery : IRequest<GetCategoryResponse>
    {
        public int CategoryId { get; set; }
    }

    public class GetCategoryResponse
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public CategoryStatus Status { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, GetCategoryResponse>
    {
        private readonly IIMSDbContext _dbContext;

        public GetCategoryHandler(IIMSDbContext iMSDbContext)
        {
            _dbContext = iMSDbContext;
        }

        public async Task<GetCategoryResponse> Handle (GetCategoryQuery query, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Category
               .Where(c => c.CategoryId == query.CategoryId)
               .AsNoTracking()
               .Select(c => new GetCategoryResponse
               {
                   CategoryName = c.CategoryName,
                   Description = c.Description,
                   Status = c.Status,
                   IsActive = c.IsActive,
                   LastUpdatedAt = c.LastUpdatedAt,
                   LastUpdatedBy = c.LastUpdatedBy
               })
               .FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("No category found !");
            return category;
        }
    } 
}
