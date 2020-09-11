using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Mediator.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthServer.UnitTests.Commands
{
    public class StartSessionCommandHandlerUnitTest
    {
        Mock<ISessionRepository> SessionRepositoryMock;
        StartSessionCommandHandler Handler;

        public StartSessionCommandHandlerUnitTest()
        {
            SessionRepositoryMock = new Mock<ISessionRepository>();
            Handler = new StartSessionCommandHandler(SessionRepositoryMock.Object);
        }

        [Fact]
        public async Task TestHandler_Expect_True()
        {
            const int sessionId = 21;
            SessionRepositoryMock.Setup(s => s.GetSessionById(It.IsAny<int>())).Returns(null as Session);

            var resp = await Handler.Handle(new StartSessionCommand { ResourceOwnerId = "1", ClientId = "2", ClientSecret = "Secret", AccessParameters = new AccessParameters()});

            SessionRepositoryMock.Verify(s =>
                s.GetSessionById(
                    It.Is<int>(id => id == sessionId)),
                    Times.Once);

            Assert.True(resp.IsSucceed);
            Assert.Equal(resp.SessionId, sessionId);
        }


        [Fact]
        public async Task TestHandler_Expect_False()
        {
            const int sessionId = 21;
            SessionRepositoryMock.Setup(s => s.GetSessionById(It.IsAny<int>())).Returns(new Session());

            var resp = await Handler.Handle(new StartSessionCommand { ResourceOwnerId = "1", ClientId = "2", ClientSecret = "Secret", AccessParameters = new AccessParameters() });

            SessionRepositoryMock.Verify(s =>
                s.GetSessionById(
                    It.Is<int>(id => id == sessionId)),
                    Times.Once);

            Assert.False(resp.IsSucceed);
            Assert.Null(resp.SessionId);
        }
    }
}
