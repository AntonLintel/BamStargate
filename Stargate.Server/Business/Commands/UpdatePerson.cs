using MediatR;
using MediatR.Pipeline;
using Stargate.Server.Controllers;
using Stargate.Server.Data.Models;
using Stargate.Server.Data;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Stargate.Server.Business.Commands
{
    public class UpdatePerson : IRequest<UpdatePersonResult>
    {
        public required string OriginalName { get; set; } = string.Empty;

        public required string NewName { get; set; } = string.Empty;
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;
        public UpdatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrEmpty(request.OriginalName) || string.IsNullOrEmpty(request.NewName))
                throw new Exception("Bad Request");

            var originalName = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.OriginalName);
            if (originalName is null)
                throw new Exception("That person does not currently exist in the Database.");

            var newName = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.NewName);
            if (newName is not null)
                throw new Exception("Someone with that name already exists in the DB.");

            return Task.CompletedTask;
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
    {
        private readonly StargateContext _context;

        public UpdatePersonHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrEmpty(request.OriginalName) || string.IsNullOrEmpty(request.NewName))
                throw new Exception("Request invalid");

            var person = await _context.People.FirstOrDefaultAsync(p => p.Name == request.OriginalName, cancellationToken);

            if (person is null)
                throw new Exception($"There was an issue getting {request.OriginalName}'s record. Please contact support.");

            person.Name = request.NewName;
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatePersonResult()
            {
                Id = person.Id
            };
        }
    }

    public class UpdatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
