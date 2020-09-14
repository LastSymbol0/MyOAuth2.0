using AuthServer.Controllers;
using AuthServer.Mediator;
using AuthServer.Mediator.Commands;
using AuthServer.Mediator.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthServer.UnitTests.Controllers
{
    public class ClientControllerUnitTest
    {
        Mock<IMediator> MediatorMock;
        ClientController Controller;

        const string validBase64Creds = "bG9naW46cGFzcw=="; // login:pass
        AuthenticateClientCommand AuthenticateClientCommand;
        RefreshClientTokenCommand RefreshClientTokenCommand;

        public ClientControllerUnitTest()
        {
            MediatorMock = new Mock<IMediator>();
            Controller = new ClientController(MediatorMock.Object);

            AuthenticateClientCommand = new AuthenticateClientCommand { AuthenticationCode = "code", ClientId = "login", ClientSecret = "pass" };
            RefreshClientTokenCommand = new RefreshClientTokenCommand { RefreshToken = "token", ClientId = "login", ClientSecret = "pass" };
        }

        [Fact]
        public async Task GetToken_Should_return_bad_request_When_passed_invalid_grantType()
        {
            var resp = await Controller.GetToken("invalid grant type", "token", "url", "code", "Basic "+validBase64Creds);

            Assert.IsType(typeof(BadRequestObjectResult), resp.Result);
        }

        [Theory]
        [InlineData("authorization_code")]
        [InlineData("refresh_token")]
        public async Task GetToken_Should_return_bad_request_When_mediator_return_failure(string grantType)
        {
            if (grantType == "authorization_code")
            {
                MediatorMock.Setup(
                    m => m.Execute<AuthenticateClientCommand, SessionCommandResponce>(
                        It.IsAny<AuthenticateClientCommand>()))
                    .ReturnsAsync(new SessionCommandResponce(false, null));
            }
            else if (grantType == "refresh_token")
            {
                MediatorMock.Setup(
                    m => m.Execute<RefreshClientTokenCommand, SessionCommandResponce>(
                        It.IsAny<RefreshClientTokenCommand>()))
                    .ReturnsAsync(new SessionCommandResponce(false, null));
            }
            else
            {
                Assert.True(false);
            }

            var resp = await Controller.GetToken(grantType, "token", "url", "code", "Basic " + validBase64Creds);

            Assert.IsType(typeof(BadRequestObjectResult), resp.Result);

            if (grantType == "authorization_code")
            {
                MediatorMock.Verify(
                    m => m.Execute<AuthenticateClientCommand, SessionCommandResponce>(
                        It.IsAny<AuthenticateClientCommand>()),
                    Times.Once);
            }
            else if (grantType == "refresh_token")
            {
                MediatorMock.Verify(
                    m => m.Execute<RefreshClientTokenCommand, SessionCommandResponce>(
                        It.IsAny<RefreshClientTokenCommand>()),
                    Times.Once);
            }
        }

        [Theory]
        [InlineData("authorization_code")]
        [InlineData("refresh_token")]
        public async void GetToken_Should_return_ok_When_mediator_return_succeed(string grantType)
        {
            if (grantType == "authorization_code")
            {
                MediatorMock.Setup(
                    m => m.Execute<AuthenticateClientCommand, SessionCommandResponce>(
                        It.IsAny<AuthenticateClientCommand>()))
                    .ReturnsAsync(new SessionCommandResponce(true, 1));
            }
            else if (grantType == "refresh_token")
            {
                MediatorMock.Setup(
                    m => m.Execute<RefreshClientTokenCommand, SessionCommandResponce>(
                        It.IsAny<RefreshClientTokenCommand>()))
                    .ReturnsAsync(new SessionCommandResponce(true, 1));
            }
            else
            {
                Assert.True(false);
            }

            GetTokenPairQueryDTO tokenPair = new GetTokenPairQueryDTO
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                AccessTokenExpirationDate = DateTime.Now
            };

            MediatorMock.Setup(
                m => m.Get<GetTokenPairQuery, GetTokenPairQueryDTO>(
                    It.IsAny<GetTokenPairQuery>()))
                .ReturnsAsync(tokenPair);

            var resp = await Controller.GetToken(grantType, "token", "url", "code", "Basic " + validBase64Creds);

            Assert.IsType(typeof(OkObjectResult), resp.Result);

            if (grantType == "authorization_code")
            {
                MediatorMock.Verify(
                    m => m.Execute<AuthenticateClientCommand, SessionCommandResponce>(
                        It.IsAny<AuthenticateClientCommand>()),
                    Times.Once);
            }
            else if (grantType == "refresh_token")
            {
                MediatorMock.Verify(
                    m => m.Execute<RefreshClientTokenCommand, SessionCommandResponce>(
                        It.IsAny<RefreshClientTokenCommand>()),
                    Times.Once);
            }
            MediatorMock.Verify(
                m => m.Get<GetTokenPairQuery, GetTokenPairQueryDTO>(
                    It.IsAny<GetTokenPairQuery>()),
                    Times.Once);

            Assert.Equal(tokenPair, (resp.Result as OkObjectResult).Value);
        }


        [Fact]
        public async Task Auth_Should_return_nothing()
        {
            Controller.Auth();
        }
    }
}
