using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBMMessageFiltering
{
    class SMSMessage : Message
    {

        private AbbreviationsStore abbreviations = AbbreviationsStore.Instance;
        public SMSMessage(string msgID, string msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        public override string processMessage(string message)
        {
            string[] words = message.Split(' ');
            string processedMessage = abbreviations.replaceAbbreviations(words);
            return processedMessage;
        }
    }
}
