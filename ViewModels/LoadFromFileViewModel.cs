using Microsoft.Win32;
using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace NBMMessageFiltering.ViewModels
{
    class LoadFromFileViewModel : BaseViewModel
    {
        private DataStore dataStore = DataStore.Instance;
        private MessageFactory messageFactory = new MessageFactory();
        public string FileLocationLabelTextBlock { get; private set; }
        public string FileLocationTextBlock { get; private set; }
        public string ChangeLocationButtonContent { get; private set; }
        public ICommand ChangeLocationButtonCommand { get; private set; }
        public string LoadFileButtonContent { get; private set; }
        public ICommand LoadFileButtonCommand { get; private set; }
        public string LoadingDataSubtitleTextBlock { get; private set; }
        public string LoadingDataTextBlock { get; private set; }
        public string DataLoadedIntoSystem { get; private set; }
        public ObservableCollection<Message> DataGridDisplay { get; set; }

        public LoadFromFileViewModel()
        {
            FileLocationLabelTextBlock = "File Location: ";
            FileLocationTextBlock = @"..\..\..\testInputs1.csv"; //Default file path - can be changed with button

            ChangeLocationButtonContent = "Change Location...";
            ChangeLocationButtonCommand = new RelayCommand(ChangeLocationButtonClick);

            LoadFileButtonContent = "Load File";
            LoadFileButtonCommand = new RelayCommand(LoadFileButtonClick);

            LoadingDataSubtitleTextBlock = "Loading data:";
            LoadingDataTextBlock = string.Empty;

            DataLoadedIntoSystem = "Messages Loaded In:";

            DataGridDisplay = new ObservableCollection<Message>();
        }

        private void ChangeLocationButtonClick()
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = "CSV Files (*.csv)|*csv"; //Only showing selectable files as csv files
            browseFile.FilterIndex = 1; //Only select 1 file
            browseFile.ShowDialog();
            FileLocationTextBlock = browseFile.FileName;
            OnChanged(nameof(FileLocationTextBlock)); //Updating text block to show file selected
        }

        //Only load in csv files. Index 0 will be the message ID and Index 1 will be the message body
        private void LoadFileButtonClick()
        {
            if (!FileLocationTextBlock.Equals(""))
            {
                try
                {
                    //Could move all this into DataStore if want to make it neater and have it return a string which would be the [LOADED MESSAGE] bit
                    StreamReader reader = new StreamReader(FileLocationTextBlock);
                    string line;
                    string[] lineSplitter;

                    while ((line=reader.ReadLine()) != null)
                    {
                        List<string> read = new List<string>();
                        List<string> createBody = new List<string>();

                        lineSplitter = line.Split(','); //Splitting line into columns

                        //Will loop over the line and add columns to a list that do not contain blanks/white space
                        //This list will be used to create message based on number of items in the list. For example, an SMS message would only have ID and body, so length 2.
                        //Whereas email would have ID, Sender, Subject and body, so length of 4.
                        for (int i = 0; i < lineSplitter.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(lineSplitter[i]))
                            {
                                read.Add(lineSplitter[i]);
                            }
                        }

                        //Could do if count AND first char in 1st element is S, E or T
                        if(read.Count == 4 && read[0][0].Equals('E'))
                        {
                            //indexes are - msg id, sender, subject, msg body
                            if (read[0].Length != 0 && read[1].Length != 0 && read[2].Length != 0 && read[3].Length != 0)
                            {
                                if (CheckLoadedData(read[0]))
                                {
                                    for (int i = 1; i < read.Count; i++)
                                    {
                                        createBody.Add(read[i].Trim());
                                    }
                                    AddDataToSystem(read[0], createBody.ToArray());
                                }
                            }
                        } else if (read.Count == 3 && read[0][0].Equals('T'))
                        {
                            //indexes are - msg id, twitter id, msg body
                            if (read[0].Length != 0 && read[1].Length != 0 && read[2].Length != 0)
                            {
                                if (CheckLoadedData(read[0]))
                                {
                                    for (int i = 1; i < read.Count; i++)
                                    {
                                        createBody.Add(read[i].Trim());
                                    }
                                    AddDataToSystem(read[0], createBody.ToArray());
                                }
                            }
                        } else if (read.Count == 3 && read[0][0].Equals('S'))
                        {
                            //If item has 2 populated columns - then process the message
                            //indexes are - msg id, phone number (including international code), msg body
                            if (read[0].Length != 0 && read[1].Length != 0 && read[2].Length != 0)
                            {
                                if (CheckLoadedData(read[0]))
                                {
                                    for (int i = 1; i < read.Count; i++)
                                    {
                                        //If statement to add the '+' to the start of the phone number (for international code)
                                        //Checking if i is 1 as the phone number will be index 1 of the line being read from
                                        if (i == 1)
                                        {
                                            createBody.Add("+" + read[i].Trim());
                                        } else
                                        {
                                            createBody.Add(read[i].Trim());
                                        }
                                    }
                                    AddDataToSystem(read[0], createBody.ToArray());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Cannot read file - Too few columns entered for set of data: " + e);
                }
            } else
            {
                MessageBox.Show("No file selected - Please select a file to load.");
            }
            dataStore.saveToJSON();
        }

        //Method to check if data has already been loaded in to system. If it has then it will return false (telling program not to add the data again)
        //Returning true lets program carry on with processing and add data
        private Boolean CheckLoadedData(string msgID)
        {
            string checkID = msgID.Substring(1);
            int checkIntID = 0;
            if (int.TryParse(checkID, out checkIntID) && msgID.Length == 10)
            {
                if (msgID != null && !dataStore.listOfMessages.Exists(x => x.MsgID == msgID))
                {
                    LoadingDataTextBlock += "[LOADING IN MESSAGE] [ID]: " + msgID + "\n";
                    OnChanged(nameof(LoadingDataTextBlock));
                    return true;
                }
                else
                {
                    LoadingDataTextBlock += "[ALREADY LOADED DATA] [ID]: " + msgID + "\n";
                    OnChanged(nameof(LoadingDataTextBlock));
                    return false;
                }
            } else
            {
                LoadingDataTextBlock += "ERROR LOADING DATA - ID invalid. Must start with an 'S', 'E' or 'T' and have 9 digits.\n";
                OnChanged(nameof(LoadingDataTextBlock));
                return false;
            }
        }

        private void AddDataToSystem(string msgID, string[] msgBody)
        {
            Message message = messageFactory.categoriseMessage(msgID, msgBody); //Using message factory to take input data and create a message from it
            if (!string.IsNullOrEmpty(message.MsgID) && !string.IsNullOrEmpty(message.MsgBody))
            {
                dataStore.listOfMessages.Add(message); //Storing message into list of messages
                DataGridDisplay.Add(message);
            }
        }
    }
}
