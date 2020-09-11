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
    public class RefreshClientTokenCommandHandlerUnitTest
    {
        Mock<ISessionRepository> SessionRepositoryMock;
        RefreshClientTokenCommandHandler Handler;

        const int nullSessionId = 0;
        const int invalidSessionId = 1;
        const int canNotRefreshTokenSessionId = 2;
        const int notOpenedSessionId = 3;
        const int invalidClientIdSessionId = 4;
        const int validSessionId = 5;

        const int invalidClientId = 0;
        const int validClientId = 1;

        public RefreshClientTokenCommandHandlerUnitTest()
        {
            SessionRepositoryMock = new Mock<ISessionRepository>();
            Handler = new RefreshClientTokenCommandHandler(SessionRepositoryMock.Object);

            Session nullSession = null;

            Mock<Session> invalidSession = new Mock<Session>();
            invalidSession.Setup(s => s.Id).Returns(invalidSessionId);
            invalidSession.Setup(s => s.IsValid()).Returns(false);

            Mock<Session> canNotRefreshTokenSession = new Mock<Session>();
            canNotRefreshTokenSession.Setup(s => s.IsValid()).Returns(true);
            canNotRefreshTokenSession.Setup(s => s.IsOpen()).Returns(true);
            canNotRefreshTokenSession.Setup(s => s.RefreshToken(It.IsAny<string>())).Returns(false);
            canNotRefreshTokenSession.Setup(s => s.ClientId).Returns(validClientId);
            canNotRefreshTokenSession.Setup(s => s.Id).Returns(canNotRefreshTokenSessionId);

            Mock<Session> notOpenedSession = new Mock<Session>();
            notOpenedSession.Setup(s => s.IsValid()).Returns(true);
            notOpenedSession.Setup(s => s.IsOpen()).Returns(false);
            notOpenedSession.Setup(s => s.Id).Returns(notOpenedSessionId);

            Mock<Session> invalidClientIdSession = new Mock<Session>();
            invalidClientIdSession.Setup(s => s.IsValid()).Returns(true);
            invalidClientIdSession.Setup(s => s.IsOpen()).Returns(true);
            invalidClientIdSession.Setup(s => s.ClientId).Returns(invalidClientId);
            invalidClientIdSession.Setup(s => s.Id).Returns(invalidClientIdSessionId);

            Mock<Session> validSession = new Mock<Session>();
            validSession.Setup(s => s.IsValid()).Returns(true);
            validSession.Setup(s => s.IsOpen()).Returns(true);
            validSession.Setup(s => s.RefreshToken(It.IsAny<string>())).Returns(true);
            validSession.Setup(s => s.ClientId).Returns(validClientId);
            validSession.Setup(s => s.Id).Returns(validSessionId);

            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.refresh_token), It.Is<string>(token => token == "0")))
                .Returns(nullSession);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.refresh_token), It.Is<string>(token => token == "1")))
                .Returns(invalidSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.refresh_token), It.Is<string>(token => token == "2")))
                .Returns(canNotRefreshTokenSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.refresh_token), It.Is<string>(token => token == "3")))
                .Returns(notOpenedSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.refresh_token), It.Is<string>(token => token == "4")))
                .Returns(invalidClientIdSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionByGrant(
                It.Is<GrantType>(c => c == GrantType.refresh_token), It.Is<string>(token => token == "5")))
                .Returns(validSession.Object);

            SessionRepositoryMock.Setup(r => r.RefreshToken(It.Is<int>(i => i == canNotRefreshTokenSessionId), It.IsAny<string>()))
                .ReturnsAsync(false);
        }

        [Theory]
        [InlineData("5")]
        public async Task TestHandler_Expect_True(string token)
        {
            RefreshClientTokenCommand command = new RefreshClientTokenCommand { RefreshToken = token, ClientId = validClientId.ToString() };
            int sessionId = int.Parse(token);

            SessionRepositoryMock.Setup(ctx =>
                ctx.RefreshToken(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable();

            var responce = await Handler.Handle(command);

            SessionRepositoryMock.Verify(ctx =>
                ctx.GetSessionByGrant(
                    It.Is<GrantType>(i => i == GrantType.refresh_token),
                    It.Is<string>(i => i == command.RefreshToken)),
                    Times.Once);

            SessionRepositoryMock.Verify(ctx =>
                ctx.RefreshToken(
                    It.Is<int>(i => i == validSessionId),
                    It.Is<string>(i => i == command.RefreshToken)),
                    Times.Once);

            Assert.True(responce.IsSucceed);
            Assert.Equal(responce.SessionId, sessionId);
        }


        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        public async Task TestHandler_Expect_False(string token)
        {
            RefreshClientTokenCommand command = new RefreshClientTokenCommand { RefreshToken = token, ClientId = validClientId.ToString() };

            int sessionId = int.Parse(token);

            var responce = await Handler.Handle(command);

            SessionRepositoryMock.Verify(ctx =>
                ctx.GetSessionByGrant(
                    It.Is<GrantType>(i => i == GrantType.refresh_token),
                    It.Is<string>(i => i == command.RefreshToken)),
                    Times.Once);

            if (sessionId == canNotRefreshTokenSessionId)
            {

                SessionRepositoryMock.Verify(ctx =>
                    ctx.RefreshToken(
                        It.Is<int>(i => i == sessionId),
                        It.Is<string>(i => i == command.RefreshToken)),
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
