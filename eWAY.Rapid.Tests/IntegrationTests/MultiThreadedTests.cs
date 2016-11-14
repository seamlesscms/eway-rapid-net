using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eWAY.Rapid.Enums;
using eWAY.Rapid.Models;
using Xunit;
using Internal = eWAY.Rapid.Internals.Models;
namespace eWAY.Rapid.Tests.IntegrationTests
{
    public class MultiThreadedTests : SdkTestBase
    {
        [Fact]
        public void Multithread_CreatingMultipleClientsInManyThreads_ReturnsValidData()
        {
            RunInMultipleThreads(20, DoSingleUnitOfWork);


        }

        private async Task DoSingleUnitOfWork()
        {
            IRapidClient client = CreateRapidApiClient();
            Transaction transaction = new Transaction()
            {
                Customer = TestUtil.CreateCustomer(),
                PaymentDetails = new PaymentDetails()
                {
                    TotalAmount = 200
                },
                TransactionType = TransactionTypes.MOTO,


            };
            CreateTransactionResponse response = await client.Create(PaymentMethod.Direct, transaction);
        }



        private void RunInMultipleThreads(int threadCount, Func<Task> action)
        {
            List<Task> startedThreads = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                startedThreads.Add(action());
            }
            Task.WaitAll(startedThreads.ToArray());
        }
    }
}
