using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NBMMessageFiltering
{
    //This class will be used to create messages of type SMS
    public class SMSMessage : Message
    {
        //SMS Char limit of 140 has been set in CreateMessageViewModel
        private DataStore abbreviations = DataStore.Instance;
        public SMSMessage(string msgID, string[] msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        //Method to process an SMS message
        //SMS will be processed by removing abbreviations
        //Message array contains 2 elements. Index 0 will be phone number and index 1 will be the message
        public override string processMessage(string[] message)
        {
            //these 2 variables will be used to check that the phone number is valid. Phone number will be the entered phone number without spaces
            string phoneNumber = message[0].Replace(" ", string.Empty); //removing all spaces from input string
            string validatePhoneNo = phoneNumber.Substring(1); //will be used to check if phone number only contains digits (starts at index 1 as first char is '+' for international codes

            if (message.Length == 0 || message == null || message[0] == string.Empty || message[1] == string.Empty)
            {
                MessageBox.Show("Invalid Input - No input");
                return "";
            }

            if (phoneNumber.Length > 15)
            {
                MessageBox.Show("Length of phone number details too long.");
                return "";
            }

            if (!validatePhoneNo.All(char.IsDigit))
            {
                MessageBox.Show("Phone number must only contain numbers.");
                return "";
            }

            if (!phoneNumber.StartsWith('+'))
            {
                MessageBox.Show("International code must start with a '+'");
                return "";
            }

            //Messages have character limit of 140
            if (message[1].Length <= 140)
            {
                //Using this to take into account user inputting a double space
                List<string> words = new List<string>(message[1].Split(new char[] { ' ', '\n', '\r' })); //Splitting message body into words
                words.RemoveAll(str => string.IsNullOrWhiteSpace(str)); //Removing any empty elements

                return "[Phone Number]: " + message[0] + " [Message]: " + abbreviations.replaceAbbreviations(words.ToArray(), typeof(SMSMessage));
            } else
            {
                MessageBox.Show("Message Length Exceeded");
                return "";
            }
        }
    }
}
