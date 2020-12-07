
using BusinessClasses.Models;
using MainService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MainService.Helpers
{
    //Komunikacija sa display-em
    class CommunicationHelper : DisposableObject
    {
        public CommunicationHelper () {
        }

        //Endpoint u ovom slučaju je cijeli url, bez samog enpoint-a
        public async Task<object> GetFromApi(string Endpoint, object Request)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => {
                    return true;
                };
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    string content = "";
                 
                    //formatiranje poruke i točnom formatu
                    content =GetQueryString(Request);

                    System.Net.Http.HttpResponseMessage response = await httpClient.GetAsync(Endpoint + "/?" + content).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                       
                        var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        //odgovor se parsa u json formatu
                        return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                    else
                    {
                        return "Service error " + response.StatusCode.ToString();
                    }
                }

            }
        }

        //fromatter stringa koji se šalje 
        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }


    }
}
