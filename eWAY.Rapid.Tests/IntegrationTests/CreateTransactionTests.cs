using System.Linq;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Models;
using Xunit;

namespace eWAY.Rapid.Tests.IntegrationTests
{
    public class CreateTransactionTests : SdkTestBase
    {
        [Fact]
        public async Task Transaction_CreateTransactionDirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();

            //Act
            var response = await client.Create(PaymentMethod.Direct, transaction);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.Transaction);
            Assert.NotNull(response.TransactionStatus);
            Assert.NotNull(response.TransactionStatus.Status);
            Assert.True(response.TransactionStatus.Status.Value);
            Assert.True(response.TransactionStatus.TransactionID > 0);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyCardDetailsAreEqual(response.Transaction.Customer,
                transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Transaction.Customer,
                transaction.Customer);
        }

        [Fact]
        public async Task Transaction_CreateTransactionTransparentRedirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();

            //Act
            var response = await client.Create(PaymentMethod.TransparentRedirect, transaction);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.AccessCode);
            Assert.NotNull(response.FormActionUrl);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Transaction.Customer,
                transaction.Customer);
        }

        [Fact]
        public async Task Transaction_CreateTokenTransactionTransparentRedirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();
            transaction.SaveCustomer = true;

            //Act
            var response = await client.Create(PaymentMethod.TransparentRedirect, transaction);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.AccessCode);
            Assert.NotNull(response.FormActionUrl);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Transaction.Customer,
                transaction.Customer);
        }

        [Fact]
        public async Task Transaction_CreateTransactionResponsiveShared_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction();

            // Responsive Shared Fields
            transaction.LogoUrl = "https://mysite.com/images/logo4eway.jpg";
            transaction.HeaderText = "My Site Header Text";
            transaction.CustomerReadOnly = true;
            transaction.CustomView = "bootstrap";
            transaction.VerifyCustomerEmail = false;
            transaction.VerifyCustomerPhone = false;

            //Act
            var response = await client.Create(PaymentMethod.ResponsiveShared, transaction);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.AccessCode);
            Assert.NotNull(response.FormActionUrl);
            Assert.NotNull(response.SharedPaymentUrl);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Transaction.Customer,
                transaction.Customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Transaction.Customer,
                transaction.Customer);
        }

        [Fact]
        public async Task Transaction_CreateTransactionDirect_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            transaction.Customer.CardDetails.Number = "-1";
            //Act
            var response1 = await client.Create(PaymentMethod.Direct, transaction);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.FirstOrDefault(), "V6110");
            //Arrange
            transaction = TestUtil.CreateTransaction(true);
            transaction.PaymentDetails.TotalAmount = -1;
            //Act
            var response2 = await client.Create(PaymentMethod.Direct, transaction);
            //Assert
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.FirstOrDefault(), "V6011");
        }

        [Fact]
        public async Task Transaction_CreateTransactionTransparentRedirect_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            transaction.PaymentDetails.TotalAmount = 0;
            //Act
            var response1 = await client.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.FirstOrDefault(), "V6011");
            //Arrange
            transaction = TestUtil.CreateTransaction(true);
            transaction.RedirectURL = "anInvalidRedirectUrl";
            //Act
            var response2 = await client.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.FirstOrDefault(), "V6059");
        }

        [Fact]
        public async Task Transaction_CreateTransactionResponsiveShared_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var transaction = TestUtil.CreateTransaction(true);
            transaction.PaymentDetails.TotalAmount = 0;
            //Act
            var response1 = await client.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.FirstOrDefault(), "V6011");
            //Arrange
            transaction = TestUtil.CreateTransaction(true);
            transaction.RedirectURL = "anInvalidRedirectUrl";
            //Act
            var response2 = await client.Create(PaymentMethod.TransparentRedirect, transaction);
            //Assert
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.FirstOrDefault(), "V6059");
        }
    }
}
