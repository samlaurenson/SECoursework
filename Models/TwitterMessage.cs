using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace NBMMessageFiltering.Models
{
    class TwitterMessage : Message
    {
        //3.1.3	Tweets: Textspeak abbreviations will be expanded (as in SMS messages above). Hashtags will be added to a hashtag list that will count the number of uses of each to produce a trending list. “Mentions”, i.e. embedded Twitter IDs will be added to a mentions list.

        //Could use hashtable to store Key as Hashtag and value would be an integer starting from 1 and if hashtag is in hashtable then add 1 to value, if not then add to hashtable with value of 1. This will be used to produce a "trending list"
        //"Mentions" (embedded twitter ID's) will be used to create a mentions list. Not sure if this includes the sender's @ (probably does). Mentions start with an @ followed by 15 characters. Tweets have maximum of 140 characters.

        private DataStore abbreviations = DataStore.Instance;

        public TwitterMessage(string msgID, string msgBody)
        {
            this.msgID = msgID;
            this.msgBody = processMessage(msgBody);
        }

        public override string processMessage(string message)
        {
            string[] words = message.Split(' ');
            return abbreviations.replaceAbbreviations(words, typeof(TwitterMessage));
        }
    }
}
