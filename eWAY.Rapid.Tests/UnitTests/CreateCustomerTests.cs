using eWAY.Rapid.Enums;
using eWAY.Rapid.Internals;
using eWAY.Rapid.Internals.Enums;
using eWAY.Rapid.Internals.Request;
using eWAY.Rapid.Internals.Response;
using eWAY.Rapid.Internals.Services;
using eWAY.Rapid.Tests.IntegrationTests;
using Xunit;
using Moq;

namespace eWAY.Rapid.Tests.UnitTests
{
    
    public class CreateCustomerTests: SdkTestBase
    {
        [Fact]
        public void CreateCustomer_Direct_InvokeDirectPayment_MethodCreateTokenCustomer()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            DirectPaymentRequest assertRequest = null;
            //Arrange
            var customer = TestUtil.CreateCustomer();
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.DirectPayment(It.IsAny<DirectPaymentRequest>()))
                .Callback<DirectPaymentRequest>(i => assertRequest = i)
                .ReturnsAsync(new DirectPaymentResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.Direct, customer);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.CreateTokenCustomer);
        }

        [Fact]
        public void CreateCustomer_TransparentRedirect_InvokeCreateAccessCode_MethodCreateTokenCustomer()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeRequest assertRequest = null;
            //Arrange
            var customer = TestUtil.CreateCustomer();
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCode(It.IsAny<CreateAccessCodeRequest>()))
                .Callback<CreateAccessCodeRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.TransparentRedirect, customer);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.CreateTokenCustomer);
        }
        [Fact]
        public void CreateCustomer_ResponsiveShared_InvokeCreateAccessCodeShared_MethodCreateTokenCustomer()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeSharedRequest assertRequest = null;
            //Arrange
            var customer = TestUtil.CreateCustomer();
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCodeShared(It.IsAny<CreateAccessCodeSharedRequest>()))
                .Callback<CreateAccessCodeSharedRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeSharedResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.ResponsiveShared, customer);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.CreateTokenCustomer);
        }
    }
}
