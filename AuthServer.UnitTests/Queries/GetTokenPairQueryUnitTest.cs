using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Mediator.Queries;
using AuthServer.Models;
using AuthServer.Service;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthServer.UnitTests.Queries
{
    public class GetTokenPairQueryUnitTest
    {
        Mock<ITokenManager> TokenManagerMock;
        Mock<IMapper> MapperMock;
        Mock<ISessionRepository> SessionRepositoryMock;
        GetTokenPairQueryHandler Handler;

        const int nullSessionId = 0;
        const int invalidSessionId = 1;
        const int notOpenedSessionId = 2;
        const int validSessionId = 3;

        Mock<Session> invalidSession {get; set;}
        Mock<Session> notOpenedSession {get; set;}
        Mock<Session> validSession { get; set; }
        AccessParameters accessParameters = new AccessParameters();

        public GetTokenPairQueryUnitTest()
        {
            SessionRepositoryMock = new Mock<ISessionRepository>();
            TokenManagerMock = new Mock<ITokenManager>();
            MapperMock = new Mock<IMapper>();
            Handler = new GetTokenPairQueryHandler(SessionRepositoryMock.Object, TokenManagerMock.Object, MapperMock.Object);

            Session nullSession = null;

            invalidSession = new Mock<Session>();
            invalidSession.Setup(s => s.Id).Returns(invalidSessionId);
            invalidSession.Setup(s => s.IsValid()).Returns(false);

            notOpenedSession = new Mock<Session>();
            notOpenedSession.Setup(s => s.IsValid()).Returns(true);
            notOpenedSession.Setup(s => s.IsOpen()).Returns(false);
            notOpenedSession.Setup(s => s.Id).Returns(notOpenedSessionId);

            validSession = new Mock<Session>(1, 2, accessParameters);
            validSession.Setup(s => s.IsValid()).Returns(true);
            validSession.Setup(s => s.IsOpen()).Returns(true);
            validSession.Setup(s => s.Id).Returns(validSessionId);

            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == nullSessionId)))
                .Returns(nullSession);
            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == invalidSessionId)))
                .Returns(invalidSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == notOpenedSessionId)))
                .Returns(notOpenedSession.Object);
            SessionRepositoryMock.Setup(r => r.GetSessionById(
                It.Is<int>(c => c == validSessionId)))
                .Returns(validSession.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_null_When_session_is_null()
        {
            GetTokenPairQueryDTO res = await Handler.Handle(new GetTokenPairQuery { SessionId = nullSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == nullSessionId)), Times.Once);

            Assert.Null(res);
        }

        [Fact]
        public async Task Handle_Should_Return_null_When_session_is_invalid()
        {
            GetTokenPairQueryDTO res = await Handler.Handle(new GetTokenPairQuery { SessionId = invalidSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == invalidSessionId)), Times.Once);
            invalidSession.Verify(s => s.IsValid(), Times.Once);

            Assert.Null(res);
        }


        [Fact]
        public async Task Handle_Should_Return_null_When_session_is_notOpened()
        {
            GetTokenPairQueryDTO res = await Handler.Handle(new GetTokenPairQuery { SessionId = notOpenedSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == notOpenedSessionId)), Times.Once);
            notOpenedSession.Verify(s => s.IsValid(), Times.Once);

            Assert.Null(res);
        }

        [Fact]
        public async Task Handle_Should_Return_grant_When_session_is_valid()
        {
            IEnumerable<Claim> claims = new List<Claim>();
            AccessTokenDTO accessToken = new AccessTokenDTO
            {
                AccessToken = "validAccessToken",
                ExpirationDate = DateTime.UtcNow
            };

            MapperMock.Setup(
                m => m.Map<IEnumerable<Claim>>(
                    It.Is<AccessParameters>(i => i == accessParameters)))
                .Returns(claims);

            TokenManagerMock.Setup(
                t => t.GenerateAccessToken(
                    It.Is<IEnumerable<Claim>>(i => i == claims)))
                .Returns(accessToken);


            GetTokenPairQueryDTO res = await Handler.Handle(new GetTokenPairQuery { SessionId = validSessionId });

            SessionRepositoryMock.Verify(r => r.GetSessionById(It.Is<int>(i => i == validSessionId)), Times.Once);
            validSession.Verify(s => s.IsValid(), Times.Once);
            validSession.Verify(s => s.IsOpen(), Times.Once);

            MapperMock.Verify(
                m => m.Map<IEnumerable<Claim>>(
                    It.Is<AccessParameters>(i => i == accessParameters)),
                Times.Once);

            TokenManagerMock.Verify(
                t => t.GenerateAccessToken(
                    It.Is<IEnumerable<Claim>>(i => i == claims)),
                Times.Once);

            Assert.Equal(validSession.Object.ClientGrantValue, res.RefreshToken);
            Assert.Equal(accessToken.AccessToken, res.AccessToken);
            Assert.Equal(accessToken.ExpirationDate, res.AccessTokenExpirationDate);
        }
    }
}
