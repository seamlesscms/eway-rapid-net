using System.Linq;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using Xunit;

namespace eWAY.Rapid.Tests.IntegrationTests
{

    public class UpdateCustomerTests : SdkTestBase
    {
        [Fact]
        public async Task Customer_UpdateCustomerDirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();
            var createResponse = await client.Create(PaymentMethod.Direct, customer);
            customer.TokenCustomerID = createResponse.Customer.TokenCustomerID;
            //Act
            var updateResponse = await client.UpdateCustomer(PaymentMethod.Direct, customer);

            //Assert
            Assert.Equal(createResponse.Customer.TokenCustomerID, updateResponse.Customer.TokenCustomerID);
        }

        [Fact]
        public async Task Customer_UpdateCustomerTransparentRedirect_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();
            var createResponse = await client.Create(PaymentMethod.TransparentRedirect, customer);
            customer.TokenCustomerID = createResponse.Customer.TokenCustomerID;
            //Act
            var updateResponse = await client.UpdateCustomer(PaymentMethod.Direct, customer);

            //Assert
            Assert.Equal(createResponse.Customer.TokenCustomerID, updateResponse.Customer.TokenCustomerID);
        }

        [Fact]
        public async Task Customer_UpdateCustomerResponsiveShared_ReturnValidData()
        {
            var client = CreateRapidApiClient();
            //Arrange
            var customer = TestUtil.CreateCustomer();
            var createResponse = await client.Create(PaymentMethod.TransparentRedirect, customer);
            customer.TokenCustomerID = createResponse.Customer.TokenCustomerID;
            //Act
            var updateResponse = await client.UpdateCustomer(PaymentMethod.ResponsiveShared, customer);

            //Assert
            Assert.Equal(createResponse.Customer.TokenCustomerID, updateResponse.Customer.TokenCustomerID);
        }
    }
}
