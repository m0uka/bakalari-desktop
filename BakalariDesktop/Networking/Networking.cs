using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BakalariDesktop.Networking
{
    public class Networking
    {
        public string GET(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                var webResponse = request.GetResponse();
                var webStream = webResponse.GetResponseStream();
                var responseReader = new StreamReader(webStream);
                var response = responseReader.ReadToEnd();
                responseReader.Close();
                return response;
            }
            catch
            {
                return null; // chyba
            }
            
        }

        /// <summary>
        /// Vrátí dynamický objekt z XML získaný z URL.
        /// </summary>
        /// <param name="url">URL stránky</param>
        public dynamic ParseXMLFromURL (string url)
        {
            string response = GET(url);
            if (response == null) return null;
            XDocument doc = XDocument.Parse(response);
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
            return dyn;
        }
    }
}
