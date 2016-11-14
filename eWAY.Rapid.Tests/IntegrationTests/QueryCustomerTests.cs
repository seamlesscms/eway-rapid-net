using System.Linq;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using Xunit;

namespace eWAY.Rapid.Tests.IntegrationTests
{
    
    public class QueryCustomerTests : SdkTestBase
    {
        [Fact]
        public async Task  QueryCustomer_ByCustomerTokenId_Test()
        {
            //Arrange
            var client = CreateRapidApiClient();
            var customer = TestUtil.CreateCustomer();

            //Act
            var createCustomerResponse = await client.Create(PaymentMethod.Direct, customer);
            var customerId = long.Parse(createCustomerResponse.Customer.TokenCustomerID);
            var queryResponse = await client.QueryCustomer(customerId);

            //Assert
            Assert.NotNull(queryResponse);
            Assert.Equal(createCustomerResponse.Customer.TokenCustomerID, queryResponse.Customers.First().TokenCustomerID);
            //TestUtil.AssertReturnedCustomerData_VerifyAddressAreEqual(createCustomerResponse.Customer,
                //queryResponse.Customers.First());
            TestUtil.AssertReturnedCustomerData_VerifyAllFieldsAreEqual(createCustomerResponse.Customer,
                queryResponse.Customers.First());
            TestUtil.AssertReturnedCustomerData_VerifyCardDetailsAreEqual(createCustomerResponse.Customer,
                queryResponse.Customers.First());
        }

        [Fact]
        public async Task  QueryCustomer_ByCustomerTokenId_InvalidId_ReturnErrorV6040()
        {
            //Arrange
            var client = CreateRapidApiClient();
            
            //Act
            var customerId = -1;
            var queryResponse = await client.QueryCustomer(customerId);

            //Assert
            Assert.NotNull(queryResponse.Errors);
            Assert.Equal(queryResponse.Errors.FirstOrDefault(), "V6040");
        }
    }
}
