using eWAY.Rapid.Enums;
using eWAY.Rapid.Internals;
using eWAY.Rapid.Internals.Enums;
using eWAY.Rapid.Internals.Request;
using eWAY.Rapid.Internals.Response;
using eWAY.Rapid.Internals.Services;
using Xunit;
using Moq;

namespace eWAY.Rapid.Tests.UnitTests
{

    public class CreateTransactionTests
    {
        [Fact]
        public void CreateTransaction_Direct_CaptureTrue_InvokeDirectPayment()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.DirectPayment(It.IsAny<DirectPaymentRequest>())).ReturnsAsync(new DirectPaymentResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.Direct, transaction);
            //Assert
            mockRapidApiClient.Verify();
        }

        [Fact]
        public void CreateTransaction_ResponsiveShared_CaptureTrue_TokenNo_InvokeCreateAccessCodeShared()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCodeShared(It.IsAny<CreateAccessCodeSharedRequest>())).ReturnsAsync(new CreateAccessCodeSharedResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.ResponsiveShared, transaction);
            //Assert
            mockRapidApiClient.Verify();
        }

        [Fact]
        public void CreateTransaction_ResponsiveShared_CaptureTrue_TokenYes_InvokeCreateAccessCodeShared_TokenPayment()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeSharedRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(true, "123123123");
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCodeShared(It.IsAny<CreateAccessCodeSharedRequest>()))
                .Callback<CreateAccessCodeSharedRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeSharedResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.ResponsiveShared, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.TokenPayment);
        }
        [Fact]
        public void CreateTransaction_TransparentRedirect_CaptureTrue_TokenNo_InvokeCreateAccessCode()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCode(It.IsAny<CreateAccessCodeRequest>())).ReturnsAsync(new CreateAccessCodeResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            mockRapidApiClient.Verify();
        }
        [Fact]
        public void CreateTransaction_TransparentRedirect_CaptureTrue_TokenYes_InvokeCreateAccessCode_TokenPayment()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(true, "123123123");
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCode(It.IsAny<CreateAccessCodeRequest>()))
                .Callback<CreateAccessCodeRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.TokenPayment);
        }
        [Fact]
        public void CreateTransaction_TransparentRedirect_CreateTokenTrue_InvokeCreateAccessCode_TokenPayment()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(true, null, true);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCode(It.IsAny<CreateAccessCodeRequest>()))
                .Callback<CreateAccessCodeRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(Method.TokenPayment, assertRequest.Method);
        }

        [Fact]
        public void CreateTransaction_Authorisation_InvokeDirectAuthorisation()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.DirectAuthorisation(It.IsAny<DirectAuthorisationRequest>())).ReturnsAsync(new DirectAuthorisationResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.Authorisation, transaction);
            //Assert
            mockRapidApiClient.Verify();
        }
        [Fact]
        public void CreateTransaction_Direct_CaptureFalse_InvokeDirectPayment_MethodAuthorise()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            DirectPaymentRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.DirectPayment(It.IsAny<DirectPaymentRequest>()))
                .Callback<DirectPaymentRequest>(i => assertRequest = i)
                .ReturnsAsync(new DirectPaymentResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.Direct, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.Authorise);
        }
        [Fact]
        public void CreateTransaction_ResponsiveShared_CaptureFalse_InvokeCreateAccessCodeShared_MethodAuthorise()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeSharedRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCodeShared(It.IsAny<CreateAccessCodeSharedRequest>()))
                .Callback<CreateAccessCodeSharedRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeSharedResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.ResponsiveShared, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.Authorise);
        }
        [Fact]
        public void CreateTransaction_TransparentRedirect_CaptureFalse_InvokeCreateAccessCode_MethodAuthorise()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            CreateAccessCodeRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.CreateAccessCode(It.IsAny<CreateAccessCodeRequest>()))
                .Callback<CreateAccessCodeRequest>(i => assertRequest = i)
                .ReturnsAsync(new CreateAccessCodeResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.Authorise);
        }

        [Fact]
        public void CreateTransaction_Wallet_CaptureTrue_InvokeDirectPayment_MethodProcessPayment()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            DirectPaymentRequest assertRequest = null;
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            transaction.SecuredCardData = "123123123";
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.DirectPayment(It.IsAny<DirectPaymentRequest>()))
                .Callback<DirectPaymentRequest>(i => assertRequest = i)
                .ReturnsAsync(new DirectPaymentResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.Wallet, transaction);
            //Assert
            mockRapidApiClient.Verify();
            Assert.NotNull(assertRequest);
            Assert.Equal(assertRequest.Method, Method.ProcessPayment);
        }

        [Fact]
        public void CreateTransaction_Wallet_CaptureFalse_InvokeDirectAuthorisation()
        {
            var mockRapidApiClient = new Mock<IRapidService>();
            var rapidSdkClient = new RapidClient(mockRapidApiClient.Object);
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            transaction.SecuredCardData = "123123123";
            mockRapidApiClient.Setup(x => x.IsValid()).Returns(true);
            mockRapidApiClient.Setup(x => x.DirectAuthorisation(It.IsAny<DirectAuthorisationRequest>())).ReturnsAsync(new DirectAuthorisationResponse()).Verifiable();
            //Act
            rapidSdkClient.Create(PaymentMethod.Wallet, transaction);
            //Assert
            mockRapidApiClient.Verify();
        }
    }
}
