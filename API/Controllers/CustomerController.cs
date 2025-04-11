using Api.Controllers;
using Application.Customers.Command;
using Application.Customers.Query;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class CustomerController : BaseController
    {
        public CustomerController(IOptions<ApplicationConfiguration> options) : base(options) { }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateCustomerResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("create-customer")]
        public async Task<IActionResult> Create([FromForm] CreateCustomerCommand command)
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
        [ProducesResponseType(typeof(DeleteCustomerResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPatch("delete-customer")]
        public async Task<IActionResult> Delete([FromBody] DeleteCustomerCommand command)
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
        [ProducesResponseType(typeof(UpdateCustomerResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPut("update-customer/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateCustomerCommand command)
        {
            try
            {
                command.CustomerId = id;
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
        [HttpGet("get-customer/{id}")]
        [ProducesResponseType(typeof(GetCustomerResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetCustomer([FromRoute] int id)
        {
            try
            {
                var query = new GetCustomerQuery { CustomerId = id };
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
        [HttpGet("list-customers")]
        [ProducesResponseType(typeof(ListCustomerResponse), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> ListCustomers([FromQuery] int pageNumber)
        {
            try
            {
                var query = new ListCustomerQuery { PageNumber = pageNumber };
                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("filter-customer")]
        [ProducesResponseType(typeof(FilterCustomerResponse), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> Filter([FromQuery] FilterCustomerQuery query)
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
