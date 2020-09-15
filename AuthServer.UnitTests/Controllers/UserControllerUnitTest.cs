using AuthServer.Controllers;
using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Mediator;
using AuthServer.Mediator.Commands;
using AuthServer.Mediator.Queries;
using AuthServer.Models;
using AuthServer.Models.GivingAccessPage;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AuthServer.UnitTests.Controllers
{
    public class UserControllerUnitTest
    {
        UserController UserController;
        Mock<IMediator> Mediator;
        Mock<IMapper> Mapper;

        const string validBase64Creds = "bG9naW46cGFzcw=="; // login:pass

        GivingAccessModel Model = new GivingAccessModel
        {
            ClientInfo = new RequestCodeClientDTO
            {
                ResouceOwnerId = "login",
                ClientId = "clientId",
                ResponceType = "responceType",
                RedirectUrl = "redirectUrl",
                State = "state"
            },
            AccessParameters = new AccessParametersDTO
            {
                Scope1Access = false,
                Scope2Access = true,
                Scope3Access = false,
                Scope4Access = true
            }
        };
        AccessParameters accessParametrs = new AccessParameters();

        public UserControllerUnitTest()
        {
            Mediator = new Mock<IMediator>();
            Mapper = new Mock<IMapper>();
            UserController = new UserController(Mediator.Object, Mapper.Object);
        }

        [Fact]
        public void GetAccessToClient_on_get_Should_return_view()
        {
            var resp = UserController.GetAccessToClient("clientId", "responceType", "redirectUrl", "state", "Basic "+validBase64Creds);

            Assert.IsType(typeof(ViewResult), resp);

            ViewResult viewResult = resp as ViewResult;

            Assert.IsType(typeof(GivingAccessModel), viewResult.Model);

            GivingAccessModel model = viewResult.Model as GivingAccessModel;

            Assert.Equal("clientId", model.ClientInfo.ClientId);
            Assert.Equal("responceType", model.ClientInfo.ResponceType);
            Assert.Equal("redirectUrl", model.ClientInfo.RedirectUrl);
            Assert.Equal("state", model.ClientInfo.State);
            Assert.Equal("login", model.ClientInfo.ResouceOwnerId);

            Assert.NotNull(model.AccessParameters);
        }


        [Fact]
        public void GetAccessToClient_on_post_Should_return_bad_request_When_mediator_return_failure()
        {
            Mediator.Setup(m => m.Execute<StartSessionCommand, SessionCommandResponce>(It.IsAny<StartSessionCommand>()))
                .ReturnsAsync(new SessionCommandResponce(false, null));

            Mapper.Setup(m => m.Map<AccessParameters>(It.IsAny<AccessParametersDTO>()))
                .Returns(accessParametrs);

            var resp = UserController.GetAccessToClient(Model);

            Mediator.Verify(m => m.Execute<StartSessionCommand, SessionCommandResponce>(It.Is<StartSessionCommand>(
                cmd => cmd.ClientId == Model.ClientInfo.ClientId
                && cmd.ResourceOwnerId == Model.ClientInfo.ResouceOwnerId
                && cmd.AccessParameters == accessParametrs)),
                Times.Once);

            Mediator.Verify(m => m.Get<GetClientAuthCodeQuery, string>(It.IsAny<GetClientAuthCodeQuery>()),
                Times.Never);

            Assert.IsType(typeof(BadRequestObjectResult), resp.Result);
        }

        [Fact]
        public void GetAccessToClient_on_post_Should_return_redirect_When_mediator_return_succees()
        {
            Mediator.Setup(m => m.Execute<StartSessionCommand, SessionCommandResponce>(It.IsAny<StartSessionCommand>()))
                .ReturnsAsync(new SessionCommandResponce(true, 11));

            Mapper.Setup(m => m.Map<AccessParameters>(It.IsAny<AccessParametersDTO>()))
                .Returns(accessParametrs);

            Mediator.Setup(m => m.Get<GetClientAuthCodeQuery, string>(It.IsAny<GetClientAuthCodeQuery>()))
                .ReturnsAsync("code");

            var resp = UserController.GetAccessToClient(Model);

            Mapper.Verify(m => m.Map<AccessParameters>(It.Is<AccessParametersDTO>(i => i == Model.AccessParameters)),
                Times.Once);

            Mediator.Verify(m => m.Execute<StartSessionCommand, SessionCommandResponce>(It.Is<StartSessionCommand>(
                cmd => cmd.ClientId == Model.ClientInfo.ClientId
                && cmd.ResourceOwnerId == Model.ClientInfo.ResouceOwnerId
                && cmd.AccessParameters == accessParametrs)),
                Times.Once);

            Mediator.Verify(m => m.Get<GetClientAuthCodeQuery, string>(It.Is<GetClientAuthCodeQuery>(q => q.SessionId == 11)),
                Times.Once);

            Assert.IsType(typeof(RedirectResult), resp.Result);

            //TODO: add redirection url query parameters validation 
        }
    }
}
