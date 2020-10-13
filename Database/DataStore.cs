using NBMMessageFiltering.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace NBMMessageFiltering.Database
{
    //Singleton - so that the program doesn't have to load in the same abbreviations list multiple times
    //And used to store all the data for saving and creating lists - mentions list, trending list and saving messages to JSON file
    class DataStore
    {
        private static DataStore _instance;

        private const string abbreviationsFile = @"..\..\..\textwords.csv";
        public Hashtable abbreviations = new Hashtable();

        public List<Message> listOfMessages = new List<Message>();
        public Dictionary<string, int> trendingDictionary = new Dictionary<string, int>();
        public List<string> mentionsList = new List<string>();

        public List<string> urlQuarantineList = new List<string>();

        public Dictionary<string, string> seriousIncidentReports = new Dictionary<string, string>();
        public List<string> seriousIncidents = new List<string>
        {
            "Theft",
            "Staff Attack",
            "ATM Theft",
            "Raid",
            "Customer Attack",
            "Staff Abuse",
            "Bomb Threat",
            "Terrorism",
            "Suspicious Incident",
            "Intelligence",
            "Cash Loss"
        };

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

        public Hashtable returnHashtable()
        {
            return abbreviations;
        }

        public void loadAbbreviations()
        {
            StreamReader reader = new StreamReader(abbreviationsFile);
            string line;
            string[] read;

            while ((line=reader.ReadLine())!=null)
            {
                read = line.Split(',');
                abbreviations.Add(read[0], read[1]);
            }
        }

        public string replaceAbbreviations(string[] words, object typeOfMessage)
        {
            string processedMessage = "";

            foreach (string word in words)
            {

                //If message is a twitter message, it will also check for hashtags. If word starts with hashtag and is not in dictionary, then add it to dictionary and set value (count of hashtags) to 1.
                //If it is already in the dictionary then it will add 1 to its value.
                if (typeOfMessage.Equals(typeof(TwitterMessage)))
                {
                    if (word[0].Equals('#'))
                    {
                        if (trendingDictionary.ContainsKey(word))
                        {
                            trendingDictionary[word]++;
                        }
                        else
                        {
                            trendingDictionary.Add(word, 1);
                        }
                    } 
                    else if (word[0].Equals('@'))
                    {
                        if (!mentionsList.Contains(word))
                        {
                            mentionsList.Add(word);
                        }
                    }
                }

                int length = word.Length-1;
                string wordWithoutPunctuation = "";

                if (word[length].Equals('.') || word[length].Equals(',') || word[length].Equals('!') || word[length].Equals('?'))
                {
                    wordWithoutPunctuation = word;
                    wordWithoutPunctuation = wordWithoutPunctuation.Remove(length); //Removing last char in string if last char is punctuation
                }

                //Check if word is an abbreviation
                if (abbreviations.ContainsKey(word))
                {
                    processedMessage += word + "<"+abbreviations[word]+">" + " ";
                } else if (abbreviations.ContainsKey(wordWithoutPunctuation) && !wordWithoutPunctuation.Equals("")) //Checking if word without punctuation is an abbreviation
                {
                    processedMessage += word + "<"+abbreviations[wordWithoutPunctuation]+">" + " ";
                } else
                {
                    processedMessage += word + " ";
                }
            }

            return processedMessage.Remove(processedMessage.Length-1); //return complete message and remove space char at the end
        }

        public void saveToJSON()
        {
            string json = JsonSerializer.Serialize(listOfMessages);
            File.WriteAllText(@"..\..\..\outputFile.json", json);
        }
    }
}
