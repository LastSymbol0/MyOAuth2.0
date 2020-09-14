using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AuthServer.UnitTests.Domain
{
    public class ResourceOwnerUnitTest
    {
        [Fact]
        public void TestResourceOwner_Constructor()
        {
            const int id = 2134;

            var owner = new ResourceOwner(id);

            Assert.Equal(id, owner.Id);
        }
    }
}
