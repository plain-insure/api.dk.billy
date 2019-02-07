using BillyService.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace BillyService
{
    public class Contacts
    {
        private RestClient client;

        public Contacts(string key)
        {
            client = new RestClient("https://api.billysbilling.com/v2/");
            client.AddDefaultHeader("X-Access-Token", key);
        }

        // GET /v2/contacts:id
        public Contact Get(string id)
        {
            try
            {
                var request = new RestRequest("contacts/" + id, Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get<ContactRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return result.contact;
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


        // GET /v2/contacts
        public List<Contact> List()
        {
            try
            {
                var request = new RestRequest("contacts/", Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get<ContactRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return result.contacts;
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

        // POST /v2/contacts
        public string Create(Contact contact)
        {
            try
            {
                var request = new RestRequest("contacts/", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(contact);

                var response = client.Post<ContactRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return result.contacts[0].id;
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

        public bool Delete(string id)
        {
            try
            {
                var request = new RestRequest("contacts/" + id, Method.DELETE);

                request.RequestFormat = DataFormat.Json;

                var response = client.Delete<ContactRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
