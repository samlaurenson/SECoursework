using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NBMMessageFiltering.Models
{
    //This class will be used to create Twitter messages
    public class TwitterMessage : Message
    {
        private DataStore abbreviations = DataStore.Instance;

        public TwitterMessage(string msgID, string[] msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        //Method used to process Twitter messages
        //Processed by removing abbreviations, gathering user mentions and hashtags.
        //message array contains 2 items. Index 0 will have the username of the sender and Index 1 will have the message.
        public override string processMessage(string[] message)
        {
            if (message.Length == 0 || message == null)
            {
                MessageBox.Show("Invalid Input - No input");
                return "";
            }

            //If sender does not have an @ sign in front of their user name then it will be placed for them
            //Allows the user to input either '@username' or 'username'
            if (!message[0][0].Equals('@'))
            {
                message[0] = "@" + message[0];
            }

            //setting invalid length to 16 to take into account the '@' at the start of the user name
            if (message[0].Length > 16) //Checking that twitter ID has valid length and contains the @ symbol
            {
                MessageBox.Show("Invalid Twitter ID - ID exceedes length");
                return "";
            }

            if (message[1].Length <= 140)
            {
                string messageAsString = message[0] + " " + message[1]; //Combining twitter ID and body of message together as twitter ID is counted as being in the body

                //Splitting message body into words. Doing it as a list so can go through and remove any empty elements (in case user has entered a double space)
                List<string> words = new List<string>(messageAsString.Split(new char[] { ' ', '\n', '\r' }));
                words.RemoveAll(str => string.IsNullOrWhiteSpace(str)); //Removing any empty elements

                return abbreviations.replaceAbbreviations(words.ToArray(), typeof(TwitterMessage));
            } else
            {
                MessageBox.Show("Message length exceeded");
                return "";
            }
        }
    }
}
