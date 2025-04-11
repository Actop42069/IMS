using Application.Interface;
using Domain.Enumeration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Query
{
    public class FilterCategoryQuery : IRequest<List<FilterCategoryResponse>>
    {
        public string SearchText { get; set; }
    }

    public class FilterCategoryResponse
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public CategoryStatus Status { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class FilterCategoryHandler : IRequestHandler<FilterCategoryQuery, List<FilterCategoryResponse>>
    {
        private readonly IIMSDbContext _dbContext;
        private readonly IErrorLogService _errorLogService;

        public FilterCategoryHandler(IIMSDbContext iMSDbContext,
                                        IErrorLogService errorLogService)
        {
            _dbContext = iMSDbContext;
            _errorLogService = errorLogService;
        }

        public async Task<List<FilterCategoryResponse>> Handle(FilterCategoryQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var searchText = query.SearchText?.Trim().ToLower() ?? string.Empty;

                var categories = await _dbContext.Category
                    .Where(c => string.IsNullOrEmpty(searchText) ||
                               (c.CategoryName != null &&
                                c.CategoryName.ToLower().Contains(searchText)) ||
                               (c.Description != null &&
                                c.Description.ToLower().Contains(searchText)))
                    .AsNoTracking()
                    .Select(c => new FilterCategoryResponse
                    {
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        Status = c.Status,
                        IsActive = c.IsActive,
                        LastUpdatedAt = c.LastUpdatedAt,
                        LastUpdatedBy = c.LastUpdatedBy
                    })
                    .ToListAsync(cancellationToken);

                if (!categories.Any())
                {
                    throw new Exception("No categories found matching the search criteria!");
                }

                return categories;
            }
            catch (Exception ex)
            {
                _errorLogService.LogError(ex);
                throw;
            }
        }
    }
}