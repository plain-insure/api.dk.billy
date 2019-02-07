using BillyService.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

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

                var response = client.Get<InvoiceRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

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

                var response = client.Get<InvoiceRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

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

                request.AddJsonBody(invoice);

                var response = client.Post<InvoiceRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

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


