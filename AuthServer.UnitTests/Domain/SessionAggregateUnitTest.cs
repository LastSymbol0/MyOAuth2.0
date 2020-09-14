using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using Xunit;

namespace AuthServer.UnitTests.Domain
{
    public class SessionAggregateUnitTest
    {
        const int ValidClientId = 1;
        const int ValidResourceOwnerId = 2;
        AccessParameters ValidAccessParameters = new AccessParameters(new List<ScopeAccess>
                {
                    new ScopeAccess{ ScopeName = "Scope1Access", HasAccess = true },
                    new ScopeAccess{ ScopeName = "Scope2Access", HasAccess = false },
                    new ScopeAccess{ ScopeName = "Scope3Access", HasAccess = true },
                    new ScopeAccess{ ScopeName = "Scope4Access", HasAccess = false }
                })
            ;
        //object[] InvalidSessionData = new object[] { 1, 2, null as AccessParameters};

        Session ValidStartedSession;

        public SessionAggregateUnitTest()
        {
            ValidStartedSession = new Session(ValidClientId, ValidResourceOwnerId, ValidAccessParameters);
        }


            /*** Session create ***/

        [Fact]
        public void TestSession_Creating_Success()
        {
            Session session = new Session(ValidClientId, ValidResourceOwnerId, ValidAccessParameters);

            Assert.Equal(ValidClientId, session.ClientId);
            Assert.Equal(ValidResourceOwnerId, session.ResourceOwnerId);
            Assert.Equal(ValidAccessParameters, session.AccessParameters);
            Assert.Equal(int.Parse($"{ValidClientId}{ValidResourceOwnerId}"), session.Id);
            Assert.Equal(SessionStatus.WaitingForClientAuthorization, session.Status);
            Assert.Equal(GrantType.code, session.ClientGrantType);
            Assert.NotNull(session.ClientGrantValue);
            Assert.True(DateTime.Compare(session.ExpireIn, DateTime.UtcNow) > 0);

            Assert.True(session.IsValid());
            Assert.True(int.TryParse(session.ClientGrantValue, out int n));
            Assert.False(session.IsOpen());
        }

        [Fact]
        public void TestSession_Creating_Failure()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Session session = new Session(ValidClientId, ValidResourceOwnerId, null as AccessParameters);
            });
        }

            /*** Session open ***/

        [Fact]
        public void TestSession_Open_Success()
        {
            var code = ValidStartedSession.ClientGrantValue;

            bool isSucceed = ValidStartedSession.OpenSession(code);

            Assert.True(isSucceed);
            Assert.True(ValidStartedSession.IsValid());
            Assert.True(ValidStartedSession.IsOpen());
            Assert.False(ValidStartedSession.ClientGrantValue.Contains(' '));
            Assert.Equal(GrantType.refresh_token, ValidStartedSession.ClientGrantType);
        }

        [Fact]
        public void TestSession_Open_Failure_InvalidStatus()
        {
            // precondition [
            bool isSucceed = ValidStartedSession.OpenSession(ValidStartedSession.ClientGrantValue);
            Assert.True(isSucceed);
            // ]

            var code = ValidStartedSession.ClientGrantValue;
            isSucceed = ValidStartedSession.OpenSession(code);

            Assert.False(isSucceed);
        }

        [Fact]
        public void TestSession_Open_Failure_CodeExpired()
        {
            // precondition [
            ValidStartedSession.ExpireIn =  DateTime.UtcNow.AddSeconds(-5);
            // ]

            var code = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.OpenSession(code);

            Assert.False(isSucceed);
            Assert.False(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
            Assert.Equal(GrantType.code, ValidStartedSession.ClientGrantType);
        }

        [Fact]
        public void TestSession_Open_Failure_CodeInvalid()
        {
            var code = "invalidcode";

            bool isSucceed = ValidStartedSession.OpenSession(code);

            Assert.False(isSucceed);
            Assert.True(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
            Assert.Equal(GrantType.code, ValidStartedSession.ClientGrantType);
        }


            /*** Session close ***/

        [Fact]
        public void TestSession_Close_Success_Opened()
        {
            // precondition [
            var code = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.OpenSession(code);
            Assert.True(isSucceed);
            // ]

            isSucceed = ValidStartedSession.CloseSession();

            Assert.True(isSucceed);
            Assert.False(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
            Assert.Equal(GrantType.None, ValidStartedSession.ClientGrantType);
            Assert.Equal(SessionStatus.Closed, ValidStartedSession.Status);
        }

        [Fact]
        public void TestSession_Close_Success_WaitingForClientAuthorization()
        {
            bool isSucceed = ValidStartedSession.CloseSession();

            Assert.True(isSucceed);

            Assert.False(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
            Assert.Equal(GrantType.None, ValidStartedSession.ClientGrantType);
            Assert.Equal(SessionStatus.Closed, ValidStartedSession.Status);
        }

        [Fact]
        public void TestSession_Close_Failure_Closed()
        {
            // precondition [
            bool isSucceed = ValidStartedSession.CloseSession();
            Assert.True(isSucceed);
            // ]

            isSucceed = ValidStartedSession.CloseSession();

            Assert.False(isSucceed);
            Assert.False(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
            Assert.Equal(GrantType.None, ValidStartedSession.ClientGrantType);
            Assert.Equal(SessionStatus.Closed, ValidStartedSession.Status);
        }

        [Fact]
        public void TestSession_Close_Failure_StatusNone()
        {
            Session ValidStartedSession = new Session();

            bool isSucceed = ValidStartedSession.CloseSession();

            Assert.False(isSucceed);

            Assert.False(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
            Assert.Equal(GrantType.None, ValidStartedSession.ClientGrantType);
            Assert.Equal(SessionStatus.None, ValidStartedSession.Status);
        }


            /*** Session refresh token ***/

        [Fact]
        public void TestSession_RefreshToken_Success()
        {
            // precondition [
            var code = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.OpenSession(code);
            Assert.True(isSucceed);
            // ]

            var refreshToken = ValidStartedSession.ClientGrantValue;
            isSucceed = ValidStartedSession.RefreshToken(refreshToken);
            Assert.True(isSucceed);

            Assert.NotEqual(refreshToken, ValidStartedSession.ClientGrantValue);
            Assert.Equal(GrantType.refresh_token, ValidStartedSession.ClientGrantType);

            Assert.True(ValidStartedSession.IsValid());
            Assert.True(ValidStartedSession.IsOpen());
        }

        [Fact]
        public void TestSession_RefreshToken_Failure_InvalidToken()
        {
            // precondition [
            var code = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.OpenSession(code);
            Assert.True(isSucceed);
            // ]

            var refreshToken = "invalidToken";
            isSucceed = ValidStartedSession.RefreshToken(refreshToken);
            Assert.False(isSucceed);

            Assert.Equal(GrantType.refresh_token, ValidStartedSession.ClientGrantType);

            Assert.True(ValidStartedSession.IsValid());
            Assert.True(ValidStartedSession.IsOpen());
        }

        [Fact]
        public void TestSession_RefreshToken_Failure_InvalidSessionState()
        {
            var refreshToken = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.RefreshToken(refreshToken);
            Assert.False(isSucceed);

            Assert.NotEqual(GrantType.refresh_token, ValidStartedSession.ClientGrantType);

            Assert.True(ValidStartedSession.IsValid());
            Assert.False(ValidStartedSession.IsOpen());
        }

        /*** Session validate grant ***/
        [Fact]
        public void TestSession_ValidateGrant_Success_AuthCode()
        {
            var code = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.ValidateGrant(GrantType.code, code);
            Assert.True(isSucceed);

            Assert.Equal(GrantType.code, ValidStartedSession.ClientGrantType);
            Assert.Equal(code, ValidStartedSession.ClientGrantValue);
        }

        [Fact]
        public void TestSession_ValidateGrant_Success_RefreshToken()
        {
            // precondition [
            bool isSucceed = ValidStartedSession.OpenSession(ValidStartedSession.ClientGrantValue);
            Assert.True(isSucceed);
            // ]

            var token = ValidStartedSession.ClientGrantValue;
            isSucceed = ValidStartedSession.ValidateGrant(GrantType.refresh_token, token);
            Assert.True(isSucceed);

            Assert.Equal(GrantType.refresh_token, ValidStartedSession.ClientGrantType);
            Assert.Equal(token, ValidStartedSession.ClientGrantValue);
        }

        [Theory]
        [InlineData(GrantType.None)]
        [InlineData(GrantType.refresh_token)]
        [InlineData(GrantType.code)]
        public void TestSession_ValidateGrant_Failure_GrantTypeInvalid(GrantType invalidGrantType)
        {
            // precondition [
            if (invalidGrantType == GrantType.code)
            {
                bool isSucceedOpen = ValidStartedSession.OpenSession(ValidStartedSession.ClientGrantValue);
                Assert.True(isSucceedOpen);
            }
            // ]

            var grantValue = ValidStartedSession.ClientGrantValue;
            bool isSucceed = ValidStartedSession.ValidateGrant(invalidGrantType, grantValue);
            Assert.False(isSucceed);

            Assert.NotEqual(invalidGrantType, ValidStartedSession.ClientGrantType);
            Assert.Equal(grantValue, ValidStartedSession.ClientGrantValue);
        }


        [Theory]
        [InlineData(GrantType.refresh_token)]
        [InlineData(GrantType.code)]
        public void TestSession_ValidateGrant_Failure_GrantValueInvalid(GrantType validGrantType)
        {
            // precondition [
            if (validGrantType == GrantType.refresh_token)
            {
                bool isSucceedOpen = ValidStartedSession.OpenSession(ValidStartedSession.ClientGrantValue);
                Assert.True(isSucceedOpen);
            }
            // ]

            var grantValue = "invalidGrantVal";
            bool isSucceed = ValidStartedSession.ValidateGrant(validGrantType, grantValue);
            Assert.False(isSucceed);

            Assert.Equal(validGrantType, ValidStartedSession.ClientGrantType);
            Assert.NotEqual(grantValue, ValidStartedSession.ClientGrantValue);
        }
    }
}
