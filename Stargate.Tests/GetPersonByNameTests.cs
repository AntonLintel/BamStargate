using Microsoft.EntityFrameworkCore;
using Stargate.Server.Business.Queries;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

namespace Stargate.Tests
{
    [TestFixture]
    public class GetPersonByNameTests
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
        public async Task GetPersonByNameHandler_ValidName_ReturnsPerson()
        {
            // Arrange
            var query = new GetPersonByName { Name = "John Doe" };
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Name = "John Doe", CurrentRank = "Captain", CurrentDutyTitle = "Mission Specialist", CareerStartDate = DateTime.Now.AddYears(-5) });
            context.SaveChanges();
            var handler = new GetPersonByNameHandler(context);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Person, Is.Not.Null);
            Assert.That(result.Person.Name, Is.EqualTo("John Doe"));
        }

        [Test]
        public void GetPersonByNameHandler_InvalidName_ThrowsException()
        {
            // Arrange
            var query = new GetPersonByName { Name = "NonExistent" };
            using var context = new StargateContext(_contextOptions);
            var handler = new GetPersonByNameHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"There was an issue getting {query.Name}'s record. Please contact support."));
        }

        [Test]
        public void GetPersonByNameHandler_NullRequest_ThrowsException()
        {
            // Arrange
            var query = new GetPersonByName { Name = null };
            using var context = new StargateContext(_contextOptions);
            var handler = new GetPersonByNameHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Requests"));
        }

        [Test]
        public void GetPersonByNameHandler_EmptyName_ThrowsException()
        {
            // Arrange
            var query = new GetPersonByName { Name = "" }; // Empty name
            using var context = new StargateContext(_contextOptions);
            var handler = new GetPersonByNameHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Requests"));
        }
    }
}
