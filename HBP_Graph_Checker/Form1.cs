using System;
using System.Windows.Forms;
using SharpPwned.NET;
using SharpPwned.NET.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Linq;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Collections;

namespace HBP_Graph_Checker
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int maxEmails = System.Convert.ToInt32(this.numericUpDown1.Value);
            //AuthenticationResult lrt = await GraphHelper.GetAccessToken().ConfigureAwait(false);
                
            Hashtable senders=await GraphHelper.SignInAndGetDistinctSenders(maxEmails,this.textBox3.Text).ConfigureAwait(false);
            
            this.dataGridView1.DataSource = HBPChecker.verifyListOfSenders(senders);
        }
    }

    public class GraphHelper
    {
        private static string ClientId = "9cb79517-6ac4-4afd-9376-08e5b3a05be2";
        private static string _accessToken = String.Empty;
        //Set the API Endpoint to Graph 'me' endpoint
        private static string _graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";
        private static PublicClientApplication PublicClientApp = new PublicClientApplication(ClientId, "https://login.microsoftonline.com/common", TokenCacheHelper.GetUserCache());
        //Set the scope for API call to user.read
        private static string[] scopes = new string[] { "user.read", "mail.read" };
        public static async Task<AuthenticationResult> GetAccessToken()
        {

            AuthenticationResult authResult = null;
            try
            {
                authResult = await PublicClientApp.AcquireTokenAsync(scopes).ConfigureAwait(false);
                _accessToken = authResult.AccessToken;
            }
            
            catch (Exception ex)
            {
                return authResult;
            }

            return authResult;
        }

        public async static Task<Hashtable> SignInAndGetDistinctSenders(int maxEmails,string queryFilter)
        {
            Hashtable distinctSenders = new Hashtable();
            GraphServiceClient client = new GraphServiceClient(new DelegateAuthenticationProvider(
            async (requestMessage) =>
            {
                AuthenticationResult authResult = await GetAccessToken();
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            }));


            try
            {
                var user = await client.Me.Request().GetAsync();

                var iClient = new GraphServiceClient(
                      new DelegateAuthenticationProvider(
                          (requestMessage) =>
                          {
                              requestMessage.Headers.Authorization =
                                  new AuthenticationHeaderValue("Bearer", _accessToken);
                              requestMessage.Headers.Add("X-AnchorMailbox", user.Mail);

                              return Task.FromResult(0);
                          }));

                var mailResults = await iClient.Me.MailFolders.Inbox.Messages.Request()
                            .Filter(queryFilter)
                            //.OrderBy("receivedDateTime DESC")
                            .Select(m => new { m.From })
                            .Top(maxEmails)
                            .GetAsync();

                foreach (var msg in mailResults.CurrentPage)
                {
                    //here we build the list of distinct senders
                    if (!distinctSenders.ContainsKey(msg.From.EmailAddress.Address))
                    {
                        distinctSenders.Add(msg.From.EmailAddress.Address, null);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return distinctSenders;
        }
        private static async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void SignOut()
        {
            if (PublicClientApp.Users.Any())
            {
                PublicClientApp.Remove(PublicClientApp.Users.FirstOrDefault());
            }
        }
    }

    public sealed class HBPChecker
    {
        private static volatile HaveIBeenPwnedRestClient instance;
        private static object syncRoot = new Object();

        private HBPChecker() { }

        private static HaveIBeenPwnedRestClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new HaveIBeenPwnedRestClient();
                    }
                }

                return instance;
            }
        }
        private static HBPResult verifyEmail(string email)
        {
            HBPResult result = new HBPResult();
            result.Email = email;
            result.Pwned = 0;
            List<Breach> lst = null;
            lst = HBPChecker.Instance.GetAccountBreaches(email).Result;
            if (lst != null && lst.Count > 0)
            {
                result.Pwned = 1;
            }
            return result;
        }

        public static List<HBPResult> verifyListOfSenders(Hashtable senders)
        {
            List<HBPResult> lst = new List<HBPResult>();
            foreach(var item in senders.Keys)
            {
                lst.Add(HBPChecker.verifyEmail(item.ToString()));
                System.Threading.Thread.Sleep(1800);
            }
            return lst;
        }
    }
    

    [Serializable]
    public class HBPResult
    {
        string _email = String.Empty;
        int _pwned = 0;

        public string Email { get => _email; set => _email = value; }
        public int Pwned { get => _pwned; set => _pwned = value; }
    }
}
