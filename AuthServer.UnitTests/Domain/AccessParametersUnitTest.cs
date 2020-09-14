using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using AuthServer.Domain.AggregatesModel.SessionAggregate;

namespace AuthServer.UnitTests.Domain
{
    public class AccessParametersUnitTest
    {
        IList<ScopeAccess> Scopes = new List<ScopeAccess>
            {
                new ScopeAccess{ScopeName = "Scope1Access", HasAccess = true },
                new ScopeAccess{ScopeName = "Scope2Access", HasAccess = false },
                new ScopeAccess{ScopeName = "Scope3Access", HasAccess = true }
            };

        [Fact]
        public void TestAccessParameters_EmptyConstructor()
        {
            var accessParameters = new AccessParameters();

            Assert.NotNull(accessParameters.Scopes);
            Assert.False(accessParameters.Scopes.Any());
        }

        [Fact]
        public void TestAccessParameters_ScopesConstructor()
        {
            var accessParameters = new AccessParameters(Scopes);

            Assert.NotNull(accessParameters.Scopes);
            Assert.Equal(Scopes, accessParameters.Scopes);
        }

        [Fact]
        public void TestAccessParameters_ScopesNullConstructor()
        {
            var accessParameters = new AccessParameters(null);

            Assert.Null(accessParameters.Scopes);
        }

        [Theory]
        [InlineData("Scope1Access", true)]
        [InlineData("Scope1Access", false)]
        [InlineData("Scope2Access", null)]
        [InlineData("Scope4Access", true)]
        [InlineData("Scope4Access", false)]
        [InlineData("Scope4Access", null)]
        public void TestAccessParameters_AddScopeAccess(string scope, bool? hasAccess)
        {
            var accessParameters = new AccessParameters(Scopes);

            if (hasAccess != null)
            {
                accessParameters.AddScopeAccess(scope, (bool)hasAccess);
            }
            else
            {
                accessParameters.AddScopeAccess(scope);
            }

            bool expectedAccess = hasAccess != null ? (bool)hasAccess : true;
            bool actualAccess = accessParameters.Scopes.FirstOrDefault(i => i.ScopeName == scope).HasAccess;

            Assert.Equal(expectedAccess, actualAccess);
        }

        [Theory]
        [InlineData("Scope1Access")]
        [InlineData("Scope2Access")]
        [InlineData("Scope4Access")]
        public void TestAccessParameters_HasAccess(string scope)
        {
            var accessParameters = new AccessParameters(Scopes);

            bool expectedAccess = Scopes.FirstOrDefault(i => i.ScopeName == scope)?.HasAccess ?? false;
            bool actualAccess = accessParameters.HasAccess(scope);

            Assert.Equal(expectedAccess, actualAccess);
        }
    }

    public class ScopeAccessUnitTest
    {
        [Theory]
        [InlineData("ScopeName:True", "ScopeName", true)]
        [InlineData("ScopeName:true", "ScopeName", true)]
        [InlineData("ScopeName:False", "ScopeName", false)]
        [InlineData("ScopeName:false", "ScopeName", false)]
        public void TestScopeAccess_FromStringConstructor_Success(string str, string scope, bool access)
        {
            var scopeAccess = new ScopeAccess(str);

            Assert.Equal(scope, scopeAccess.ScopeName);
            Assert.Equal(access, scopeAccess.HasAccess);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("ScopeName:invalidBool")]
        [InlineData("invalidFormat")]
        public void TestScopeAccess_FromStringConstructor_Failure(string str)
        {
            if (str == null)
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    var scopeAccess = new ScopeAccess(str);
                });
            }
            else
            {
                Assert.Throws<FormatException>(() =>
                {
                    var scopeAccess = new ScopeAccess(str);
                });
            }
        }

        [Theory]
        [InlineData("ScopeName:True", "ScopeName", true)]
        [InlineData("ScopeName:False", "ScopeName", false)]
        public void TestScopeAccess_ToString(string str, string scope, bool access)
        {
            var scopeAccess = new ScopeAccess { ScopeName = scope, HasAccess = access };

            Assert.Equal(str, scopeAccess.ToString());
        }
    }
}