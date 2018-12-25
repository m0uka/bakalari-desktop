using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BakalariDesktop.Login
{
    public class LoginManager
    {

        dynamic GetUserXML(string username, string url)
        {
            Networking.Networking network = new Networking.Networking();
            dynamic xml = null;


            xml = network.ParseXMLFromURL(url + "/login.aspx?gethx=" + username);
            if (xml == null) return null;
            if (xml.results.res != "02")
            {
                return xml;
            }
            else
            {
                return null;
            }

        }

        public string GenerateToken(string username, string password, string url)
        {
            
                dynamic xml = GetUserXML(username, url);
                if (xml == null)
                {
                    Console.Out.WriteLine("CHYBA: NASTALA CHYBA - XML ERR");
                    return null;
                }

                string pwd = xml.results.salt + xml.results.ikod + xml.results.typ + password;
                string hashedPwd = EncryptSHA512(pwd);

                string Date = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                string token = "*login*" + username + "*pwd*" + hashedPwd + "*sgn*ANDR" + Date;
                token = EncryptSHA512(token);

                token = token.Replace(@"\", "_").Replace(@"/", "_").Replace(@"+", "-");

                bool tokenValidate = false;

                tokenValidate = ValidateToken(token, url);

                if (tokenValidate)
                {
                    return token;
                }
                else
                {
                    return null;
                }

          
         
        }

        bool ValidateToken(string token, string url)
        {
            Networking.Networking network = new Networking.Networking();
            dynamic xml = null;

            xml = network.ParseXMLFromURL(url + "/login.aspx?hx=" + token + "&pm=all");
            if (xml == null)
            {
                Console.Out.WriteLine("CHYBA: NASTALA CHYBA - XML ERR");
                return false;
            }

            if (xml.results.result != "-1")
            {
                return true;
            } else
            {
                Console.Out.WriteLine("TOKEN NESPRÁVNÝ - MOŽNÁ ŠPATNÉ HESLO");
                return false;
            }

        }

        

        string EncryptSHA512 (string token)
        {
            SHA512Managed HashTool = new SHA512Managed();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(token));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }
    }
}
