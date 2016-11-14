using System.Linq;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Models;
using Xunit;
using Internal = eWAY.Rapid.Internals.Models;
namespace eWAY.Rapid.Tests.IntegrationTests
{
    
    public class CreateCustomerTests : SdkTestBase
    {
        [Fact]
        public async Task Customer_CreateCustomerDirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();

            //Act
            var response = await client.Create(PaymentMethod.Direct, customer);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.Customer);
            Assert.NotNull(response.Customer.TokenCustomerID);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Customer, customer);
            TestUtil.AssertReturnedCustomerData_VerifyCardDetailsAreEqual(response.Customer, customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Customer, customer);
        }

        [Fact]
        public async Task  Customer_CreateCustomerTransparentRedirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();

            //Act
            var response = await client.Create(PaymentMethod.TransparentRedirect, customer);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.AccessCode);
            Assert.NotNull(response.FormActionUrl);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Customer, customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Customer, customer);
        }

        [Fact]
        public async Task  Customer_CreateCustomerResponsiveShared_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();

            //Act
            var response = await client.Create(PaymentMethod.ResponsiveShared, customer);

            //Assert
            Assert.Null(response.Errors);
            Assert.NotNull(response.AccessCode);
            Assert.NotNull(response.FormActionUrl);
            Assert.NotNull(response.SharedPaymentUrl);
            TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(response.Customer, customer);
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(response.Customer, customer);
        }

        [Fact]
        public async Task  Customer_CreateCustomerDirect_InvalidInputData_VerifyReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();
            customer.CardDetails.ExpiryYear = "-1";
            //Act
            var response1 = await client.Create(PaymentMethod.Direct, customer);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.FirstOrDefault(), "V6102");
            //Arrange
            customer = TestUtil.CreateCustomer();
            customer.Url = "anInvalidUrl";
            //Act
            var response2 = await client.Create(PaymentMethod.Direct, customer);
            //Assert
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.FirstOrDefault(), "V6074");
        }

        [Fact]
        public async Task  Customer_CreateCustomerTransparentRedirect_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();
            customer.FirstName = null;
            //Act
            var response1 = await client.Create(PaymentMethod.TransparentRedirect, customer);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.FirstOrDefault(), "V6042");
            //Arrange
            customer = TestUtil.CreateCustomer();
            customer.RedirectURL = "anInvalidRedirectUrl";
            //Act
            var response2 = await client.Create(PaymentMethod.TransparentRedirect, customer);
            //Assert
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.FirstOrDefault(), "V6059");
        }

        [Fact]
        public async Task  Customer_CreateCustomerResponsiveShared_InvalidInputData_ReturnVariousErrors()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();
            customer.LastName = null;
            //Act
            var response1 = await client.Create(PaymentMethod.ResponsiveShared, customer);
            //Assert
            Assert.NotNull(response1.Errors);
            Assert.Equal(response1.Errors.FirstOrDefault(), "V6043");
            //Arrange
            customer = TestUtil.CreateCustomer();
            customer.RedirectURL = "anInvalidRedirectUrl";
            //Act
            var response2 = await client.Create(PaymentMethod.ResponsiveShared, customer);
            //Assert
            Assert.NotNull(response2.Errors);
            Assert.Equal(response2.Errors.FirstOrDefault(), "V6059");
        }
    }
}
