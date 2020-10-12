using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using NBMMessageFiltering.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NBMMessageFiltering.ViewModels
{
    class CreateMessageViewModel : BaseViewModel
    {
        private DataStore dataStore = DataStore.Instance;

        public string MessageTypeTextBlock { get; private set; }

        public string MessageIDTextBlock { get; private set; }
        public string MessageIDTextBox { get; set; }

        public string MessageBodyTextBlock { get; private set; }
        public string MessageBodyTextBox { get; set; }

        public string ActivitySubTitleTextBlock { get; private set; }
        public string ActivityTextBlock { get; private set; }

        public string SendButtonText { get; private set; }
        public string ClearButtonText { get; private set; }
        public string ApplyTypeButtonText { get; private set; }
        public ICommand SendButtonCommand { get; private set; }
        public ICommand ClearButtonCommand { get; private set; }
        public ICommand ApplyTypeButtonCommand { get; private set; }

        public ObservableCollection<string> MessageTypeComboBox { get; set; }
        public string MessageType { get; set; }

        public string EmailSubjectTextBlock { get; private set; }
        public string EmailSubjectTextBox { get; set; }
        public string EmailSenderTextBlock { get; private set; }
        public string EmailSenderTextBox { get; set; }
        public string EmailVis { get; set; }

        public string TwitterIDTextBlock { get; private set; }
        public string TwitterIDTextBox { get; set; }
        public string TwitterVis { get; set; }

        public MessageFactory messageFactory = new MessageFactory();

        public CreateMessageViewModel()
        {
            MessageTypeTextBlock = "Message Type: ";
            MessageIDTextBlock = "Message ID: ";
            MessageBodyTextBlock = "Message Body: ";

            MessageIDTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;

            ActivitySubTitleTextBlock = "Activity: ";
            ActivityTextBlock = string.Empty;

            SendButtonText = "Send Message";
            SendButtonCommand = new RelayCommand(SendButtonClick);

            ClearButtonText = "Clear";
            ClearButtonCommand = new RelayCommand(ClearButtonClick);

            ApplyTypeButtonText = "Click to apply type";
            ApplyTypeButtonCommand = new RelayCommand(ApplyButtonClick);

            MessageTypeComboBox = new ObservableCollection<string>();
            MessageTypeComboBox.Add("SMS Message");
            MessageTypeComboBox.Add("Email Message");
            MessageTypeComboBox.Add("Twitter Message");

            EmailSubjectTextBlock = "Subject: ";
            EmailSubjectTextBox = string.Empty;

            EmailSenderTextBlock = "Sender: ";
            EmailSenderTextBox = string.Empty;
            EmailVis = "Hidden";

            TwitterIDTextBlock = "Twitter ID: ";
            TwitterIDTextBox = string.Empty;
            TwitterVis = "Hidden";
        }

        public void SendButtonClick()
        {
            string msgType = "";
            string typetext = "";

            string addToMessageBody = "";
            int checkNumber = 0;
            if (MessageIDTextBox.Length != 9 || !int.TryParse(MessageIDTextBox.Trim(), out checkNumber)) //Checks if ID is 9 numbers long (not including the S, E or T at start
            {
                MessageBox.Show("Invalid ID - Must be 9 numbers long");
                return;
            }

            if (MessageBodyTextBox.Length == 0)
            {
                MessageBox.Show("No message body input");
                return;
            }

            if (MessageType.Equals("SMS Message") && MessageBodyTextBox.Length < 140)
            {
                msgType = "S";
                typetext = "SMS";
            } else if (MessageType.Equals("Twitter Message") && MessageBodyTextBox.Length < 140)
            {
                if (TwitterIDTextBox != string.Empty) //Length of twitter ID set in xaml (not including the @ which will be added on)
                {
                    msgType = "T";
                    typetext = "Twitter";
                    addToMessageBody += "Twitter ID: @" + TwitterIDTextBox + " Message: ";
                } else
                {
                    MessageBox.Show("TwitterID is empty. (Make sure that you have pressed 'Apply Type' button after selecting type and that boxes are filled)");
                    return;
                }
            } else if (MessageType.Equals("Email Message")) //Max length of 20 for subject and 1028 for body are set in xaml
            {
                if (EmailSenderTextBox != string.Empty && EmailSubjectTextBox != string.Empty)
                {
                    msgType = "E";
                    typetext = "Email";
                    addToMessageBody += "Sender: " + EmailSenderTextBox + " Subject: " + EmailSubjectTextBox + " Message: "; // see if i can find the email regex from last year
                } else
                {
                    MessageBox.Show("Sender or Subject is empty. (Make sure that you have pressed 'Apply Type' button after selecting type and that boxes are filled)");
                    return;
                }
                
            } else
            {
                MessageBox.Show("Error sending message - Too many characters");
                return;
            }

            Message message = messageFactory.categoriseMessage(msgType + MessageIDTextBox, addToMessageBody + MessageBodyTextBox);
            
            
            ActivityTextBlock += "[Sent "+typetext+" message] [ID]: " + message.MsgID + " [Body]: " + message.MsgBody + "\n";
            OnChanged(nameof(ActivityTextBlock));

            dataStore.listOfMessages.Add(message);
            dataStore.saveToJSON();

            //Now add details to list of messages to be saved

            ClearButtonClick(); //Used to clear the text boxes for next input
        }

        public void ClearButtonClick()
        {
            MessageIDTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;
            TwitterIDTextBox = string.Empty;
            EmailSenderTextBox = string.Empty;
            EmailSubjectTextBox = string.Empty;
            OnChanged(MessageIDTextBox);
            OnChanged(MessageBodyTextBox);
            OnChanged(TwitterIDTextBox);
            OnChanged(EmailSenderTextBox);
            OnChanged(EmailSubjectTextBox);
        }

        public void ApplyButtonClick()
        {
            if (MessageType == null)
            {
                MessageBox.Show("No type selected");
            } 
            else if (MessageType.Equals("Email Message"))
            {
                EmailVis = "Visible";
                OnChanged(nameof(EmailVis));
                TwitterVis = "Hidden";
                OnChanged(nameof(TwitterVis));
            }
            else if (MessageType.Equals("SMS Message"))
            {
                EmailVis = "Hidden";
                OnChanged(nameof(EmailVis));
                TwitterVis = "Hidden";
                OnChanged(nameof(TwitterVis));
            }
            else if (MessageType.Equals("Twitter Message"))
            {
                EmailVis = "Hidden";
                OnChanged(nameof(EmailVis));
                TwitterVis = "Visible";
                OnChanged(nameof(TwitterVis));
            }
        }
    }
}
