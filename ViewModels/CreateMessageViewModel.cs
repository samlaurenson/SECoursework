using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using NBMMessageFiltering.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NBMMessageFiltering.ViewModels
{
    class CreateMessageViewModel : BaseViewModel
    {
        private DataStore dataStore = DataStore.Instance;
        public string MessageIDTextBlock { get; private set; }
        public string MessageIDTextBox { get; set; }

        public string MessageBodyTextBlock { get; private set; }
        public string MessageBodyTextBox { get; set; }

        public string ActivitySubTitleTextBlock { get; private set; }
        public string ActivityTextBlock { get; private set; }

        public string SendButtonText { get; private set; }
        public string ClearButtonText { get; private set; }
        public ICommand SendButtonCommand { get; private set; }
        public ICommand ClearButtonCommand { get; private set; }

        public MessageFactory messageFactory = new MessageFactory();

        public CreateMessageViewModel()
        {
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
        }

        public void SendButtonClick()
        {
            Message message = messageFactory.categoriseMessage(MessageIDTextBox, MessageBodyTextBox);
            if (message == null)
            {
                MessageBox.Show("Invalid Type - Please check inputs");
                return;
            }
            string typetext = "";
            if (message.GetType().Equals(typeof(TwitterMessage)) && MessageBodyTextBox.Length <= 140) //Twitter message has max of 140 chars
            {
                typetext = "Twitter";
            } else if (message.GetType().Equals(typeof(EmailMessage)))
            {
                typetext = "Email";
            } else if (message.GetType().Equals(typeof(SMSMessage)))
            {
                typetext = "SMS";
            } else
            {
                MessageBox.Show("Error sending message - Too many characters");
                return;
            }
            
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
            OnChanged(MessageIDTextBox);
            OnChanged(MessageBodyTextBox);
        }
    }
}
