using MediatR;
using Stargate.Server.Controllers;
using Stargate.Server.Data.Models;
using Stargate.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Stargate.Server.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        public GetPersonByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrEmpty(request.Name))
                throw new Exception("Bad Requests");

            var result = new GetPersonByNameResult();

            var person = await _context.People.FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);

            if (person is null)
                throw new Exception($"There was an issue getting {request.Name}'s record. Please contact support.");

            result.Person = person;

            return result;
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public Person? Person { get; set; }
    }
}
