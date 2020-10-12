using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBMMessageFiltering
{
    class SMSMessage : Message
    {
        //SMS Char limit of 140 has been set in CreateMessageViewModel
        private DataStore abbreviations = DataStore.Instance;
        public SMSMessage(string msgID, string msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        public override string processMessage(string message)
        {
            string[] words = message.Split(' ');
            return abbreviations.replaceAbbreviations(words, typeof(SMSMessage));
        }
    }
}
