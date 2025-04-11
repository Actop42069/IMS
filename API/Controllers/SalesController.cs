using Api.Controllers;
using Application.Sale.Command;
using Application.Sale.Query;
using Common.Configurations;
using Common.Exceptions;
using Domain.Enumeration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class SalesController : BaseController
    {
        public SalesController(IOptions<ApplicationConfiguration> options) : base(options) { }

        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateSalesResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost("create-sales")]
        public async Task<IActionResult> Create([FromBody] CreateSalesCommand command)
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
        [HttpGet("get-salesdata-individual/{id}")]
        [ProducesResponseType(typeof(GetIndividualSalesDetailResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetProductFromVendor([FromRoute] int id)
        {
            try
            {
                var query = new GetIndividualSalesDetailQuery { SalesId = id };
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
        [HttpGet("sales-analysis/{period}")]
        [ProducesResponseType(typeof(List<SalesAnalysisResponse>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetSalesAnalysis([FromRoute] TimePeriod period ,CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSalesAnalysisQuery { Period = period };
                var response = await Mediator.Send(query, cancellationToken);
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
                return StatusCode(500,ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("sales-analysis-all-date/{date}")]
        [ProducesResponseType(typeof(Dictionary<string, List<SalesAnalysisResponse>>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetSalesAnalysisAllPeriods([FromRoute] DateTimeOffset date, CancellationToken cancellationToken)
        {
            try
            {
                var allPeriodsData = new Dictionary<string, List<SalesAnalysisResponse>>();
                foreach (TimePeriod period in Enum.GetValues(typeof(TimePeriod)))
                {
                    var query = new GetSalesAnalysisQuery
                    {
                        FilterDate = date,
                        Period = period
                    };

                    var response = await Mediator.Send(query, cancellationToken);
                    allPeriodsData.Add(period.ToString(), response);
                }
                   return Ok(allPeriodsData);
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
                return StatusCode(500,ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("sales-category/{id}")]
        [ProducesResponseType(typeof(GetSalesFromCategoryResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetSalesReportOfCategory([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSalesFromCategoryQuery { CategoryId = id };
                var response = await Mediator.Send(query, cancellationToken);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(ApplicationException ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("sales-vendor/{id}")]
        [ProducesResponseType(typeof(GetSalesFromVendorResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetSalesReportOfVendor([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSalesFromVendorQuery { VendorId = id };
                var response = await Mediator.Send(query, cancellationToken);
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
