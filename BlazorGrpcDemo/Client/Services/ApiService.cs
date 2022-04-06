using Newtonsoft.Json;

#nullable disable

namespace BlazorGrpcDemo.Client.Services
{
    public class ApiService
    {
        HttpClient http;

        public ApiService(HttpClient _http)
        {
            http = _http;
        }

        public async Task<List<Person>> GetAll()
        {
            try
            {
                var result = await http.GetAsync("persons");
                result.EnsureSuccessStatusCode();
                string responseBody = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Person>>(responseBody);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public async IAsyncEnumerable<Person> GetAllStream()
        {
            // This is the pattern I'm using for retrieving an IAsyncEnumerable
            // from an API endpoint and returning an IAsyncEnumerable, one record
            // at a time, using yield return 

            // set up
            // I'm using Newtonsoft.Json rather than System.Text.Json
            var serializer = new JsonSerializer();

            // This is just a System.IO.Stream
            Stream stream = await http.GetStreamAsync("persons/getstream");

            if (stream != null)
            {
                using (stream!)
                {
                    // Create a System.IO.StreamReader
                    using (var streamReader = new StreamReader(stream))

                    // Create a Newtonsoft.Json.JsonTextReader from the StreamReader
                    // JsonTextReader treats a JSON string as a series of "tokens"
                    // which are read one at a time.
                    // see https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonToken.htm
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        // read each item
                        while (await jsonTextReader.ReadAsync())
                        {
                            // Ignore start and end tokens.
                            if (jsonTextReader.TokenType != JsonToken.StartArray
                                && jsonTextReader.TokenType != JsonToken.EndArray)
                            {
                                // return the item immediately
                                yield return serializer.Deserialize<Person>(jsonTextReader);
                            }
                        };
                    }
                }
            }
        }

        public async Task<Person> GetPersonById(int Id)
        {
            try
            {
                var result = await http.GetAsync($"persons/{Id}/getbyid");
                result.EnsureSuccessStatusCode();
                string responseBody = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Person>(responseBody);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
