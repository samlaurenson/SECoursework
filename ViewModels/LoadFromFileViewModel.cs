using Microsoft.Win32;
using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            //OnChanged(nameof(DataGrid));
            if (!FileLocationTextBlock.Equals(""))
            {
                try
                {
                    //Could move all this into DataStore if want to make it neater and have it return a string which would be the [LOADED MESSAGE] bit
                    StreamReader reader = new StreamReader(FileLocationTextBlock);
                    string line;
                    string[] read;

                    while ((line=reader.ReadLine()) != null)
                    {
                        read = line.Split(',');

                        //If item has 2 populated columns - then process the message
                        if (read[0].Length != 0 && read[1].Length != 0)
                        {
                            Message message = messageFactory.categoriseMessage(read[0], read[1]); //Using message factory to take input data and create a message from it

                            if (message != null && !dataStore.listOfMessages.Exists(x => x.MsgID == message.MsgID))
                            {
                                dataStore.listOfMessages.Add(message); //Storing message into list of messages
                                LoadingDataTextBlock += "[LOADED IN MESSAGE] [ID]: " + message.MsgID + " [BODY]: " + message.MsgBody + "\n";
                                OnChanged(nameof(LoadingDataTextBlock));
                                DataGridDisplay.Add(message);

                            }
                            else
                            {
                                LoadingDataTextBlock += "[ALREADY LOADED DATA] [ID]: " + message.MsgID + " [BODY]: " + message.MsgBody + "\n";
                                OnChanged(nameof(LoadingDataTextBlock));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Cannot read file - Too few columns in file: " + e);
                }
            } else
            {
                MessageBox.Show("No file selected - Please select a file to load.");
            }
            dataStore.saveToJSON();
        }
    }
}
