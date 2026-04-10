using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class BillServiceTest : TestBase<Bills>
    {
        public override Bills CreateService(RestClient client) => new(client);

        [TestMethod]
        public void Get()
        {
            var id = service.Create(new Bill
            {
                OrganizationId = OrganizationId,
                ContactId = "GefjTensSluwEUkmmdBYCA",
                EntryDate = DateTime.Now,
                State = BillStates.draft,
                TaxMode = "incl",
                Lines = new List<BillLine>
                {
                    new BillLine
                    {
                        AccountId = "pdU7q2jETY6YehhE9ePR2g",
                        Amount = 100,
                        Description = "Test line"
                    }
                }
            });

            var result = service.Get(id);

            service.Delete(id);

            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        public void List()
        {
            var result = service.List();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
        {
            var id = service.Create(new Bill
            {
                OrganizationId = OrganizationId,
                ContactId = "GefjTensSluwEUkmmdBYCA",
                EntryDate = DateTime.Now,
                State = BillStates.draft,
                TaxMode = "incl",
                Lines = new List<BillLine>
                {
                    new BillLine
                    {
                        AccountId = "pdU7q2jETY6YehhE9ePR2g",
                        Amount = 100,
                        Description = "Test line"
                    }
                }
            });

            service.Delete(id);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void Delete()
        {
            var id = service.Create(new Bill
            {
                OrganizationId = OrganizationId,
                ContactId = "GefjTensSluwEUkmmdBYCA",
                EntryDate = DateTime.Now,
                State = BillStates.draft,
                TaxMode = "incl",
                Lines = new List<BillLine>
                {
                    new BillLine
                    {
                        AccountId = "pdU7q2jETY6YehhE9ePR2g",
                        Amount = 100,
                        Description = "Test line"
                    }
                }
            });

            var result = service.Delete(id);

            Assert.IsNotNull(result);
        }
    }
}
