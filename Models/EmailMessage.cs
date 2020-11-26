using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Windows;

namespace NBMMessageFiltering.Models
{
    //This class will be used to create messages of type Email
    public class EmailMessage : Message
    {

        private DataStore dataStore = DataStore.Instance;
        public EmailMessage(string msgID, string[] msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        //This method will be used to process the message body
        //Validates email message contents - sender, subject and body
        //As the message is an email, it will remove URLs and replace them with "<URL Quarantined>"
        //Will also check if the email is a SIR (Serious Incident Report) and will add it to a list of SIRs which will be displayed when ending session
        public override string processMessage(string[] message)
        {
            if (message.Length == 0 || message == null)
            {
                MessageBox.Show("Invalid Input - No input");
                return "";
            }

            string sender = message[0].Trim();
            string subject = message[1].Trim();
            string body = message[2].Trim();

            try
            {
                MailAddress emailAddress = new MailAddress(sender);
            } catch (Exception e)
            {
                MessageBox.Show("Invalid Email Address");
                return "";
            }

            if (body.Length > 1028 )
            {
                MessageBox.Show("Message Length Exceeded");
                return "";
            } else if (subject.Length > 20)
            {
                MessageBox.Show("Subject Length Exceeded");
                return "";
            }

            //Splitting message body into words. Doing it as a list so can go through and remove any empty elements (in case user has entered a double space)
            List<string> words = new List<string>(body.Split(new char[] {' ', '\n', '\r'})); //Splitting message body into words
            words.RemoveAll(str => string.IsNullOrWhiteSpace(str)); //Removing any empty elements

            string returnMessage = "";

            //This will rely on the inputs being correctly spaced as specified in document.
            if (subject.Contains("SIR")) //Checking if subject is a Significant Incident Report (SIR) - index 1 in message is the subject
            {
                try
                {
                    string sortCode = words[2];
                    string natureOfIncident = words[6]; //Assuming that nature of incident is only 1 word long (will deal with 2 word natures later)
                    if (dataStore.seriousIncidents.Contains(natureOfIncident)) //If serious incident is only 1 word long and is in list of serious incidents then sort code and incident added to report dictionary
                    {
                        if (!dataStore.seriousIncidentReports.ContainsKey(sortCode))
                        {
                            dataStore.seriousIncidentReports.Add(sortCode, natureOfIncident);
                        } else
                        {
                            MessageBox.Show("Sort code " + sortCode + " already entered");
                            return "";
                        }
                    }
                    else if (dataStore.seriousIncidents.Contains(natureOfIncident + " " + words[7])) //If nature of incident (2 word length) is in list of serious incidents then sort code and incident will be added to reports dictionary
                    {
                        if (!dataStore.seriousIncidentReports.ContainsKey(sortCode))
                        {
                            dataStore.seriousIncidentReports.Add(sortCode, natureOfIncident + " " + words[7]); //might replace natureofincident with words[13] if not work
                        }
                        else
                        {
                            MessageBox.Show("Sort code " + sortCode + " already entered");
                            return "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Input for Sort Code or Incident: Make sure spaces are entered correctly in 'Sort Code:' and 'Nature of Incident:'");
                        return "";
                    }
                } catch (Exception e)
                {
                    MessageBox.Show("Message Body too short to process." + e);
                    return "";
                }
            }

            //Goes over each word in the message body and if it is a URL then it is removed
            foreach (string word in words)
            {
                if (word.Contains(@"http://") || word.Contains(@"https://")) //don't know if https one is required but it's there anyway
                {
                    returnMessage += "<URL Quarantined> ";
                    dataStore.urlQuarantineList.Add(word);
                } else
                {
                    returnMessage += word + " ";
                }
            }

            //Returns string with message details
            return "[Sender]: " + sender + " [Subject]: " + subject + " [Message]: " + returnMessage.Trim();
        }
    }
}
