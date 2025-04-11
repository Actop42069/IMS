using Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Query
{
    public class ListCategoryQuery : IRequest<ListCategoryResponse>
    {
        public int PageNumber { get; set; } = 1; //default page number
    }

    public class ListCategoryResponse
    {
        public List<CategoryDto> Categories { get; set; }

        public ListCategoryResponse()
        {
            Categories = new List<CategoryDto>();
        }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ListCategoryHandler : IRequestHandler<ListCategoryQuery, ListCategoryResponse>
    {
        private readonly IIMSDbContext _dbContext;

        public ListCategoryHandler(IIMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ListCategoryResponse> Handle(ListCategoryQuery query, CancellationToken cancellationToken)
        {
            const int pageSize = 500;
            var skip = (query.PageNumber - 1) * pageSize;
            var totalRecords = await _dbContext.Category.CountAsync(cancellationToken);

            var categories = await _dbContext.Category
                .Where(a => a.IsActive == true)
                .AsNoTracking()
                .OrderBy(c => c.CategoryId) 
                .Skip(skip)
                .Take(pageSize)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync(cancellationToken);


            return new ListCategoryResponse { Categories = categories };
        }
    }
}
