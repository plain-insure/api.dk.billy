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
    public class Contacts
    {
        private HttpClient client;

        public Contacts(string key)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://api.billysbilling.com/v2/");
            client.DefaultRequestHeaders.Add("X-Access-Token", key);
        }

        // GET /v2/contacts:id
        public async Task<GetContactRoot> GetContact(string id)
        {
            try
            {
                string url = "contacts/" + id;

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GetContactRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }


        // GET /v2/contacts
        public async Task<GetContactListRoot> GetContactList()
        {
            try
            {
                string url = "contacts/";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<GetContactListRoot>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        // POST /v2/contacts
        public async Task<PostContactRootResult> PostCompany(
            ContactTypes type,
            string organization,
            string name,
            Countries country,
            string street,
            string city,
            string zipcode,
            string phone,
            string vat,
            bool isCustomer,
            bool isSupplier,
            List<ContactPerson> contactPersons)
        {
            PostContactRoot root = new PostContactRoot
            {
                contact = new PostContact
                {
                    type = ContactTypes.company.ToString(),
                    organizationId = organization,
                    name = name,
                    countryId = Countries.DK.ToString(),
                    street = street,
                    cityText = city,
                    zipcodeText = zipcode,
                    phone = phone,
                    registrationNo = vat,
                    isCustomer = isCustomer,
                    isSupplier = isSupplier,
                   contactPersons = contactPersons
                }
            };

            try
            {
                string url = "contacts/";

                HttpResponseMessage response = await client.PostAsJsonAsync<PostContactRoot>(url, root);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<PostContactRootResult>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }

        // POST /v2/contacts
        public async Task<PostContactRootResult> PostCustomer(PostContact contactRoot)
        {
            PostContactRoot root = new PostContactRoot
            {
                contact = contactRoot
            };

            var test = JsonConvert.SerializeObject(root);

            try
            {
                string url = "contacts/";

                HttpResponseMessage response = await client.PostAsJsonAsync<PostContactRoot>(url, root);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<PostContactRootResult>(responseBody);

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.Message);
            }
        }
    }

}
