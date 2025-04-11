using Api.Controllers;
using Application.Products.Command;
using Application.Products.Query;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(IOptions<ApplicationConfiguration> options) : base(options) { }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateProductResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("create-product")]
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command)
        {
            try
            {
                command.LastUpdatedBy = CurrentUserName;
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
        [ProducesResponseType(typeof(DeleteProductResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPatch("delete-product")]
        public async Task<IActionResult> Delete([FromBody] DeleteProductCommand command)
        {
            try
            {
                command.LastUpdatedBy = CurrentUserName;
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
        [ProducesResponseType(typeof(UpdateProductResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateProductCommand command)
        {
            try
            {
                command.ProductId = id;
                command.LastUpdatedBy = CurrentUserName;
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
        [HttpGet("get-product/{id}")]
        [ProducesResponseType(typeof(GetProductResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            try
            {
                var query = new GetProductQuery { ProductId = id };
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
        [HttpGet("get-product-category/{id}")]
        [ProducesResponseType(typeof(ListProductOfCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetProductFromCategory([FromRoute] int id)
        {
            try
            {
                var query = new ListProductOfCategoryQuery { CategoryId = id };
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
        [HttpGet("get-product-vendor/{id}")]
        [ProducesResponseType(typeof(ListProductOfVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetProductFromVendor([FromRoute] int id)
        {
            try
            {
                var query = new ListProductOfVendorQuery { VendorId = id };
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
        [HttpGet("list-products")]
        [ProducesResponseType(typeof(ListAllProductResponse), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ListProducts([FromQuery] int pageNumber)
        {
            try
            {
                var query = new ListAllProductQuery { PageNumber = pageNumber };
                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("filter-products")]
        [ProducesResponseType(typeof(List<FilterProductResponse>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> Filter([FromQuery] FilterProductQuery query)
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
