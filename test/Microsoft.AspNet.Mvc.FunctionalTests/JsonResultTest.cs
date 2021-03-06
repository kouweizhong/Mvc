// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class JsonResultTest
    {
        private const string SiteName = nameof(BasicWebSite);
        private readonly Action<IApplicationBuilder> _app = new BasicWebSite.Startup().Configure;
        private readonly Action<IServiceCollection> _configureServices = new BasicWebSite.Startup().ConfigureServices;

        [Theory]
        [InlineData("application/json")]
        [InlineData("text/json")]
        public async Task JsonResult_Conneg(string mediaType)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/Plain";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Accept", mediaType);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(mediaType, response.Content.Headers.ContentType.MediaType);
            Assert.Equal("{\"Message\":\"hello\"}", content);
        }

        // Using an Accept header can't force Json to not be Json. If your accept header doesn't jive with the
        // formatters/content-type configured on the result it will be ignored.
        [Theory]
        [InlineData("application/xml")]
        [InlineData("text/xml")]
        public async Task JsonResult_Conneg_Fails(string mediaType)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/Plain";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Accept", mediaType);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("{\"Message\":\"hello\"}", content);
        }

        // If the object is null, it will get formatted as JSON. NOT as a 204/NoContent
        [Fact]
        public async Task JsonResult_Null()
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/Null";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("null", content);
        }

        // If the object is a string, it will get formatted as JSON. NOT as text/plain.
        [Fact]
        public async Task JsonResult_String()
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/String";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("\"hello\"", content);
        }

        [Theory]
        [InlineData("application/json")]
        [InlineData("text/json")]
        public async Task JsonResult_CustomFormatter_Conneg(string mediaType)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/CustomFormatter";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Accept", mediaType);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(mediaType, response.Content.Headers.ContentType.MediaType);
            Assert.Equal("{\"message\":\"hello\"}", content);
        }

        // Using an Accept header can't force Json to not be Json. If your accept header doesn't jive with the
        // formatters/content-type configured on the result it will be ignored.
        [Theory]
        [InlineData("application/xml")]
        [InlineData("text/xml")]
        public async Task JsonResult_CustomFormatter_Conneg_Fails(string mediaType)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/CustomFormatter";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Accept", mediaType);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("{\"message\":\"hello\"}", content);
        }

        [Fact]
        public async Task JsonResult_Uses_CustomSerializerSettings()
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/CustomSerializerSettings";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("{\"message\":\"hello\"}", content);
        }

        [Fact]
        public async Task JsonResult_CustomContentType()
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();

            var url = "http://localhost/JsonResult/CustomContentType";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/message+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("{\"Message\":\"hello\"}", content);
        }
    }
}