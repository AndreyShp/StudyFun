using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Translators {
    internal class MicrosoftTranslator : ITranslator {
        //TODO: подсчитывать кол-во символов
        private const string CLIENT_ID = "Welcome_To_Study_Fun";
        private const string CLIENT_SECRET = "kIGkAvgP0gw2SI+RNCjn/w/KMAGZxrOjRVRsHBB/JuM=";

        #region ITranslator Members

        public string Name {
            get { return "Microsoft"; }
        }

        public List<string> Translate(LanguageShortName from, LanguageShortName to, string text) {
            int i = 0;
            do {
                var admAuth = new AdmAuthentication(CLIENT_ID, CLIENT_SECRET);
                try {
                    AdmAccessToken admToken = admAuth.GetAccessToken();
                    return Translate(text, from.ToString().ToLowerInvariant(), to.ToString().ToLowerInvariant(),
                                     admToken);
                } catch (WebException e) {
                    Console.WriteLine("MicrosoftTranslator.Translate исключение! попытка перевода {0}", i + 1);
                    ProcessWebException(e);
                } catch (Exception ex) {
                    Console.WriteLine("MicrosoftTranslator.Translate исключение! попытка перевода {0}", i + 1);
                    //TODO: логировать
                }
                i++;
            } while (i <= 5);
            return null;
        }

        #endregion

        private static List<string> Translate(string text, string from, string to, AdmAccessToken admToken) {
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text="
                         + HttpUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;
            string authToken = "Bearer" + " " + admToken.access_token;

            var httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);

            WebResponse response = httpWebRequest.GetResponse();
            using (Stream stream = response.GetResponseStream()) {
                var dcs = new DataContractSerializer(Type.GetType("System.String"));
                var translation = (string) dcs.ReadObject(stream);
                return !string.IsNullOrWhiteSpace(translation)
                           ? new List<string> {translation.Trim()}
                           : new List<string>(0);
            }
        }

        /* private static void DetectMethod(string authToken, string text)
        {
            //Keep appId parameter blank as we are sending access token in authorization header.
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Detect?text=" + text;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string languageDetected = (string)dcs.ReadObject(stream);
                    Console.WriteLine(string.Format("Language detected:{0}", languageDetected));
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }

            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }*/

        private static void ProcessWebException(WebException e) {
            //TODO: логировать
            //Console.WriteLine("{0}", e);
            // Obtain detailed error information
            /*string strResponse = string.Empty;
            using (var response = (HttpWebResponse) e.Response) {
                using (Stream responseStream = response.GetResponseStream()) {
                    using (var sr = new StreamReader(responseStream, Encoding.ASCII)) {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }*/
            //TODO: логировать
            //Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }
    }

    [DataContract]
    public class AdmAccessToken {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication {
        private const int REFRESH_TOKEN_DURATION = 9;
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private readonly Timer _accessTokenRenewer;
        private readonly string _clientId;
        private readonly string _request;
        private string _clientSecret;
        private AdmAccessToken _token;

        public AdmAuthentication(string clientId, string clientSecret) {
            _clientId = clientId;
            _clientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            _request =
                string.Format(
                    "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
                    HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
            _token = HttpPost(DatamarketAccessUri, _request);
            //renew the token every specfied minutes
            _accessTokenRenewer = new Timer(OnTokenExpiredCallback, this, TimeSpan.FromMinutes(REFRESH_TOKEN_DURATION),
                                            TimeSpan.FromMilliseconds(-1));
        }

        public AdmAccessToken GetAccessToken() {
            return _token;
        }

        private void RenewAccessToken() {
            AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, _request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            _token = newAccessToken;
            //TODO: логировать
            //Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", clientId, token.access_token));
        }

        private void OnTokenExpiredCallback(object stateInfo) {
            try {
                RenewAccessToken();
            } catch (Exception ex) {
                //TODO: логировать
                //Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            } finally {
                try {
                    _accessTokenRenewer.Change(TimeSpan.FromMinutes(REFRESH_TOKEN_DURATION),
                                               TimeSpan.FromMilliseconds(-1));
                } catch (Exception ex) {
                    //TODO: логировать
                    /* Console.WriteLine(string.Format(
                        "Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));*/
                }
            }
        }

        private static AdmAccessToken HttpPost(string datamarketAccessUri, string requestDetails) {
            //Prepare OAuth request 
            int i = 0;
            do {
                try {
                    WebRequest webRequest = WebRequest.Create(datamarketAccessUri);
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.Method = "POST";
                    byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                    webRequest.ContentLength = bytes.Length;
                    using (Stream outputStream = webRequest.GetRequestStream()) {
                        outputStream.Write(bytes, 0, bytes.Length);
                    }
                    using (WebResponse webResponse = webRequest.GetResponse()) {
                        Stream responseStream = webResponse.GetResponseStream();
                        if (responseStream == null) {
                            Console.WriteLine("MicrosoftTranslator.HttpPost Не удалось авторизоваться!!!");
                            continue;
                        }
                        var serializer = new DataContractJsonSerializer(typeof (AdmAccessToken));
                        //Get deserialized object from JSON stream
                        var token = (AdmAccessToken) serializer.ReadObject(responseStream);
                        return token;
                    }
                } catch (Exception e) {
                    Console.WriteLine("MicrosoftTranslator.HttpPost Не удалось авторизоваться, т.к. исключение!!!");
                }
                i++;
            } while (i <= 10);
            return null;
        }
    }
}