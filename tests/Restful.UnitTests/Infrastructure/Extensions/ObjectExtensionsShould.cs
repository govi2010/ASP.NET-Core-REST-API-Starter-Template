﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Restful.Core.Entities.Milk;
using Restful.Infrastructure.Extensions;
using Xunit;

namespace Restful.UnitTests.Infrastructure.Extensions
{
    public class ObjectExtensionsShould
    {
        private readonly Mock<Product> _mockProduct;

        public ObjectExtensionsShould()
        {
            _mockProduct = new Mock<Product>();
        }

        [Fact]
        public void ThrowExceptionWhenSourceNull()
        {
            Product source = null;
            Assert.Throws<ArgumentNullException>(() => source.ToDynamic());
        }

        [Fact]
        public void HasAllPropertiesWhenFieldsNull()
        {
            var expando = _mockProduct.Object.ToDynamic();
            var propertyInfos = typeof(Product).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            Assert.NotNull(expando);

            foreach (var propertyInfo in propertyInfos)
            {
                Assert.True(((IDictionary<string, object>)expando).Keys.Contains(propertyInfo.Name));
            }
        }

        [Theory]
        [InlineData("Id, Name, PackingType")]
        [InlineData("id,Name,PackingType")]
        [InlineData("id, name, PACKINGTYPE")]
        [InlineData("ID,NAME,PACKINGTYPE")]
        [InlineData("id, name, packingType")]
        [InlineData("Id,name,packingType")]
        public void TrueWhenFieldsValidCaseSensitive(string fields)
        {
            var expando = _mockProduct.Object.ToDynamic(fields);

            Assert.True(((IDictionary<string, object>)expando).Keys.Contains("Id"));
            Assert.True(((IDictionary<string, object>)expando).Keys.Contains("Name"));
            Assert.True(((IDictionary<string, object>)expando).Keys.Contains("PackingType"));
        }

        [Theory]
        [InlineData("Id, Name, PackingType", "QuantityPerBox")]
        [InlineData("id,Name,PackingType", "OrderUnit")]
        [InlineData("id, name, PACKINGTYPE", "MinimumOrderUnitQuantity")]
        [InlineData("ID,NAME,PACKINGTYPE", "UnitPrice")]
        [InlineData("id, name, packingType", "UnitPrice")]
        [InlineData("Id,name,packingType", "UnitPrice")]
        public void FailWhenPropertyNotInFields(string fields, string property)
        {
            var expando = _mockProduct.Object.ToDynamic(fields);

            Assert.False(((IDictionary<string, object>)expando).Keys.Contains(property));
        }

        [Theory]
        [InlineData("Id, Name, Packin_gType")]
        [InlineData("inimumOrderUnitQuantity, PackingType")]
        [InlineData("id1,Name,PackingType")]
        [InlineData("id, name_, PACKINGTYPE")]
        [InlineData("ID,NAME,_PACKINGTYPE")]
        [InlineData("id, name'packingType")]
        [InlineData("Idname,packingType")]
        [InlineData("UnitPrice,name`MinimumOrderUnitQuantity,")]
        [InlineData(",UnitPrice,name;MinimumOrderUnitQuantity,")]
        public void ThrowExceptionWhenInvalidFields(string fields)
        {
            Assert.Throws<Exception>(() => _mockProduct.Object.ToDynamic(fields));
        }

    }
}
