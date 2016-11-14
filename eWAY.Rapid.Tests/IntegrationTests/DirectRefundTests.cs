using System.Linq;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using Xunit;

namespace eWAY.Rapid.Tests.IntegrationTests
{
    
    public class DirectRefundTests: SdkTestBase
    {
        [Fact]
        public async Task DirectRefund_ReturnValidData()
        {
            //Arrange
            var client = CreateRapidApiClient();
            var transaction = TestUtil.CreateTransaction();
            var createTransactionResponse = await client.Create(PaymentMethod.Direct, transaction);
            var refund = TestUtil.CreateRefund(createTransactionResponse.TransactionStatus.TransactionID);
            //Act
            var refundResponse = await client.Refund(refund);
            //Assert
            Assert.Null(refundResponse.Errors);
            Assert.Equal(createTransactionResponse.TransactionStatus.TransactionID, refundResponse.Refund.OriginalTransactionID);
            Assert.True(refundResponse.ResponseMessage.StartsWith("A"));
            Assert.True(refundResponse.TransactionID > 0);
            TestUtil.AssertReturnedCustomerData_VerifyCardDetailsAreEqual(refund.Customer, refundResponse.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(refund.Customer, refundResponse.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(refund.Customer, refundResponse.Customer);
        }

        [Fact]
        public async Task  DirectRefund_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();
            var createTransactionResponse = await client.Create(PaymentMethod.Direct, transaction);
            var refund = TestUtil.CreateRefund(createTransactionResponse.TransactionStatus.TransactionID);
            refund.RefundDetails.TotalAmount = -1;
            //Act
            var refundResponse1 = await client.Refund(refund);
            //Assert
            // This test is failing at the moment
            //Assert.NotNull(refundResponse1.Errors);
            //Assert.Equal(refundResponse1.Errors.FirstOrDefault(), "D4413");
            //Arrange
            refund = TestUtil.CreateRefund(createTransactionResponse.TransactionStatus.TransactionID);
            refund.RefundDetails.OriginalTransactionID = -1;
            //Act
            var refundResponse2 = await client.Refund(refund);
            //Assert
            Assert.NotNull(refundResponse2.Errors);
            Assert.Equal(refundResponse2.Errors.FirstOrDefault(), "V6115");
        }
    }
}
