using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class DaybookTransactionsTest : TestBase<DaybookTransactions>
    {
        public override DaybookTransactions CreateService(RestClient client) => new(client);

        private string daybookId = default!;
        private string accountId = default!;
        private string contraAccountId = default!;

        [TestInitialize]
        public void InitializeTestData()
        {
            daybookId = new Daybooks(Client).List(new { organizationId = OrganizationId })?.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No daybook found in the organisation");

            var accounts = new Accounts(Client).List()
                ?? throw new InvalidOperationException("Failed to list accounts");

            accountId = accounts.FirstOrDefault(a => a.AccountNo >= 1000 && a.AccountNo <= 1999)?.Id
                ?? throw new InvalidOperationException("No account found with account number between 1000 and 1999");

            contraAccountId = accounts.FirstOrDefault(a => a.AccountNo >= 4000 && a.AccountNo <= 9999)?.Id
                ?? throw new InvalidOperationException("No account found with account number between 4000 and 9999");
        }

        private DaybookTransaction BuildDraftTransaction() => new()
        {
            OrganizationId = OrganizationId,
            DaybookId = daybookId,
            EntryDate = DateTime.Now,
            State = "draft",
            Lines =
            [
                new DaybookTransactionLine
                {
                    AccountId = accountId,
                    ContraAccountId = contraAccountId,
                    Amount = 100,
                    Side = "debit",
                    Text = "Test line"
                }
            ]
        };

        [TestMethod]
        public void Get()
        {
            var id = service.Create(BuildDraftTransaction());
            try
            {
                var result = service.Get(id);
                Assert.AreEqual(id, result.Id);
            }
            finally
            {
                service.Delete(id);
            }
        }

        [TestMethod]
        public void List()
        {
            var result = service.List(new { organizationId = OrganizationId });
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
        {
            var id = service.Create(BuildDraftTransaction());
            service.Delete(id);
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void Delete()
        {
            var id = service.Create(BuildDraftTransaction());
            var result = service.Delete(id);
            Assert.IsNotNull(result);
        }
    }
}
