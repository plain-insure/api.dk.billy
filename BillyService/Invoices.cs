using Billy.Models;
using BillyService.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BillyService
{
    public class Invoices
    {
        private RestClient client;

        public Invoices(string key)
        {
            client = new RestClient("https://api.billysbilling.com/v2/");
            client.AddDefaultHeader("X-Access-Token", key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Invoice Get(string id)
        {
            try
            {
                var request = new RestRequest("invoices/" + id, Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<InvoiceRoot>(response.Content);

                    return result.invoice;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Invoice> List()
        {
            try
            {
                var request = new RestRequest("invoices/", Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<InvoiceRoot>(response.Content);

                    return result.invoices;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public string Create(Invoice invoice)
        {
            try
            {
                var request = new RestRequest("invoices/", Method.POST);

                request.RequestFormat = DataFormat.Json;

                request.AddBody(new 
                {
                    invoice = new
                    {
                        invoiceNo = invoice.invoiceNo,
                        organizationId = invoice.organizationId,
                        contactId = invoice.contactId,
                        entryDate = invoice.entryDate,
                        taxMode = invoice.taxMode,
                        paymentTermsDays = invoice.paymentTermsDays,
                        state = invoice.state,
                        paymentTermsMode = invoice.paymentTermsMode,
                        sentState = invoice.sentState,
                        contactMessage = invoice.contactMessage,
                        lines = new []
                        {
                            new
                            {
                                unitPrice = invoice.lines[0].unitPrice,
                                productId = invoice.lines[0].productId,
                                description = invoice.lines[0].description
                            }
                        }
                    }
                });

                var response = client.Post(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<InvoiceRoot>(response.Content);

                    return result.invoices[0].id;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}


