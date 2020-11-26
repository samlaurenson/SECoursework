using NBMMessageFiltering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBMMessageFiltering
{
    //This class will be used to categorise the user input into a message type depending on the message ID
    public class MessageFactory
    {
        public Message categoriseMessage(string msgID, string[] msgBody)
        {
            
            //checks if ID is null before checking type of message in order to prevent errors when inputting a null (or empty) string
            if (string.IsNullOrWhiteSpace(msgID))
            {
                return null;
            }

            //Now checks that the ID is valid. ID should be 10 characters long. 1 char and 9 digits.
            string checkID = msgID.Substring(1);
            int checkIntID = 0;

            //Checking that ID has 9 digits after the message type character
            if (!checkID.All(char.IsDigit) || msgID.Length != 10)
            {
                return null;
            }

            if (msgID[0].Equals('S'))
            {
                return new SMSMessage(msgID, msgBody);
            } else if (msgID[0].Equals('E'))
            {
                return new EmailMessage(msgID, msgBody);
            } else if (msgID[0].Equals('T'))
            {
                return new TwitterMessage(msgID, msgBody);
            } else
            {
                return null;
            }
        }
    }
}
