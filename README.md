# HBP-Gmail-Checker
Hi this program has the following objective : scan one and one only gmail account (your account!) 
to find if some of yours friends/collegues has been pwned .
By using this program you agree on the following:
1) Allow this tool to scan your gmail emails (not the body content just the "From" field)
2) Allow this tool to scan build a list of distinct email addresses that have written an email to you
3) Check if any email account of this list has been pwned 
4) Obtain a final report (simple txt file) of email accounts pwned
5) The code is provided AS IS, you are free to use it, copy it, modify it, I do not claim any copyright on it, of course a mention if you re-use it is more than welcome ! :-)

With this tool you will NOT:
1) Know where and how those emails accounts have been pwned
2) Automatically alert those accounts of the risk

I specifically escluded the second option because you have to decide which email accounts 
are really people that you know and care about and use the way you think it's the best to alert them
of the risk.

Now we can discuss how this tool work and how to use it.
It's a .NET (standard .NET) console application and before using it you have to personalize it changing the HBP_Gmail_Checker.exe.config file

Here the parts that you have to change:
1)  maxMessagesToBeScanned property , set into the value field the max amount of messages you want to scan
2)  labelBeScanned property, set into the value field the gmail label you want to scan (I suggest to you to leave "INBOX")
3)  IncludeSpamTrash property, set the value field to true if you want to scan also SpamTrash messages  (I suggest to you to leave it to "false")
4)  userId property, set into the value field the gmail account to be scanned (I suggest to you to leave "me")
5)  headerToProcess, set into the value field the header name you want to process  (I suggest to you to leave "From" )
6)  messageQueryFilter, this is the MOST IMPORTANT value you have to set in order to avoid to scan thousands of messages! 
    This the gmail filter to search only for specific patterns, I provided a sample one that escludes usual newsletters, etc..., 
	but look CAREFULLY to it to avoid to process too many emails .

Once you have finished with the HBP_Gmail_Checker.exe.config, you can execute the HBP_Gmail_Checker.exe,
on the first launch it will open your default browser and ask to give permission to access your email
and it will save the temporary access token in your default Personal folder (My Documents) in a file.
At this time the email scan process will start and at end you will have file GHBP_export.txt in the same folder
of the HBP_Gmail_Checker.exe executable containg the pwned accounts.

I hope this tool can help you and your friends/collegues to understand better the risks of accounts/password reuse, 
in my case the result was the following:
1) Alerted and saved my wife gmail account . We decided to setup two factor authentication for her.
2) Alerted 5 friends : 3 were false positives (their already knew about HBP) , but two discovered for the first time this risk and I explained them how to setup two factor authentication.

Net result: 3 people are now more safe and secure and I hope this tool can help you in the same way it helped me.

My contacts :
email : alberto.demarco at  gmail.com
twitter handle: albertod

If you want to donate: please go to troy hunt web site and donate to him for the fantastic work he has done!
