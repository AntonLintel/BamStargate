using MediatR.Pipeline;
using MediatR;
using Stargate.Server.Data.Models;
using Stargate.Server.Data;
using Stargate.Server.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Stargate.Server.Business.Commands
{
    public class AssignAstronautDuty : IRequest<AssignAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class AssignAstronautDutyPreProcessor : IRequestPreProcessor<AssignAstronautDuty>
    {
        private readonly StargateContext _context;

        public AssignAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(AssignAstronautDuty request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrEmpty(request.Name))
                throw new Exception("Bad Request");

            // name works because of rule one
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name.ToLower() == request.Name.ToLower());

            if (person is null)
                throw new Exception($"Unable to find records for {request.Name}. Please contact support.");

            return Task.CompletedTask;
        }
    }

    public class AssignAstronautDutyHandler : IRequestHandler<AssignAstronautDuty, AssignAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public AssignAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }

        // - update persons current rank and title
        // - close previous duty's end date
        // - add current duty
        public async Task<AssignAstronautDutyResult> Handle(AssignAstronautDuty request, CancellationToken cancellationToken)
        {// RULE: Only hold one rank/title at a time
            if (request is null || string.IsNullOrEmpty(request.Name))
                throw new Exception("Bad Request");

            var person = await _context.People.FirstOrDefaultAsync(p => p.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            // Person 
            if (person is null)
                throw new Exception($"{request.Name} was not found in the Database. Please contact support.");

            // RULE: Classified as Retired (if duty title is retired)
            person.CurrentDutyTitle = request.DutyTitle;
            person.CurrentRank = request.Rank;

            // RULE: Career end Date one day before retirement date
            if (request.DutyTitle == "RETIRED")
                person.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;

            _context.People.Update(person);

            // RULE: previous duty end date set to day before new duty start date
            // RULE: astronaut that has not been assigned a duty will not have an astronaut record
            var astronautDuty = await _context.AstronautDuties.FirstOrDefaultAsync(p => p.PersonId == person.Id && p.DutyEndDate == null, cancellationToken);
            if (astronautDuty != null)
            {
                astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                _context.AstronautDuties.Update(astronautDuty);
            }

            // RULE: current duty will not have DutyEndDate
            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new AssignAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class AssignAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
