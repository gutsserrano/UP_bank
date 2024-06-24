using Models;
using Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AddressApiServices
{
    public class UPBankAddressApi : IAddressApiService
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<Address?> CreateAddress(AddressDTO addressDTO)
        {
            StringContent content = new(JsonConvert.SerializeObject(addressDTO), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("https://localhost:7156/api/addresses", content);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Address viacepAddress = JsonConvert.DeserializeObject<Address>(jsonResponse);

            if (viacepAddress == null || viacepAddress.ZipCode == null)
                return null;

            var address = new Address
            {
                ZipCode = addressDTO.ZipCode,
                Number = addressDTO.Number,
                Street = viacepAddress.Street,
                Complement = addressDTO.Complement,
                City = viacepAddress.City,
                State = viacepAddress.State
            };

            return address;
        }

        public async Task<Address?> GetAddress(AddressDTO addressDTO)
        {
            string addressApiUrl = $"https://localhost:7156/api/addresses/zipcode/{addressDTO.ZipCode}/number/{addressDTO.Number}";
            HttpResponseMessage response = await _client.GetAsync(addressApiUrl);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            Address? viacepAddress = JsonConvert.DeserializeObject<Address>(jsonResponse);

            if ((int)response.StatusCode == 404)
                return null;

            if (viacepAddress == null)
                return null;

            var address = new Address
            {
                ZipCode = addressDTO.ZipCode,
                Number = addressDTO.Number,
                Street = viacepAddress.Street,
                Complement = addressDTO.Complement,
                City = viacepAddress.City,
                State = viacepAddress.State
            };

            return address;
        }
    }
}
