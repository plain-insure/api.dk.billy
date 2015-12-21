using System.Net.Http;
using System.Threading.Tasks;
using BillyService.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Collections.Generic;

namespace BillyService
{
    public class Bills
    {
        private RestClient client;

        public Bills(string key)
        {
            client = new RestClient("https://api.billysbilling.com/v2/");
            client.AddDefaultHeader("X-Access-Token", key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bill Get(string id)
        {
            try
            {
                var request = new RestRequest("bills/" + id, Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<BillRoot>(response.Content);

                    return result.bill;
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
        public List<Bill> List()
        {
            try
            {
                var request = new RestRequest("bills/", Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<BillRoot>(response.Content);

                    return result.bills;
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
        /// <param name="bill"></param>
        /// <returns></returns>
        public string Create(Bill bill)
        {
            try
            {
                var request = new RestRequest("bills/", Method.POST);

                request.RequestFormat = DataFormat.Json;

                request.AddBody(new
                {
                    bill = new
                    {
                        organizationId = bill.organizationId,
                        contactId = bill.contactId,
                        entryDate = bill.entryDate,
                        taxMode = bill.taxMode,
                        state = bill.state,
                        suppliersInvoiceNo = bill.suppliersInvoiceNo,
                        voucherNo = bill.voucherNo,
                        lines = new[]
                        {
                            new
                            {
                                accountId = bill.lines[0].accountId,
                                amount = bill.lines[0].amount,
                                description = bill.lines[0].description,
                                taxRateId = bill.lines[0].taxRateId
                            }
                        }
                    }
                });

                var response = client.Post(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<BillRoot>(response.Content);

                    return result.bills[0].id;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
