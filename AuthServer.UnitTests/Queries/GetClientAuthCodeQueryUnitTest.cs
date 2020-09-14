using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Mediator.Queries;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthServer.UnitTests.Queries
{
    public class GetClientAuthCodeQueryUnitTest
    {
        Mock<ISessionRepository> SessionRepositoryMock;
        GetClientAuthCodeQueryHandler Handler;

        const int nullSessionId = 0;
        const int invalidSessionId = 1;
        const int openedSessionId = 2;
        const int validSessionId = 3;

        Mock<Session> invalidSession {get; set;}
        Mock<Session> openedSession {get; set;}
        Mock<Session> validSession { get; set; }

        public GetClientAuthCodeQueryUnitTest()
        {
            SessionRepositoryMock = new Mock<ISessionRepository>();
            Handler = new GetClientAuthCodeQueryHandler(SessionRepositoryMock.Object);

            Session nullSession = null;

            invalidSession = new Mock<Session>();
            invalidSession.Setup(s => s.Id).Returns(invalidSessionId);
            invalidSession.Setup(s => s.IsValid()).Returns(false);

            openedSession = new Mock<Session>();
            openedSession.Setup(s => s.IsValid()).Returns(true);
            openedSession.Object.OpenSession(openedSession.Object.ClientGrantValue);
            openedSession.Setup(s => s.Id).Returns(openedSessionId);

            validSession = new Mock<Session>(1, 2, new AccessParameters());
            validSession.Setup(s => s.IsValid()).Returns(true);
            validSession.Setup(s => s.Id).Returns(validSessionId);

            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == nullSessionId)))
                .Returns(nullSession);
            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == invalidSessionId)))
                .Returns(invalidSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == openedSessionId)))
                .Returns(openedSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == validSessionId)))
                .Returns(validSession.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_null_When_session_is_null()
        {
            string res = await Handler.Handle(new GetClientAuthCodeQuery { SessionId = nullSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == nullSessionId)), Times.Once);

            Assert.Null(res);
        }

        [Fact]
        public async Task Handle_Should_Return_null_When_session_is_invalid()
        {
            string res = await Handler.Handle(new GetClientAuthCodeQuery { SessionId = invalidSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == invalidSessionId)), Times.Once);
            invalidSession.Verify(s => s.IsValid(), Times.Once);

            Assert.Null(res);
        }


        [Fact]
        public async Task Handle_Should_Return_null_When_session_is_opened()
        {
            string res = await Handler.Handle(new GetClientAuthCodeQuery { SessionId = openedSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == openedSessionId)), Times.Once);
            openedSession.Verify(s => s.IsValid(), Times.Once);

            Assert.Null(res);
        }

        [Fact]
        public async Task Handle_Should_Return_grant_When_session_is_valid()
        {
            string res = await Handler.Handle(new GetClientAuthCodeQuery { SessionId = validSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == validSessionId)), Times.Once);
            validSession.Verify(s => s.IsValid(), Times.Once);

            Assert.Equal(validSession.Object.ClientGrantValue, res);
        }
    }
}
