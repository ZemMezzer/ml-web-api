using Newtonsoft.Json.Linq;

namespace MLTextGenerationAPIModule.Requests;

public class TextGenerationApiRequestHandler
{
    private readonly HttpClient _client = new();
    private string _apiUrl;

    public TextGenerationApiRequestHandler(string apiUrl)
    {
        _client.Timeout = new TimeSpan(0, 10, 0);
        _apiUrl = apiUrl;
    }
        
    public async Task<string> Send(RequestData requestData)
    {
        using (HttpContent content = new StringContent(requestData.ToJson()))
        {
            var response = await _client.PostAsync(_apiUrl, content);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            var jsonTask = response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(jsonTask.Result);

            response.Dispose();
                
            var parsedResult = result.Root["results"].First["history"]["visible"]?.Last.Last.ToString();

            var formattedResult = System.Net.WebUtility.HtmlDecode(parsedResult);
            return formattedResult;
        }
    }
}