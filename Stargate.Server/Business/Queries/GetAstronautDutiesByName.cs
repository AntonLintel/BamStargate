using MediatR;
using Stargate.Server.Controllers;
using Stargate.Server.Data.Models;
using Stargate.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Stargate.Server.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrEmpty(request.Name))
                throw new Exception("Bad Request");

            var result = new GetAstronautDutiesByNameResult();

            var person = await _context.People.FirstOrDefaultAsync(p => p.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (person is null)
                throw new Exception($"There was an issue getting {request.Name}'s duties. Please contact support.");

            var duties = await _context.AstronautDuties.Where(p => p.PersonId == person.Id).OrderBy(d => d.DutyStartDate).ToListAsync(cancellationToken);

            if (duties.Count != 0)
                result.AstronautDuties = duties;

            return result;
        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
