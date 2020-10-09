using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NBMMessageFiltering.Database
{
    //Singleton
    class AbbreviationsStore
    {
        private static AbbreviationsStore _instance;

        private const string abbreviationsFile = @"..\..\..\textwords.csv";
        public Hashtable abbreviations = new Hashtable();

        private AbbreviationsStore() { }

        public static AbbreviationsStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AbbreviationsStore();
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

        public string replaceAbbreviations(string[] words)
        {
            string processedMessage = "";

            foreach (string word in words)
            {
                int length = word.Length-1;
                string wordWithoutPunctuation = "";
                //char removedPunct = '\0'; //Used to store the removed punctuation to later add on after abbreviation has been swapped out

                //If word ends with common punctuation, then it will take the last character and store it in removedPunct and take the word minus the punctuation and store
                //it in wordWithoutPunctuation
                if (word[length].Equals('.') || word[length].Equals(',') || word[length].Equals('!') || word[length].Equals('?'))
                {
                    wordWithoutPunctuation = word;
                    //removedPunct = wordWithoutPunctuation[length];
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
    }
}
