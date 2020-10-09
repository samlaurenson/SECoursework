using System;
using System.Collections.Generic;
using System.Text;

namespace NBMMessageFiltering.Models
{
    class TwitterMessage : Message
    {
        public TwitterMessage(string msgID, string msgBody)
        {
            this.msgID = msgID;
            this.msgBody = msgBody;
        }

        public override string processMessage(string message)
        {
            return "";
        }
    }
}
