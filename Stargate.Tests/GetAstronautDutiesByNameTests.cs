using Microsoft.EntityFrameworkCore;
using Stargate.Server.Business.Queries;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

namespace Stargate.Tests
{
    [TestFixture]
    public class GetAstronautDutiesByNameTests
    {
        private DbContextOptions<StargateContext> _contextOptions;

        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "StargateTestDb")
                .Options;

            using var context = new StargateContext(_contextOptions);
            
        }

        [Test]
        public async Task GetAstronautDutiesByNameHandler_ValidName_ReturnsDuties()
        {
            // Arrange
            var command = new GetAstronautDutiesByName { Name = "Robert Paulson" };
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Id = 112552, Name = "Robert Paulson", CurrentRank = "Captain", CurrentDutyTitle = "Mission Specialist", CareerStartDate = DateTime.Now.AddYears(-5) });
            context.AstronautDuties.Add(new AstronautDuty { Id = 23456, PersonId = 112552, Rank = "Lieutenant", DutyTitle = "Initial Duty", DutyStartDate = DateTime.Now.AddYears(-5), DutyEndDate = DateTime.Now.AddYears(-3) });
            context.AstronautDuties.Add(new AstronautDuty { Id = 34567, PersonId = 112552, Rank = "Captain", DutyTitle = "Current Duty", DutyStartDate = DateTime.Now.AddYears(-3) });
            context.SaveChanges();
            var handler = new GetAstronautDutiesByNameHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AstronautDuties[0].PersonId, Is.EqualTo(112552));
            Assert.That(result.AstronautDuties[0].Person.Name, Is.EqualTo("Robert Paulson"));
            Assert.That(result.AstronautDuties.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAstronautDutiesByNameHandler_InvalidName_ThrowsException()
        {
            // Arrange
            var command = new GetAstronautDutiesByName { Name = "NonExistent" };
            using var context = new StargateContext(_contextOptions);
            var handler = new GetAstronautDutiesByNameHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("There was an issue getting NonExistent's duties. Please contact support."));
        }

        [Test]
        public async Task GetAstronautDutiesByNameHandler_ValidName_NoDuties_ReturnsEmptyDutiesList()
        {
            // Arrange
            var command = new GetAstronautDutiesByName { Name = "NoDutiesName" };
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Id = 8675309, Name = "NoDutiesName" });
            await context.SaveChangesAsync();

            var handler = new GetAstronautDutiesByNameHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.AstronautDuties.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetAstronautDutiesByNameHandler_NullRequest_ThrowsException()
        {
            // Arrange
            GetAstronautDutiesByName query = null;
            using var context = new StargateContext(_contextOptions);
            var handler = new GetAstronautDutiesByNameHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Request"));
        }

        [Test]
        public void GetAstronautDutiesByNameHandler_EmptyName_ThrowsException()
        {
            // Arrange
            var query = new GetAstronautDutiesByName { Name = "" };
            using var context = new StargateContext(_contextOptions);
            var handler = new GetAstronautDutiesByNameHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Request"));
        }
    }
}
