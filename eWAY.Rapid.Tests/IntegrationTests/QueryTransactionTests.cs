using System;
using System.Linq;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Internals.Services;
using eWAY.Rapid.Models;
using Xunit;
using Moq;

namespace eWAY.Rapid.Tests.IntegrationTests
{
    
    public class QueryTransactionTests : SdkTestBase
    {
        [Fact]
        public async Task  QueryTransaction_ByTransactionId_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();

            //Act
            var response = await client.Create(PaymentMethod.Direct, transaction);
            var filter = new TransactionFilter() {TransactionID = response.TransactionStatus.TransactionID};
            var queryResponse = await client.QueryTransaction(filter);
            var queryResponse2 = await client.QueryTransaction(response.TransactionStatus.TransactionID);
            //Assert
            Assert.NotNull(queryResponse);
            Assert.NotNull(queryResponse2);
            Assert.Equal(response.TransactionStatus.TransactionID, queryResponse.TransactionStatus.TransactionID);
            Assert.Equal(response.TransactionStatus.TransactionID, queryResponse2.TransactionStatus.TransactionID);
            Assert.Equal(response.TransactionStatus.Total, queryResponse2.TransactionStatus.Total);
            //TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
            //    queryResponse.Transaction.Customer);
            //TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Transaction.Customer,
            //    queryResponse.Transaction.Customer);
            //TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
            //    queryResponse2.Transaction.Customer);
            //TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Transaction.Customer,
            //    queryResponse2.Transaction.Customer);
        }
        [Fact]
        public async Task  QueryTransaction_ByAccessCode_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();

            //Act
            var response = await client.Create(PaymentMethod.TransparentRedirect, transaction);
            var filter = new TransactionFilter() {AccessCode = response.AccessCode};
            var queryResponse = await client.QueryTransaction(filter);
            var queryResponse2 = await client.QueryTransaction(response.AccessCode);
            //Assert
            Assert.NotNull(queryResponse);
            Assert.NotNull(queryResponse2);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                queryResponse.Transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                queryResponse2.Transaction.Customer);
        }
        [Fact]
        public async Task  QueryTransaction_ByInvoiceRef_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();
            var r = new Random();
            var randomInvoiceRef = r.Next(100000, 999999);
            transaction.PaymentDetails.InvoiceReference = randomInvoiceRef.ToString();
            //Act
            var response = await client.Create(PaymentMethod.Direct, transaction);
            var filter = new TransactionFilter()
            {
                InvoiceReference = response.Transaction.PaymentDetails.InvoiceReference
            };
            var queryResponse = await client.QueryTransaction(filter);
            var queryResponse2 = await client.QueryInvoiceRef(response.Transaction.PaymentDetails.InvoiceReference);
            //Assert
            Assert.NotNull(queryResponse);
            Assert.Equal(response.TransactionStatus.TransactionID, queryResponse.TransactionStatus.TransactionID);
            Assert.NotNull(queryResponse2);
            Assert.Equal(response.TransactionStatus.TransactionID, queryResponse2.TransactionStatus.TransactionID);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                queryResponse.Transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                queryResponse2.Transaction.Customer);
        }
        [Fact]
        public async Task  QueryTransaction_ByInvoiceNumber_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();
            var r = new Random();
            var randomInvoiceNumber = r.Next(10000, 99999);
            transaction.PaymentDetails.InvoiceNumber = "Inv " + randomInvoiceNumber;
            //Act
            var response = await client.Create(PaymentMethod.Direct, transaction);
            var filter = new TransactionFilter()
            {
                InvoiceNumber = response.Transaction.PaymentDetails.InvoiceNumber
            };
            var queryResponse = await client.QueryTransaction(filter);
            var queryResponse2 = await client.QueryInvoiceNumber(response.Transaction.PaymentDetails.InvoiceNumber);
            //Assert
            Assert.NotNull(queryResponse);
            Assert.Equal(response.TransactionStatus.TransactionID, queryResponse.TransactionStatus.TransactionID);
            Assert.NotNull(queryResponse2);
            Assert.Equal(response.TransactionStatus.TransactionID, queryResponse2.TransactionStatus.TransactionID);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                queryResponse.Transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                queryResponse2.Transaction.Customer);
        }

        [Fact]
        public async Task  QueryTransaction_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var filter = new TransactionFilter()
            {
                TransactionID = -1
            };
            //Act
            var queryByIdResponse = await client.QueryTransaction(filter);
            //Assert
            Assert.NotNull(queryByIdResponse.Errors);
            Assert.Equal(queryByIdResponse.Errors.FirstOrDefault(), "S9995");

            //Arrange
            filter = new TransactionFilter()
            {
                AccessCode = "leRandomAccessCode"
            };
            //Act
            var queryByAccessCodeResponse = await client.QueryTransaction(filter);
            //Assert
            Assert.Null(queryByAccessCodeResponse.Transaction);

            //Arrange
            filter = new TransactionFilter()
            {
                InvoiceNumber = "leRandomInvoiceNumber"
            };
            //Act
            var queryByInvoiceNumberResponse = await client.QueryTransaction(filter);
            //Assert
            Assert.NotNull(queryByInvoiceNumberResponse.Errors);
            Assert.Equal(queryByInvoiceNumberResponse.Errors.FirstOrDefault(), "V6171");

            //Arrange
            filter = new TransactionFilter()
            {
                InvoiceReference = "leRandomInvoiceReference"
            };
            //Act
            var queryByInvoiceRefResponse = await client.QueryTransaction(filter);
            //Assert
            Assert.NotNull(queryByInvoiceRefResponse.Errors);
            Assert.Equal(queryByInvoiceRefResponse.Errors.FirstOrDefault(), "V6171");
        }

        [Fact]
        public async Task  QueryTransaction_Rapidv40_Test()
        {
            if (GetVersion() > 31)
            {
                var client = CreateRapidApiClient();
                //Arrange
                var transaction = TestUtil.CreateTransaction();

                //Act
                var response = await client.Create(PaymentMethod.Direct, transaction);
                var filter = new TransactionFilter() { TransactionID = response.TransactionStatus.TransactionID };
                var queryResponse = await client.QueryTransaction(filter);
                var queryResponse2 = await client.QueryTransaction(response.TransactionStatus.TransactionID);
                //Assert
                Assert.NotNull(queryResponse);
                Assert.NotNull(queryResponse2);
                Assert.Equal(response.TransactionStatus.TransactionID, queryResponse.TransactionStatus.TransactionID);
                Assert.Equal(response.TransactionStatus.TransactionID, queryResponse2.TransactionStatus.TransactionID);
                Assert.Equal(response.TransactionStatus.Total, queryResponse2.TransactionStatus.Total);

                Assert.Equal("036", queryResponse2.Transaction.CurrencyCode);
                Assert.Equal(response.TransactionStatus.Total, queryResponse2.Transaction.MaxRefund);
            }
        }
    }
}
