using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Internals;
using eWAY.Rapid.Internals.Services;
using eWAY.Rapid.Models;
using eWAY.Rapid.Tests.IntegrationTests;
using Xunit;
using Moq;

namespace eWAY.Rapid.Tests.UnitTests
{
    public interface IHttpWebRequestFactory
    {
        HttpWebRequest Create(string uri);
    }

    public class MiscTests : SdkTestBase
    {
        [Fact]
        public async Task AuthenticationError_Test()
        {
            //Arrange
            var wr = new Mock<HttpWebResponse>();
            wr.Setup(c => c.StatusCode).Returns(HttpStatusCode.Unauthorized);
            var request = new Mock<HttpWebRequest>();
            request.Setup(c => c.GetResponseAsync()).ReturnsAsync(wr.Object);
            var transaction = TestUtil.CreateTransaction();
            var mockClient = new Mock<RapidService>(APIKEY, PASSWORD, ENDPOINT);
            var we = new WebException("MockException", null, WebExceptionStatus.ProtocolError, wr.Object);
            mockClient.Setup(x => x.GetWebResponse(It.IsAny<WebRequest>(), It.IsAny<string>())).Throws(we);
            var client = new RapidClient(mockClient.Object);
            //Act
            var response = await client.Create(PaymentMethod.Direct, transaction);
            //Assert
            Assert.True(response.Errors[0] == RapidSystemErrorCode.AUTHENTICATION_ERROR);
        }

        [Fact]
        public async Task CommunicationError_Test()
        {
            //Arrange
            var wr = new Mock<HttpWebResponse>();
            wr.Setup(c => c.StatusCode).Returns(HttpStatusCode.ServiceUnavailable);
            var request = new Mock<HttpWebRequest>();
            request.Setup(c => c.GetResponseAsync()).ReturnsAsync(wr.Object);
            var transaction = TestUtil.CreateTransaction();
            var mockClient = new Mock<RapidService>(APIKEY, PASSWORD, ENDPOINT);
            var we = new WebException("MockException", null, WebExceptionStatus.ProtocolError, wr.Object);
            mockClient.Setup(x => x.GetWebResponse(It.IsAny<WebRequest>(), It.IsAny<string>())).Throws(we);
            var client = new RapidClient(mockClient.Object);
            //Act
            var response = await client.Create(PaymentMethod.Direct, transaction);
            //Assert
            Assert.True(response.Errors[0] == RapidSystemErrorCode.COMMUNICATION_ERROR);
        }

        [Fact]
        public async Task SdkInvalidStateErrors_Test()
        {
            //Arrange
            var mockClient = new Mock<IRapidService>();
            mockClient.Setup(x => x.IsValid()).Returns(false);
            mockClient.Setup(x => x.GetErrorCodes()).Returns(new List<string>(new[] { RapidSystemErrorCode.INVALID_ENDPOINT_ERROR }));
            var transaction = TestUtil.CreateTransaction();
            var customer = TestUtil.CreateCustomer();
            var client = new RapidClient(mockClient.Object);
            //Act
            var response1 = await client.Create(PaymentMethod.Direct, transaction);
            var response2 = await client.Create(PaymentMethod.Direct, customer);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.First(), RapidSystemErrorCode.INVALID_ENDPOINT_ERROR);
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.First(), RapidSystemErrorCode.INVALID_ENDPOINT_ERROR);
        }

        [Fact]
        public async Task SdkInternalErrors_Test()
        {
            //Arrange
            var mockClient = new Mock<IRapidService>();
            mockClient.Setup(x => x.GetErrorCodes()).Returns(new List<string>(new[] { RapidSystemErrorCode.INTERNAL_SDK_ERROR }));
            var client = new RapidClient(mockClient.Object);
            var filter = new TransactionFilter()
            {
                TransactionID = 123,
                AccessCode = "abc",
                InvoiceNumber = "123",
                InvoiceReference = "123"
            };
            //Act
            var response = await client.QueryTransaction(filter);
            //Assert
            Assert.NotNull(response.Errors);
            Assert.Equal(response.Errors.First(), RapidSystemErrorCode.INTERNAL_SDK_ERROR);
        }

        [Fact]
        public void UserDisplayMessage_ReturnValidErrorMessage()
        {
            //Arrange
            var testMessage = "Invalid TransactionType, account not certified for eCome only MOTO or Recurring available";
            //Act
            var message = RapidClientFactory.UserDisplayMessage("V6010", "en");
            //Assert
            Assert.Equal(message, testMessage);
        }

        [Fact]
        public void UserDisplayMessage_ReturnInvalidErrorMessage()
        {
            //Arrange
            var testMessage = SystemConstants.INVALID_ERROR_CODE_MESSAGE;
            //Act
            var message = RapidClientFactory.UserDisplayMessage("blahblah", "en");
            //Assert
            Assert.Equal(message, testMessage);
        }

        [Fact]
        public void UserDisplayMessage_ReturnDefaultEnglishLanguage()
        {
            //Arrange
            var testMessage = "Invalid TransactionType, account not certified for eCome only MOTO or Recurring available";
            //Act
            var message1 = RapidClientFactory.UserDisplayMessage("V6010", "de");
            var message2 = RapidClientFactory.UserDisplayMessage("V6010", "blahblah");
            //Assert
            Assert.Equal(message1, testMessage);
            Assert.Equal(message2, testMessage);
        }
    }
}
