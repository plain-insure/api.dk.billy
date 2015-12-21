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

                var response = client.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ContactRoot>(response.Content);

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

                var response = client.Get(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ContactRoot>(response.Content);

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

                request.AddBody(new
                {
                    contact = new
                    {
                        organizationId = contact.organizationId,
                        name = contact.name,
                        countryId = contact.countryId,
                        street = contact.street,
                        zipcodeText = contact.zipcodeText,
                        cityText = contact.cityText,
                        phone = contact.phone,
                        isCustomer = contact.isCustomer,
                        isSupplier = contact.isSupplier,
                        contactPersons = new[]
                        {
                            new
                            {
                                name = contact.contactPersons[0].name,
                                email = contact.contactPersons[0].email
                            }
                        }
                    }
                });

                var response = client.Post(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ContactRoot>(response.Content);

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

                var response = client.Delete(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<ContactRoot>(response.Content);

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
