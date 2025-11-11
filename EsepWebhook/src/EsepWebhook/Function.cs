using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    public string FunctionHandler(object input, ILambdaContext context)
    {
        context.Logger.LogInformation($"FunctionHandler received: {input}");
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string payload = $"{{\"text\":\"Issue Created: {json.issue.html_url}\"}}";
        string slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");
        var client = new HttpClient();
        var req = new HttpRequestMessage(HttpMethod.Post, slackUrl)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        var resp = client.Send(req);
        using var reader = new StreamReader(resp.Content.ReadAsStream());
        return reader.ReadToEnd();
    }
}