using Billy.Api.Models;

namespace Billy.Api
{
    public class Bills : Repositories.BaseWithDelete<Bill, BillRoot>
    {
        public Bills(RestSharp.RestClient client) : base(client) { }

        public Bills(string key) : base(key) { }


        public void SideloadLines()
        {
            this.AddSideload(r => r.BillLines, b => b.Lines);
        }
    }
}
