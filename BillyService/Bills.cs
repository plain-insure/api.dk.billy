using BillyService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BillyService
{
    public class Bills
    {
        private HttpClient client;

        public Bills(string key)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://api.billysbilling.com/v2/");
            client.DefaultRequestHeaders.Add("X-Access-Token", key);
        }

        // GET /v2/contacts:id
        public async Task<GetBillRoot> GetBill(string id)
        {
            try
            {
                string url = "bills/" + id;

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GetBillRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        // GET /v2/bills
        public async Task<GetBillListRoot> GetBillList()
        {
            try
            {
                string url = "bills/";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GetBillListRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        public async Task<PostBillResultRoot> PostBill(PostBillRoot bill)
        {
            try
            {
                string url = "bills/";

                var json = JsonConvert.SerializeObject(bill);

                HttpResponseMessage response = await client.PostAsJsonAsync<PostBillRoot>(url, bill);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<PostBillResultRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }
    }
}
