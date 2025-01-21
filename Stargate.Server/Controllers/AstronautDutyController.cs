using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stargate.Server.Business.Commands;
using Stargate.Server.Business.Queries;
using Stargate.Server.Utilities;
using System.Net;

namespace Stargate.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AstronautDutyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/GetDutiesByName{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest();

                var result = await _mediator.Send(new GetAstronautDutiesByName()
                {
                    Name = name
                });

                if (result.Success)
                    DbLogger.Logger.Information($"Successfully acquired duty data for {name}!");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                DbLogger.Logger.Error("An error occured while attempting to retrieve duties for someone. Message: " + ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        [HttpPost("/api/AssignAstronautDuty")]
        public async Task<IActionResult> AssignAstronautDuty([FromBody] AssignAstronautDuty request)
        {
            try
            {
                if (request is null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.DutyTitle) || string.IsNullOrEmpty(request.Rank))
                    return BadRequest();

                var result = await _mediator.Send(request);

                if (result.Success)
                    DbLogger.Logger.Information($"Successful assign new duty to {request.Name}!");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                DbLogger.Logger.Error("An error occured while attempting to assign a new astronaut duty. Message: " + ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }
    }
}
