using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stargate.Server.Business.Commands;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

namespace Stargate.Tests
{
    [TestFixture]
    public class CreatePersonTests
    {
        private DbContextOptions<StargateContext> _contextOptions;

        [SetUp]
        public void Setup()
        {
            _contextOptions = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "StargateTestDb")
                .Options;

            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Name = "ExistingName" });
            context.SaveChanges();
        }

        [Test]
        public void CreatePersonPreProcessor_DuplicateName_ThrowsException()
        {
            // Arrange
            var command = new CreatePerson { Name = "ExistingName" };
            using var context = new StargateContext(_contextOptions);
            var preProcessor = new CreatePersonPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Someone with this name already exists in the DB."));
        }

        [Test]
        public void CreatePersonPreProcessor_NullRequest_ThrowsException()
        {
            // Arrange
            CreatePerson command = null;
            using var context = new StargateContext(_contextOptions);
            var preProcessor = new CreatePersonPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.AreEqual("Bad Request", ex.Message);
        }

        [Test]
        public async Task CreatePersonHandler_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var command = new CreatePerson { Name = "NewName" };
            using var context = new StargateContext(_contextOptions);
            var handler = new CreatePersonHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That((await context.People.FindAsync(result.Id)).Name, Is.EqualTo("NewName"));
        }

        [Test]
        public void CreatePersonHandler_InvalidRequest_ThrowsException()
        {
            // Arrange
            var command = new CreatePerson { Name = null }; // Invalid request
            using var context = new StargateContext(_contextOptions);
            var handler = new CreatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Request invalid"));
        }        

        [Test]
        public void CreatePersonHandler_NullRequest_ThrowsException()
        {
            // Arrange
            CreatePerson command = null;
            using var context = new StargateContext(_contextOptions);
            var handler = new CreatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.AreEqual("Request invalid", ex.Message);
        }

        [Test]
        public void CreatePersonHandler_EmptyName_ThrowsException()
        {
            // Arrange
            var command = new CreatePerson { Name = "" }; // Empty name
            using var context = new StargateContext(_contextOptions);
            var handler = new CreatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.AreEqual("Request invalid", ex.Message);
        }
    }
}
