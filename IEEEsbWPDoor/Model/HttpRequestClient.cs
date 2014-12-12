using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace IEEEsbWPDoor.Model
{
    public class HttpRequestClient
    {

        public async Task<String> RequestOpen(string uri, string Name, string ID, string Token)
        {
            var httpClient = new HttpClient();
            Dictionary<String, String> valuePair = new Dictionary<string, string>();
            valuePair.Add("Name", Name);
            valuePair.Add("DNI", ID);
            valuePair.Add("Token", Token);
            HttpContent content = new FormUrlEncodedContent(valuePair);
            var response = await httpClient.PostAsync(uri, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<String> RequestPrinterStatus(string uri)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            string result = string.Empty;
            try
            {
                result = await httpClient.GetStringAsync(uri);
            }
            catch(Exception e)
            {
                result = "Nope";
            }
            return result;
        }

        public async Task<String> RequestPrinterProfiles(string uri)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            string result = string.Empty;
            try
            {
                result = await httpClient.GetStringAsync(uri);
            }
            catch(Exception e)
            {
                result = "Nope";
            }
            return result;
        }

        public async Task<String> RequestSlic3r(string uri, string name, string dni, string token, string email, string profile, string filename, Stream stl)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent content = new MultipartFormDataContent();

            StringContent nameContent = new StringContent(name);
            nameContent.Headers.Add("name", "Name");
            content.Add(nameContent);
            StringContent dniContent = new StringContent(dni);
            dniContent.Headers.Add("name", "DNI");
            content.Add(dniContent);
            StringContent tokenContent = new StringContent(token);
            tokenContent.Headers.Add("name", "Token");
            content.Add(tokenContent);
            StringContent emailContent = new StringContent(email);
            emailContent.Headers.Add("name", "Email");
            content.Add(emailContent);

            StringContent profileContent = new StringContent(profile);
            profileContent.Headers.Add("name", "Profile");
            content.Add(profileContent);
            content.Add(CreateFileContent(stl, filename, "application/octet-stream"));
                
            var response = await httpClient.PostAsync(uri, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
            
        }

        private StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "attachment",
                FileName = fileName
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }


        public async Task<String> RequestToken(string uri, string Name, string ID, string RegistrationID)
        {
            var httpClient = new HttpClient();
            Dictionary<String, String> valuePair = new Dictionary<string, string>();
            valuePair.Add("Name", Name);
            valuePair.Add("DNI", ID);
            valuePair.Add("RegID", RegistrationID);
            HttpContent content = new FormUrlEncodedContent(valuePair);
            var response = await httpClient.PostAsync(uri, content);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<String> RequestCredit(string uri, string Name, string ID)
        {
            var httpClient = new HttpClient();
            Dictionary<String, String> valuePair = new Dictionary<string, string>();
            valuePair.Add("Name", Name);
            valuePair.Add("DNI", ID);
            HttpContent content = new FormUrlEncodedContent(valuePair);

            var response = await httpClient.PostAsync(uri, content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<String> RequestIEEENumber(string uri, string Name, string ID, string token)
        {
            var httpClient = new HttpClient();
            Dictionary<String, String> valuePair = new Dictionary<string, string>();
            valuePair.Add("Name", Name);
            valuePair.Add("DNI", ID);
            valuePair.Add("Token", token);
            HttpContent content = new FormUrlEncodedContent(valuePair);

            var response = await httpClient.PostAsync(uri, content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<String> RequestPair(string uri, string Name, string ID, string PairingToken, string token)
        {
            var httpClient = new HttpClient();
            Dictionary<String, String> valuePair = new Dictionary<string, string>();
            valuePair.Add("Name", Name);
            valuePair.Add("DNI", ID);
            valuePair.Add("PairingToken", PairingToken);
            valuePair.Add("Token", token);
            HttpContent content = new FormUrlEncodedContent(valuePair);

            var response = await httpClient.PostAsync(uri, content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<String> RequestUnpair(string uri, string Name, string ID, string token)
        {
            var httpClient = new HttpClient();
            Dictionary<String, String> valuePair = new Dictionary<string, string>();
            valuePair.Add("Name", Name);
            valuePair.Add("DNI", ID);
            valuePair.Add("Token", token);
            HttpContent content = new FormUrlEncodedContent(valuePair);

            var response = await httpClient.PostAsync(uri, content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return "Nope";
            }
            return await response.Content.ReadAsStringAsync();
        }
    }

}
