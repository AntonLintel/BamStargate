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
    public class AstronautDutyControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private AstronautDutyController _controller;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AstronautDutyController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetAstronautDutiesByName_ValidName_ReturnsOk()
        {
            // Arrange
            var name = "John Doe";
            var response = new GetAstronautDutiesByNameResult { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAstronautDutiesByName>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAstronautDutiesByName(name) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAstronautDutiesByName_InvalidName_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAstronautDutiesByName(null) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task AssignAstronautDuty_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new AssignAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = DateTime.Now };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignAstronautDuty>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new AssignAstronautDutyResult { Success = true });

            // Act
            var result = await _controller.AssignAstronautDuty(request) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task AssignAstronautDuty_InvalidRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AssignAstronautDuty(null) as BadRequestResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task AssignAstronautDuty_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var request = new AssignAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignAstronautDuty>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AssignAstronautDuty(request) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }
    }
}