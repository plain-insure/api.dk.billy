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
    public class Invoices
    {
        private HttpClient client;

        public Invoices(string key)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://api.billysbilling.com/v2/");
            client.DefaultRequestHeaders.Add("X-Access-Token", key);
        }

        // GET /v2/invoices/:id 
        public async Task<InvoiceRoot> GetInvoice(string id)
        {
            try
            {
                string url = "invoices/" + id;

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<InvoiceRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        // GET /v2/invoices/:id 
        public async Task<InvoiceListRoot> GetInvoiceList()
        {
            try
            {
                string url = "invoices/";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<InvoiceListRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        /// <summary>
        /// POST /v2/invoices
        /// </summary>
        /// <param name="invoiceNo">Invoice number (order number)</param>
        /// <param name="organizationId">Number of the organization</param>
        /// <param name="contactId">Contact ID</param>
        /// <param name="entryDate">Date of entry</param>
        /// <param name="paymentTermsDays">Days to payment</param>
        /// <param name="contactMessage">Message</param>
        /// <param name="lines">List of lines</param>
        /// <returns>PostInvoiceResultRoot</returns>
        public async Task<PostInvoiceResultRoot> PostInvoice(PostInvoice invoicePost)
        {
            PostInvoiceRoot root = new PostInvoiceRoot
            {
                invoice = invoicePost
            };

            var test = JsonConvert.SerializeObject(root);

            try
            {
                string url = "invoices/";

                HttpResponseMessage response = await client.PostAsJsonAsync<PostInvoiceRoot>(url, root);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<PostInvoiceResultRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }
    }
}
