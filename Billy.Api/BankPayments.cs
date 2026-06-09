using Billy.Api.Models;

namespace Billy.Api
{
    public class BankPayments : Repositories.BaseWithCreate<BankPayment, BankPaymentRoot>
    {
        public BankPayments(RestSharp.RestClient client) : base(client) { }

        public BankPayments(string key) : base(key) { }
    }
}
