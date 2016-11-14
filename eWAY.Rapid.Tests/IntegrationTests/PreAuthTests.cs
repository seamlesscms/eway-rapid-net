using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Models;
using Xunit;

namespace eWAY.Rapid.Tests.IntegrationTests
{

    public class PreAuthTests : SdkTestBase
    {
        [Fact]
        public async Task PreAuth_CapturePayment_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            var preAuthTransaction = await client.Create(PaymentMethod.Direct, transaction);
            //Act
            var preAuthRequest = new CapturePaymentRequest()
            {
                Payment = new Payment()
                {
                    CurrencyCode = preAuthTransaction.Transaction.PaymentDetails.CurrencyCode,
                    InvoiceDescription = preAuthTransaction.Transaction.PaymentDetails.InvoiceDescription,
                    InvoiceNumber = preAuthTransaction.Transaction.PaymentDetails.InvoiceNumber,
                    InvoiceReference = preAuthTransaction.Transaction.PaymentDetails.InvoiceReference,
                    TotalAmount = preAuthTransaction.Transaction.PaymentDetails.TotalAmount
                },
                TransactionId = preAuthTransaction.TransactionStatus.TransactionID.ToString()
            };
            var preAuthResponse = await client.CapturePayment(preAuthRequest);
            //Assert
            Assert.NotNull(preAuthResponse);
            Assert.True(preAuthResponse.TransactionStatus);
            Assert.NotNull(preAuthResponse.ResponseMessage);
            Assert.NotNull(preAuthResponse.ResponseCode);
            Assert.NotNull(preAuthResponse.TransactionID);
        }

        [Fact]
        public async Task PreAuth_CancelAuthorisation_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction(false);
            var preAuthTransaction = await client.Create(PaymentMethod.Direct, transaction);
            //Act
            var preAuthRequest = new CancelAuthorisationRequest()
            {
                TransactionId = preAuthTransaction.TransactionStatus.TransactionID.ToString()
            };

            var preAuthResponse = await client.CancelAuthorisation(preAuthRequest);
            //Assert
            Assert.NotNull(preAuthResponse);
            Assert.True(preAuthResponse.TransactionStatus);
            Assert.NotNull(preAuthResponse.ResponseMessage);
            Assert.NotNull(preAuthResponse.ResponseCode);
            Assert.NotNull(preAuthResponse.TransactionID);
        }
    }
}
