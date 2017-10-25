using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections;
using System.IO;
using System.Threading;
using SharpPwned.NET.Model;
using SharpPwned.NET;

namespace HBP_Gmail_Checker_UI
{
    public partial class Form1 : Form
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/GHBP.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "GHBP";
        
        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        // Set up the BackgroundWorker object by 
        // attaching event handlers. 
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            this.textBox5.Text = e.UserState.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                this.textBox5.Text = "Canceled";
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                this.textBox5.Text = e.Result.ToString();
            }

            // Enable the UpDown control.
            this.numericUpDown1.Enabled = true;

            // Enable the Start button.
            button1.Enabled = true;

           
            groupBox1.Enabled = true;

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = performSearchTask(worker, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.groupBox1.Enabled = false;
            this.button1.Enabled = false;
            this.button2.Enabled = true;
            this.progressBar1.Value = 0;
            this.backgroundWorker1.RunWorkerAsync();
        }

        private string performSearchTask(BackgroundWorker worker, DoWorkEventArgs e)
        {
            var client = new HaveIBeenPwnedRestClient();
            UserCredential credential;
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
            UsersResource.MessagesResource.ListRequest lRequest = service.Users.Messages.List(this.textBox4.Text);
            lRequest.LabelIds = this.textBox1.Text;
            lRequest.IncludeSpamTrash = this.checkBox1.Checked;
            lRequest.Q = this.textBox3.Text;
            //list of total messages coming from the query filter
            List<Google.Apis.Gmail.v1.Data.Message> result = new List<Google.Apis.Gmail.v1.Data.Message>();
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
                catch (Exception excp)
                {
                    worker.ReportProgress(0, "An error occurred: " + excp.Message);
                }
            } while (!String.IsNullOrEmpty(lRequest.PageToken));

            IList<MessagePartHeader> tempHeader = null;
            string emailToCheck = String.Empty;
            int processedMessages = 0;
            int totalMessages = 0;
            int maxRecords = System.Convert.ToInt32(this.numericUpDown1.Value);
            if (result != null && result.Count > 0)
            {
                totalMessages = result.Count;
                foreach (var messageItem in result)
                {
                    System.Threading.Thread.Sleep(50);
                    var emailInfoRequest = service.Users.Messages.Get(this.textBox4.Text, messageItem.Id);
                    //Gmail service gives us only the message Id for each message we need another call to have the
                    // other fields , in particular the headers 
                    var emailInfoResponse = emailInfoRequest.Execute();

                    if (emailInfoResponse != null)
                    {
                        tempHeader = emailInfoResponse.Payload.Headers;
                        foreach (MessagePartHeader mParts in tempHeader.Where(x => x.Name == this.textBox2.Text).ToList())
                        {
                            emailToCheck = ExtractString(mParts.Value);
                            //here we build the list of distinct senders
                            if (!distinctSenders.ContainsKey(emailToCheck))
                            {
                                distinctSenders.Add(emailToCheck, null);
                            }

                        }
                        processedMessages++;
                        worker.ReportProgress(processedMessages / totalMessages, "Processed " + processedMessages + " of " + totalMessages + " total messages");
                    
                        if (processedMessages >= maxRecords)
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
                            catch (Exception excp)
                            {
                                //try to wait more if we hit an exception
                                System.Threading.Thread.Sleep(5000);
                                
                            }
                            if (response != null && response.Count > 0)
                            {
                                fw.WriteLine(item);
                                
                                Console.WriteLine(" ");
                            }
                            //To avoid breaking api request limit of 1 request every 1.5 seconds
                            System.Threading.Thread.Sleep(2000);
                            processedSenders++;
                            worker.ReportProgress(processedSenders / totalSenders, "This sender has been pwned: " + item +" - Processed " + processedSenders + " of " + totalSenders + " total senders");

                        }
                    }
                }
            }
            else
            {
                worker.ReportProgress(10000, "No messages found");
            }
           

            return "Ok";
        }



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

        private void button2_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
            this.button2.Enabled = false;
        }
    }
}
