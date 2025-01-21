using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stargate.Server.Business.Commands;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

namespace Stargate.Tests
{
    [TestFixture]
    public class CreateAstronautDutyTests
    {
        private DbContextOptions<StargateContext> _contextOptions;

        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "StargateTestDb")
                .Options;

            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Name = "ExistingName"});
            context.SaveChanges();
        }

        [Test]
        public void CreateAstronautDutyPreProcessor_NullRequest_ThrowsException()
        {
            // Arrange
            AssignAstronautDuty command = null;
            using var context = new StargateContext(_contextOptions);
            var preProcessor = new AssignAstronautDutyPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Request"));
        }

        [Test]
        public void CreateAstronautDutyHandler_NullRequest_ThrowsException()
        {
            // Arrange
            AssignAstronautDuty command = null;
            using var context = new StargateContext(_contextOptions);
            var handler = new AssignAstronautDutyHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Request"));
        }

        [Test]
        public void CreateAstronautDutyPreProcessor_InvalidName_ThrowsException()
        {
            // Arrange
            var command = new AssignAstronautDuty { Name = "NonExistent", Rank = "Captain", DutyTitle = "Mission Specialist", DutyStartDate = DateTime.Now };
            using var context = new StargateContext(_contextOptions);
            var preProcessor = new AssignAstronautDutyPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"Unable to find records for {command.Name}. Please contact support."));
        }

        [Test]
        public async Task CreateAstronautDutyHandler_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var command = new AssignAstronautDuty { Name = "ExistingName", Rank = "Captain", DutyTitle = "Mission Specialist", DutyStartDate = DateTime.Now };
            using var context = new StargateContext(_contextOptions);
            var handler = new AssignAstronautDutyHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.Null);

            // result ID is duty ID
            Assert.That(actual: (context.People.FirstOrDefault(p => p.Name == command.Name)).CurrentRank, Is.EqualTo("Captain"));
            Assert.That(actual: (context.People.FirstOrDefault(p => p.Name == command.Name)).CurrentDutyTitle, Is.EqualTo("Mission Specialist"));
        }

        [Test]
        public void CreateAstronautDutyHandler_InvalidName_ThrowsException()
        {
            // Arrange
            var command = new AssignAstronautDuty { Name = "NonExistent", Rank = "Captain", DutyTitle = "Mission Specialist", DutyStartDate = DateTime.Now };
            using var context = new StargateContext(_contextOptions);
            var handler = new AssignAstronautDutyHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("There was an issue getting NonExistent's record. Please contact support."));
        }

        [Test]
        public async Task CreateAstronautDutyHandler_RetiredPerson_SetsCareerEndDate()
        {
            // Arrange
            var command = new AssignAstronautDuty { Name = "ExistingName", Rank = "Retired", DutyTitle = "RETIRED", DutyStartDate = DateTime.Now };
            using var context = new StargateContext(_contextOptions);
            var handler = new AssignAstronautDutyHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That((await context.People.FindAsync(result.Id)).CurrentRank, Is.EqualTo("Retired"));
            Assert.That((await context.People.FindAsync(result.Id)).CurrentDutyTitle, Is.EqualTo("RETIRED"));
            Assert.That((await context.People.FindAsync(result.Id)).CareerEndDate, Is.EqualTo(command.DutyStartDate.AddDays(-1).Date));
        }

        [Test]
        public async Task CreateAstronautDutyHandler_UpdatesPreviousDutyEndDate()
        {
            // Arrange
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Name = "Richard Feynman" });
            context.SaveChanges();

            var existingPerson = await context.People.FirstOrDefaultAsync(p => p.Name == "Richard Feynman");
            context.AstronautDuties.Add(new AstronautDuty
            {
                PersonId = existingPerson.Id,
                Rank = "Lieutenant",
                DutyTitle = "Previous Duty",
                DutyStartDate = DateTime.Now.AddMonths(-6),
                DutyEndDate = null
            });
            await context.SaveChangesAsync();

            var command = new AssignAstronautDuty { Name = "Richard Feynman", Rank = "Captain", DutyTitle = "New Duty", DutyStartDate = DateTime.Now };
            var handler = new AssignAstronautDutyHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var previousDuty = await context.AstronautDuties.FirstOrDefaultAsync(d => d.DutyTitle == "Previous Duty" && d.PersonId == existingPerson.Id);
            Assert.That(previousDuty, Is.Not.Null);
            Assert.That(previousDuty.DutyEndDate, Is.EqualTo(command.DutyStartDate.AddDays(-1).Date));

            var newDuty = await context.AstronautDuties.FirstOrDefaultAsync(d => d.DutyTitle == "New Duty" && d.PersonId == existingPerson.Id);
            Assert.That(newDuty, Is.Not.Null);
            Assert.That(newDuty.Rank, Is.EqualTo("Captain"));
            Assert.That(newDuty.DutyTitle, Is.EqualTo("New Duty"));
            Assert.That(newDuty.DutyStartDate, Is.EqualTo(command.DutyStartDate.Date));
            Assert.That(newDuty.DutyEndDate, Is.Null);
        }
    }
}
