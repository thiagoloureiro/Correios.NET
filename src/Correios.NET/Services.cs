using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Correios.NET.Models;
using System.Text;
using Newtonsoft.Json;

namespace Correios.NET
{
    public class Services : IServices
    {
        //mobile url http://m.correios.com.br/movel/index.do
        private const string PACKAGE_TRACKING_URL = "http://sro.micropost.com.br/consulta.php?objetos={0}";
        private const string ZIP_ADDRESS_URL = "http://m.correios.com.br/movel/buscaCepConfirma.do";

        private readonly HttpClient _httpClient;

        public Services()
        {
            _httpClient = new HttpClient();            
        }

        public async Task<Package> GetPackageTrackingAsync(string packageCode)
        {
            var url = string.Format(PACKAGE_TRACKING_URL, packageCode);
            var response = await _httpClient.GetByteArrayAsync(url);
            var html = Encoding.GetEncoding("ISO-8859-1").GetString(response, 0, response.Length - 1);
            return await Task.Run(() => Package.Parse(html));
        }

        public Package GetPackageTracking(string packageCode)
        {
            return GetPackageTrackingAsync(packageCode).Result;
        }


        public async Task<Address> GetAddressAsync(string zipCode)
        {
            Address ret;
            string page = $"https://viacep.com.br/ws/{zipCode}/json/";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                ret = JsonConvert.DeserializeObject<Address>(result);
            }
            return ret;
        }

        public Address GetAddress(string zipCode)
        {
            Address ret;
            string page = $"https://viacep.com.br/ws/{zipCode}/json/";

            using (var client = new HttpClient())
            using (var response =  client.GetAsync(page).Result)
            using (var content = response.Content)
            {
                string result = content.ReadAsStringAsync().Result;
                ret = JsonConvert.DeserializeObject<Address>(result);
            }
            return ret;
        }

        private static FormUrlEncodedContent CreateAddressRequest(string zipCode)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("cepEntrada", zipCode),
                new KeyValuePair<string, string>("tipoCep", string.Empty),
                new KeyValuePair<string, string>("cepTemp", string.Empty),
                new KeyValuePair<string, string>("metodo", "buscarCep"),
            });
            return content;
        }
    }
}
