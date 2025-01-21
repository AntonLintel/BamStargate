using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Stargate.Server.Business.Commands;
using Stargate.Server.Business.Queries;
using Stargate.Server.Controllers;
using System.Net;

namespace Stargate.Tests
{
    [TestFixture]
    public class PersonControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private PersonController _controller;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PersonController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetPeople_ReturnsOk()
        {
            // Arrange
            var response = new GetPeopleResult { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPeople() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetPeople_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetPeople() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task GetPersonByName_ValidName_ReturnsOk()
        {
            // Arrange
            var name = "John Doe";
            var response = new GetPersonByNameResult { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPersonByName(name) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetPersonByName_InvalidName_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetPersonByName(null) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetPersonByName_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetPersonByName("John Doe") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task CreatePerson_ValidRequest_ReturnsOk()
        {
            // Arrange
            var name = "John Doe";
            var response = new CreatePersonResult { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.CreatePerson(name) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task CreatePerson_InvalidRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreatePerson(null) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreatePerson_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreatePerson("John Doe") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task UpdatePerson_ValidRequest_ReturnsOk()
        {
            // Arrange
            var originalName = "John Doe";
            var newName = "Jane Doe";
            var response = new UpdatePersonResult { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePerson>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdatePerson(new UpdatePerson { OriginalName = originalName, NewName = newName }) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task UpdatePerson_InvalidRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdatePerson(new UpdatePerson { OriginalName = null, NewName = "Jane Doe" }) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task UpdatePerson_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var originalName = "John Doe";
            var newName = "Jane Doe";
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePerson>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdatePerson(new UpdatePerson { OriginalName = originalName, NewName = newName }) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }
    }
}
