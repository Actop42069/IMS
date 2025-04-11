using Api.Controllers;
using Application.Categories.Command;
using Application.Categories.Query;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IOptions<ApplicationConfiguration> options) : base(options) { }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("create-category")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            try
            {
                command.CurrentUserName = CurrentUserName;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeleteCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPatch("delete-category")]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoryCommand command)
        {
            try
            {
                command.UpdatedUser = CurrentUserName;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeleteCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryCommand command)
        {
            try
            {
                command.CategoryId = id;
                command.UpdateUserName = CurrentUserName;
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("get-category/{id}")]
        [ProducesResponseType(typeof(GetCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            try
            {
                var query = new GetCategoryQuery { CategoryId = id };
                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("list-categories")]
        [ProducesResponseType(typeof(ListCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ListCategories([FromQuery] int pageNumber)
        {
            try
            {
                var query = new ListCategoryQuery { PageNumber = pageNumber};
                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("filter-category")]
        [ProducesResponseType(typeof(GetCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> Filter([FromQuery] FilterCategoryQuery query )
        {
            try
            {
                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
