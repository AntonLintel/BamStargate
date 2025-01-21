using Microsoft.EntityFrameworkCore;
using Stargate.Server.Business.Queries;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

namespace Stargate.Tests
{
    [TestFixture]
    public class GetPeopleTests
    {
        private DbContextOptions<StargateContext> _contextOptions;

        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "StargateTestDb")
                .Options;            
        }

        [Test]
        public async Task GetPeopleHandler_ReturnsPeople()
        {
            // Arrange
            var query = new GetPeople();
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Id = 1, Name = "John Doe", CurrentRank = "Captain", CurrentDutyTitle = "Mission Specialist", CareerStartDate = DateTime.Now.AddYears(-5) });
            context.People.Add(new Person { Id = 2, Name = "Jane Doe", CurrentRank = "Lieutenant", CurrentDutyTitle = "Pilot", CareerStartDate = DateTime.Now.AddYears(-3) });
            context.SaveChanges();
            var handler = new GetPeopleHandler(context);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.People, Is.Not.Null);
            Assert.That(result.People.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPeopleHandler_ReturnsEmptyListIfNoPeople()
        {
            // Arrange
            var query = new GetPeople();
            using var context = new StargateContext(_contextOptions);

            // Clear existing people for this test
            foreach (var person in context.People)
            {
                context.People.Remove(person);
            }
            await context.SaveChangesAsync();

            var handler = new GetPeopleHandler(context);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.People, Is.Not.Null);
            Assert.That(result.People.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetPeopleHandler_NullRequest_ThrowsException()
        {
            // Arrange
            GetPeople query = null;
            using var context = new StargateContext(_contextOptions);
            var handler = new GetPeopleHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Request"));
        }
    }
}
