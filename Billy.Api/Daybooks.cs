using Billy.Api.Models;

namespace Billy.Api
{
    public class Daybooks : Repositories.BaseWithDelete<Daybook, DaybookRoot>
    {
        public Daybooks(RestSharp.RestClient client) : base(
            client,
            "daybooks/",
            (root) => root?.Daybook,
            (root) => root?.Daybooks,
            (item) => item?.Id,
            (item) => new DaybookRoot { Daybook = item },
            (root) => root?.Meta?.DeletedRecords?.Daybooks?.FirstOrDefault()
        )
        { }

        public Daybooks(string key) : base(
            key,
            "daybooks/",
            (root) => root?.Daybook,
            (root) => root?.Daybooks,
            (item) => item?.Id,
            (item) => new DaybookRoot { Daybook = item },
            (root) => root?.Meta?.DeletedRecords?.Daybooks?.FirstOrDefault()
        )
        { }
    }
}
