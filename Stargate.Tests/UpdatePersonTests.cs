using Microsoft.EntityFrameworkCore;
using Stargate.Server.Business.Commands;
using Stargate.Server.Data;
using Stargate.Server.Data.Models;

namespace Stargate.Tests
{
    [TestFixture]
    public class UpdatePersonTests
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
        public void UpdatePersonPreProcessor_InvalidOriginalName_ThrowsException()
        {
            // Arrange
            var command = new UpdatePerson { OriginalName = "NonExistent", NewName = "NewName" };
            using var context = new StargateContext(_contextOptions);
            var preProcessor = new UpdatePersonPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("That person does not currently exist in the Database."));
        }

        [Test]
        public void UpdatePersonPreProcessor_DuplicateNewName_ThrowsException()
        {
            // Arrange
            var command = new UpdatePerson { OriginalName = "ExistingName", NewName = "ExistingName" };
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Name = "ExistingName" });
            context.SaveChanges();
            var preProcessor = new UpdatePersonPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Someone with that name already exists in the DB."));
        }

        [Test]
        public void UpdatePersonPreProcessor_NullRequest_ThrowsException()
        {
            // Arrange
            UpdatePerson command = null;
            using var context = new StargateContext(_contextOptions);
            var preProcessor = new UpdatePersonPreProcessor(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => preProcessor.Process(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Bad Request"));
        }

        [Test]
        public async Task UpdatePersonHandler_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var command = new UpdatePerson { OriginalName = "MyOldName", NewName = "NewName" };
            using var context = new StargateContext(_contextOptions);
            context.People.Add(new Person { Name = "MyOldName" });
            context.SaveChanges();
            var handler = new UpdatePersonHandler(context);

            var matchingId = context.People.FirstOrDefault(p => p.Name == "MyOldName");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);            

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(matchingId.Id));
        }

        [Test]
        public void UpdatePersonHandler_InvalidOriginalName_ThrowsException()
        {
            // Arrange
            var command = new UpdatePerson { OriginalName = "NonExistent", NewName = "NewName" };
            using var context = new StargateContext(_contextOptions);
            var handler = new UpdatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"There was an issue getting {command.OriginalName}'s record. Please contact support."));
        }        

        [Test]
        public void UpdatePersonHandler_NullRequest_ThrowsException()
        {
            // Arrange
            UpdatePerson command = null;
            using var context = new StargateContext(_contextOptions);
            var handler = new UpdatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Request invalid"));
        }

        [Test]
        public void UpdatePersonHandler_EmptyOriginalName_ThrowsException()
        {
            // Arrange
            var command = new UpdatePerson { OriginalName = "", NewName = "NewName" };
            using var context = new StargateContext(_contextOptions);
            var handler = new UpdatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Request invalid"));
        }

        [Test]
        public void UpdatePersonHandler_EmptyNewName_ThrowsException()
        {
            // Arrange
            var command = new UpdatePerson { OriginalName = "ExistingName", NewName = "" };
            using var context = new StargateContext(_contextOptions);
            var handler = new UpdatePersonHandler(context);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("Request invalid"));
        }
    }
}
