using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ElectronServerBlazorEf.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace ElectronServerBlazorEf.TagHelper
{
    [HtmlTargetElement("toon")]
    [HtmlTargetElement(Attributes = "toonie")]

    public class ToonTag
    {
        private string baseUrl = "https://api4all.azurewebsites.net";
        public async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            IEnumerable<Toon> toons = await GetToonsAsync();
            string html = string.Empty;
            html += "<table><tr><th>Name</th><th>Picture</th></tr>";
            foreach (var item in toons) {    html += "<tr>";
                html += $"<td>{item.FirstName} {item.LastName}</td>";
                html += "<td><img src='" + item.PictureUrl + "' style='width: 50px' /></td>";
                html += "</tr>";
            }
            html += "</table>";
            output.Content.SetHtmlContent(html);
        }

        async Task<IEnumerable<Toon>> GetToonsAsync() {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            IEnumerable<Toon> toons = null;
            try {
                // Get all cartoon characters
                HttpResponseMessage response = await client.GetAsync("/api/people");
                if (response.IsSuccessStatusCode)
                {
                string json = await response.Content.ReadAsStringAsync();
                toons = JsonConvert.DeserializeObject<IEnumerable<Toon>>(json);
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return toons;
        }


    }
}