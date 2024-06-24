using System.Text.Json;
using UPBank.AddressAPI.PostalServices.Abstract;

namespace UPBank.AddressAPI.PostalServices
{
    public class ViaCepAddressResult : IAddressResult
    {
        public string Street { get; set; }
        public string StreetType { get; set; }
        public string Zipcode { get; set; }
        public string Neighborhood { get; set; }
        public string State { get; set; }
        public string City { get; set; }

        public static ViaCepAddressResult FromJson(string json)
        {
            var root = JsonDocument.Parse(json).RootElement;

            return new ViaCepAddressResult
            {
                Street = root.GetProperty("logradouro").GetString(),
                StreetType = root.GetProperty("logradouro").GetString().Split(" ").First(),
                Zipcode = root.GetProperty("cep").GetString(),
                Neighborhood = root.GetProperty("bairro").GetString(),
                State = root.GetProperty("uf").GetString(),
                City = root.GetProperty("localidade").GetString()
            };
        }
    }

    public class ViaCepService : IPostalAddressService
    {
        private string _url = "https://viacep.com.br/ws/";

        public async Task<IAddressResult?> Fetch(string zipcode)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(_url + zipcode + "/json");

                if (await IsErrorResponse(response))
                    return null;

                string data = await response.Content.ReadAsStringAsync();

                return ViaCepAddressResult.FromJson(data);
            }
        }

        private async Task<bool> IsErrorResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return true;

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                var root = JsonDocument.Parse(json).RootElement;

                if (root.TryGetProperty("erro", out var error) && error.GetBoolean())
                    return true;
            }
            catch (JsonException)
            {
            }

            return false;
        }
    }
}
