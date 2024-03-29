﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NBMMessageFiltering
{
    //Message types will inherit from this class.
    public abstract class Message
    {
        protected string msgID;
        protected string msgBody;

        public string MsgID
        {
            get { return msgID; }
        }
        public string MsgBody
        {
            get { return msgBody; }
        }

        public abstract string processMessage(string[] message);
    }
}
