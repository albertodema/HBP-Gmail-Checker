using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpPwned.NET.Model;
using SharpPwned.NET;


namespace HBP_Gmail_Checker
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/GHBP.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "GHBP";
        
        
        //This function extracts email addresses from the choosen header field
        static string ExtractString(string s)
        {
            string returnValue = s;
            if (s.Contains("<") && s.Contains(">"))
            {
                var startTag = "<";
                int startIndex = s.IndexOf(startTag) + startTag.Length;
                int endIndex = s.IndexOf(">", startIndex);
                returnValue = s.Substring(startIndex, endIndex - startIndex);
            }
            return returnValue;
        }
        static void Main(string[] args)
        {
            UserCredential credential;
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;

            var client = new HaveIBeenPwnedRestClient();

            //We open the oauth setting of this app and request with browser your consent (only first time)
            using (var stream =
              new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                  System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/GHBP.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  GoogleClientSecrets.Load(stream).Secrets,
                  Scopes,
                  "user",
                  CancellationToken.None,
                  new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            // Define parameters of request.
            UsersResource.MessagesResource.ListRequest lRequest = service.Users.Messages.List(appSettings["userId"]);
            lRequest.LabelIds = appSettings["labelBeScanned"];
            lRequest.IncludeSpamTrash = System.Convert.ToBoolean(appSettings["IncludeSpamTrash"]);
            lRequest.Q = appSettings["messageQueryFilter"];
            //list of total messages coming from the query filter
            List<Message> result = new List<Message>();
            //Distinct List of Senders
            Hashtable distinctSenders = new Hashtable();
            do
            {
                try
                {
                    //Gmail service does not give all the messages in one shot, so we have to perfom multiple request
                    //using pagination.
                    ListMessagesResponse response = lRequest.Execute();
                    result.AddRange(response.Messages);
                    lRequest.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            } while (!String.IsNullOrEmpty(lRequest.PageToken));
            
            IList<MessagePartHeader> tempHeader = null;
            string emailToCheck = String.Empty;
            int processedMessages = 0;
            int totalMessages = 0;
            int maxRecords = System.Convert.ToInt32(appSettings["maxMessagesToBeScanned"]);
            if (result != null && result.Count > 0)
            {
                totalMessages = result.Count;
                foreach (var messageItem in result)
                {
                    System.Threading.Thread.Sleep(50);
                    var emailInfoRequest = service.Users.Messages.Get(appSettings["userId"], messageItem.Id);
                    //Gmail service gives us only the message Id for each message we need another call to have the
                    // other fields , in particular the headers 
                    var emailInfoResponse = emailInfoRequest.Execute();

                    if (emailInfoResponse != null)
                    {   
                        tempHeader = emailInfoResponse.Payload.Headers;
                        foreach (MessagePartHeader mParts in tempHeader.Where(x=>x.Name== appSettings["headerToProcess"]).ToList())
                        {
                            emailToCheck = ExtractString(mParts.Value);
                            //here we build the list of distinct senders
                            if (!distinctSenders.ContainsKey(emailToCheck))
                            {
                                distinctSenders.Add(emailToCheck, null);
                            }                          

                        }
                        processedMessages++;
                        Console.WriteLine("Processed " + processedMessages + " of " + totalMessages + " total messages");
                        if(processedMessages>=maxRecords)
                        {
                            //we stop the execution if we reached the max amount of messages defined into the config
                            break;
                        }
                    }

                }
                int totalSenders = 0;
                int processedSenders = 0;

                using (var fw = new StreamWriter("GHBP_export.txt", true))
                {
                    if (distinctSenders.Count > 0)
                    {
                        totalSenders = distinctSenders.Count;
                        foreach (var item in distinctSenders.Keys)
                        {
                            List<Breach> response = null;
                            try
                            {
                                response = client.GetAccountBreaches(item.ToString()).Result;
                            }
                            catch (Exception e)
                            {
                                //try to wait more if we hit an exception
                                System.Threading.Thread.Sleep(5000);                              
                                Console.WriteLine("An error occurred: " + e.Message);                            
                            }
                            if (response != null && response.Count > 0)
                            {
                                fw.WriteLine(item);
                                Console.WriteLine("This sender has been pwned: " + item);
                                Console.WriteLine(" ");
                            }
                            //To avoid breaking api request limit of 1 request every 1.5 seconds
                            System.Threading.Thread.Sleep(2000);
                            processedSenders++;                            
                            Console.WriteLine("Processed " + processedSenders + " of " + totalSenders + " total senders");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No Messages Found.");
            }
            Console.Read();

        }
    }

}

