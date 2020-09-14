using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AuthServer.UnitTests.Domain
{
    public class ClientUnitTests
    {
        [Fact]
        public void TestClient_EmptyConstructor()
        {
            var client = new Client();

            Assert.Equal(0, client.Id);
            Assert.Equal(null, client.ClientSecret);
        }

        [Fact]
        public void TestClient_Constructor_Success()
        {
            const int id = 2134;
            const string secret = "2134";

            var client = new Client(id, secret);

            Assert.Equal(id, client.Id);
            Assert.Equal(secret, client.ClientSecret);
        }

        [Fact]
        public void TestClient_Constructor_Exception()
        {
            const int id = 2134;
            const string secret = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var client = new Client(id, secret);
            });
        }
    }
}
