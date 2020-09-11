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
    public class AuthenticateClientCommandUnitTest
    {
        Mock<ISessionRepository> SessionRepositoryMock;
        AuthenticateClientCommandHandler Handler;

        const int nullSessionId = 0;
        const int invalidSessionId = 1;
        const int canNotOpenSessionId = 2;
        const int validSessionId = 3;

        public AuthenticateClientCommandUnitTest()
        {
            SessionRepositoryMock = new Mock<ISessionRepository>();
            Handler = new AuthenticateClientCommandHandler(SessionRepositoryMock.Object);

            Session nullSession = null;
            Mock<Session> invalidSession = new Mock<Session>();
            invalidSession.Setup(s => s.Id).Returns(invalidSessionId);
            invalidSession.Setup(s => s.IsValid()).Returns(false);
            Mock<Session> canNotOpenSession = new Mock<Session>();
            canNotOpenSession.Setup(s => s.IsValid()).Returns(true);
            canNotOpenSession.Setup(s => s.OpenSession(It.IsAny<string>())).Returns(false);
            canNotOpenSession.Setup(s => s.Id).Returns(canNotOpenSessionId);

            Mock<Session> validSession = new Mock<Session>();
            validSession.Setup(s => s.IsValid()).Returns(true);
            validSession.Setup(s => s.OpenSession(It.IsAny<string>())).Returns(true);
            validSession.Setup(s => s.Id).Returns(validSessionId);

            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.code), It.Is<string>(code => code == "0")))
                .Returns(nullSession);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.code), It.Is<string>(code => code == "1")))
                .Returns(invalidSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.code), It.Is<string>(code => code == "2")))
                .Returns(canNotOpenSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.code), It.Is<string>(code => code == "3")))
                .Returns(validSession.Object);

            SessionRepositoryMock.Setup(r => r.OpenSession(It.Is<int>(i => i == canNotOpenSessionId), It.IsAny<string>()))
                .ReturnsAsync(false);
        }

        [Theory]
        [InlineData("3")]
        public async Task TestHandler_Expect_True(string authCode)
        {
            AuthenticateClientCommand command = new AuthenticateClientCommand { AuthenticationCode = authCode };
            int sessionId = int.Parse(authCode);

            SessionRepositoryMock.Setup(ctx =>
                ctx.OpenSession(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable();

            var responce = await Handler.Handle(command);

            SessionRepositoryMock.Verify(ctx =>
                ctx.GetSessionByGrant(
                    It.Is<GrantType>(i => i == GrantType.code),
                    It.Is<string>(i => i == command.AuthenticationCode)),
                    Times.Once);

            SessionRepositoryMock.Verify(ctx =>
                ctx.OpenSession(
                    It.Is<int>(i => i == validSessionId),
                    It.Is<string>(i => i == command.AuthenticationCode)),
                    Times.Once);

            Assert.True(responce.IsSucceed);
            Assert.Equal(responce.SessionId, sessionId);
        }


        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        public async Task TestHandler_Expect_False(string authCode)
        {
            AuthenticateClientCommand command = new AuthenticateClientCommand { AuthenticationCode = authCode };

            int sessionId = int.Parse(authCode);

            var responce = await Handler.Handle(command);

            SessionRepositoryMock.Verify(ctx =>
                ctx.GetSessionByGrant(
                    It.Is<GrantType>(i => i == GrantType.code),
                    It.Is<string>(i => i == command.AuthenticationCode)),
                    Times.Once);

            if (sessionId == canNotOpenSessionId)
            {

                SessionRepositoryMock.Verify(ctx =>
                    ctx.OpenSession(
                        It.Is<int>(i => i == sessionId),
                        It.Is<string>(i => i == command.AuthenticationCode)),
                    Times.Once);
            }
            else
            {
                SessionRepositoryMock.Verify(ctx =>
                    ctx.OpenSession(
                        It.IsAny<int>(),
                        It.IsAny<string>()),
                    Times.Never);
            }

            Assert.False(responce.IsSucceed);
            if (sessionId == nullSessionId)
            {
                Assert.Null(responce.SessionId);
            }
            else
            {
                Assert.Equal(sessionId, responce.SessionId);
            }
        }
    }
}
