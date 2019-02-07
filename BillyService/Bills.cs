using BillyService.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

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

                var response = client.Get<BillRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

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

                var response = client.Get<BillRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

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
                request.AddJsonBody(bill);

                var response = client.Post<BillRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

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
