using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace NBMMessageFiltering.Database
{
    class DataStore
    {
        private static DataStore _instance;

        public List<Message> listOfMessages = new List<Message>(); 

        private DataStore() { }

        public static DataStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataStore();
                }
                return _instance;
            }
        }

        public void saveToJSON()
        {
            string json = JsonSerializer.Serialize(listOfMessages);
            File.WriteAllText(@"..\..\..\outputFile.json", json);
        }
    }
}
