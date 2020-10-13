using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NBMMessageFiltering.Models
{
    class EmailMessage : Message
    {

        private DataStore dataStore = DataStore.Instance;
        public EmailMessage(string msgID, string msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        public override string processMessage(string message)
        {
            string[] words = message.Split(new char[] {' ', '\n', '\r'});

            string returnMessage = "";

            //This will rely on the inputs being correctly spaced as specified in document.
            if (words[3].Contains("SIR")) //Checking if subject is a Significant Incident Report (SIR) - index 3 in array will be subject
            {
                try
                {
                    string sortCode = words[8];
                    string natureOfIncident = words[13]; //Assuming nature of incident is 2 words long.
                    //+ " " + words[12]
                    if (dataStore.seriousIncidents.Contains(natureOfIncident)) //If nature of incident (2 word length) is in list of serious incidents then sort code and incident will be added to reports dictionary
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
                    else if (dataStore.seriousIncidents.Contains(words[13] + " " + words[14])) //If serious incident is only 1 word long and is in list of serious incidents then sort code and incident added to report dictionary
                    {
                        if (!dataStore.seriousIncidentReports.ContainsKey(sortCode))
                        {
                            dataStore.seriousIncidentReports.Add(sortCode, words[13] + " " + words[14]);
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
                    MessageBox.Show("Message Body too short to process.");
                    return "";
                }
            }

            foreach (string word in words)
            {
                if (word.Contains(@"http:\\"))
                {
                    returnMessage += "<URL Quarantined> ";
                    dataStore.urlQuarantineList.Add(word);
                } else
                {
                    returnMessage += word + " ";
                }
            }
            return returnMessage;
        }
    }
}
