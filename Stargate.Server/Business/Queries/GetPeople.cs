using MediatR;
using Stargate.Server.Controllers;
using Stargate.Server.Data.Models;
using Stargate.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Stargate.Server.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public readonly StargateContext _context;
        public GetPeopleHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new Exception("Bad Request");

            var result = new GetPeopleResult();

            var people = await _context.People.ToListAsync();

            result.People = people;

            return result;
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<Person> People { get; set; } = new List<Person> { };
    }
}
