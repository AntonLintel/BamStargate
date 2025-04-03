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
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/GetPeople")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople() { });

                if (result.Success)
                    DbLogger.Logger.Information($"Successful retrieval of all people!");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                DbLogger.Logger.Error("An error occured while attempting to retrieve all people. Message: " + ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("/api/GetPersonByName/{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest();

                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                if (result.Success)
                    DbLogger.Logger.Information($"Successful retrieval of {name}!");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                DbLogger.Logger.Error("An error occured while attempting to retrieve Astronaut by Name. Message: " + ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }                      
        }

        [HttpPost("/api/Create")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest();

                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });

                if (result.Success)
                    DbLogger.Logger.Information($"Successful creation of {name}! You're a proud parent!");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                DbLogger.Logger.Error("An error occured while attempting to create a new person. Message: " + ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        [HttpPost("/api/UpdatePersonByName")]
        public async Task<IActionResult> UpdatePerson([FromBody] UpdatePerson personUpdates)
        {
            try
            {
                if (string.IsNullOrEmpty(personUpdates.OriginalName) || string.IsNullOrEmpty(personUpdates.NewName))
                    return BadRequest();

                var result = await _mediator.Send(personUpdates);

                if (result.Success)
                    DbLogger.Logger.Information($"Successfully updated {personUpdates.OriginalName} to {personUpdates.NewName}!");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                DbLogger.Logger.Error("An error occured while attempting rename a person. Message: " + ex.Message);
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
