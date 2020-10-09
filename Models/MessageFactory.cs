using NBMMessageFiltering.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NBMMessageFiltering
{
    class MessageFactory
    {
        public Message categoriseMessage(string msgID, string msgBody)
        {
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
