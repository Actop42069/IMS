using Api.Controllers;
using Application.Vendors.Command;
using Application.Vendors.Query;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class VendorController : BaseController
    {
        public VendorController(IOptions<ApplicationConfiguration> options) : base(options)
        {           
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("create-vendor")]
        public async Task<IActionResult> Create([FromBody] CreateVendorCommand command, CancellationToken cancellationToken)
        {
            try
            {
                command.UpdatedUserName = CurrentUserName;
                var response = await Mediator.Send(command, cancellationToken);
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
        [ProducesResponseType(typeof(DeleteVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPut("delete-vendor")]
        public async Task<IActionResult> Delete([FromBody] DeleteVendorCommand command, CancellationToken cancellationToken)
        {
            try
            {
                command.LastUpdatedUser = CurrentUserName;
                var response = await Mediator.Send(command, cancellationToken);
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
        [ProducesResponseType(typeof(UpdateVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPut("update-vendor/{id}")]
        public async Task<IActionResult> Update ([FromRoute] int id, [FromBody] UpdateVendorCommand command, CancellationToken cancellationToken)
        {
            try
            {
                command.VendorId = id;
                command.UpdatedUserName = CurrentUserName;
                var response = await Mediator.Send(command, cancellationToken);
                return Ok(response);
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet("get-vendor/{id}")]
        [ProducesResponseType(typeof(GetVendorResponse), 200)]
        public async Task<IActionResult> GetVendor([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetVendorQuery { VendorId = id };
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
        [HttpGet("list-vendors")]
        [ProducesResponseType(typeof(ListVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ListCategories([FromQuery] int pageNumber)
        {
            try
            {
                var query = new ListVendorQuery { PageNumber = pageNumber };
                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("filter-vendor")]
        [ProducesResponseType(typeof(GetVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> Filter([FromQuery] FilterVendorQuery query)
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
