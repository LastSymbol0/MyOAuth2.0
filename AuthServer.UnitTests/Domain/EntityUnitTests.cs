using AuthServer.Domain.Abstractions;
using AuthServer.Domain.AggregatesModel.SessionAggregate;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AuthServer.UnitTests.Domain
{
    public class EntityUnitTests
    {
        Entity RO1 = new ResourceOwner(1);
        Entity RO2 = new ResourceOwner(2);
        Entity Client = new Client();

        Mock NotEntity = new Mock<EntityUnitTests>();

        [Fact]
        public void Equals_Should_Return_true_In_case_of_Same_object()
        {
            Assert.True(RO1.Equals(RO1));
            Assert.True(RO2.Equals(RO2));
            Assert.True(Client.Equals(Client));
        }

        [Fact]
        public void Equals_Should_Return_false_In_case_of_Differet_object_types()
        {
            Assert.False(RO1.Equals(Client));
            Assert.False(Client.Equals(RO1));
        }

        [Fact]
        public void Equals_Should_Return_false_In_case_of_Differet_object_data()
        {
            Assert.False(RO1.Equals(RO2));
            Assert.False(RO2.Equals(RO1));
        }

        [Fact]
        public void Equals_Should_Return_false_In_case_of_Null_object()
        {
            Assert.False(RO1.Equals(null));
            Assert.False(RO2.Equals(null));
            Assert.False(Client.Equals(null));
        }

        [Fact]
        public void Equals_Should_Return_false_In_case_of_NonEntity_object()
        {
            Assert.False(RO1.Equals(NotEntity));
            Assert.False(RO2.Equals(NotEntity));
            Assert.False(Client.Equals(NotEntity));
        }

        [Fact]
        public void EqualsOperator_Should_Return_false_In_case_of_Null_object()
        {
            Assert.True(null == null);
            Assert.False(RO1 == null);
            Assert.False(null == RO1);
        }
    }
}
