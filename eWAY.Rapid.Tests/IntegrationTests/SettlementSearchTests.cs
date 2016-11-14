using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Models;
using Xunit;

namespace eWAY.Rapid.Tests.IntegrationTests
{

    public class SettlementSearchTests : SdkTestBase
    {
        [Fact]
        public async Task SettlementSearch_ByDate_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange

            //Act
            var settlementSearch = new SettlementSearchRequest() { ReportMode = SettlementSearchMode.Both, SettlementDate = "2016-02-01" };
            var settlementResponse = await client.SettlementSearch(settlementSearch);

            //Assert
            Assert.NotNull(settlementResponse);
        }

        [Fact]
        public async Task SettlementSearch_ByDateRange_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange

            //Act
            var settlementSearch = new SettlementSearchRequest()
            {
                ReportMode = SettlementSearchMode.Both,
                StartDate = "2016-02-01",
                EndDate = "2016-02-08",
                CardType = CardType.ALL,
            };
            var settlementResponse = await client.SettlementSearch(settlementSearch);

            //Assert
            Assert.NotNull(settlementResponse);
            Assert.True(settlementResponse.SettlementTransactions.Length > 1);
            Assert.True(settlementResponse.SettlementSummaries.Length > 1);
        }

        [Fact]
        public async Task SettlementSearch_WithPage_Test()
        {
            var client = CreateRapidApiClient();
            //Arrange

            //Act
            var settlementSearch = new SettlementSearchRequest()
            {
                ReportMode = SettlementSearchMode.TransactionOnly,
                SettlementDate = "2016-02-01",
                Page = 1,
                PageSize = 5
            };
            var settlementResponse = await client.SettlementSearch(settlementSearch);

            //Assert
            Assert.NotNull(settlementResponse);
            Assert.True(settlementResponse.SettlementTransactions.Length < 6);
        }

    }
}
