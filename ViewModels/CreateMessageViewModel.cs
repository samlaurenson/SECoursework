using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using NBMMessageFiltering.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace NBMMessageFiltering.ViewModels
{
    //This is the view model class for the CreateMessage View
    //This is used when the user presses the "Create Message" button on the side bar
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

        public string SMSInternationalCodeTextBlock { get; private set; }
        public string SMSInternationalCodeTextBox { get; set; }
        public string SMSPhoneNumberTextBlock { get; private set; }
        public string SMSPhoneNumberTextBox { get; set; }
        public string SMSVis { get; set; }

        //Used to create messages
        private MessageFactory messageFactory = new MessageFactory();

        //Constructor to set inital values of text blocks, text boxes, visibility, combo box values
        //and to give commands to buttons
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

            SMSInternationalCodeTextBlock = "International Code:";
            SMSInternationalCodeTextBox = string.Empty;

            SMSPhoneNumberTextBlock = "Phone Number:";
            SMSPhoneNumberTextBox = string.Empty;
            SMSVis = "Hidden";
        }

        //Method that will run when user presses "Send Message" button
        //This method will create the message, validate it and if everything is valid it will be added to the list of messages
        //and will be displayed in the "Activity" text block to show what data has been added to the system
        //If there is an error then a message box will be shown to give a brief description of why the error occurred 
        public void SendButtonClick()
        {
            string typetext = ""; //Used for activity text block to show what type of message has been added

            List<string> createBody = new List<string>(); //List used for collating all the information required to make a body of a type of message
            //For example, if it were an email message - index 0 would be sender, index 1 would be subject and index 2 would be the main body of the message

            string subString = MessageIDTextBox.Substring(1);
            int checkNumber = 0; //variable used to check if characters in ID (not including letter indicator at start) are numbers

            if (MessageIDTextBox.Length != 10 || !int.TryParse(subString, out checkNumber)) //Checks if ID is 9 numbers long (not including the S, E or T at start)
            {
                MessageBox.Show("Invalid ID - Must be 10 characters long. 1 type character followed by 9 numbers.");
                return;
            }

            //Checking message body has information inside it
            if (string.IsNullOrWhiteSpace(MessageBodyTextBox))
            {
                MessageBox.Show("No message body input");
                return;
            }

            //If statements to check that fields (not including the body of the message) have been filled
            //Will then add this information to the createBody list
            if (MessageType.Equals("SMS Message") && !string.IsNullOrWhiteSpace(SMSInternationalCodeTextBox) && !string.IsNullOrWhiteSpace(SMSPhoneNumberTextBox))
            {
                typetext = "SMS";
                createBody.Add(SMSInternationalCodeTextBox.Trim() + SMSPhoneNumberTextBox.Trim());
            } else if (MessageType.Equals("Twitter Message") && !string.IsNullOrWhiteSpace(TwitterIDTextBox))
            {
                typetext = "Twitter";
                createBody.Add(TwitterIDTextBox.Trim());
            } else if (MessageType.Equals("Email Message") && !string.IsNullOrWhiteSpace(EmailSenderTextBox) && !string.IsNullOrWhiteSpace(EmailSubjectTextBox)) //Max length of 20 for subject and 1028 for body are set in xaml
            {
                typetext = "Email";
                createBody.Add(EmailSenderTextBox.Trim());
                createBody.Add(EmailSubjectTextBox.Trim());

            } else
            {
                MessageBox.Show("Missing Details - Make sure all fields are filled and that you have pressed 'Apply Type' button after selecting type");
                return;
            }

            createBody.Add(MessageBodyTextBox);

            //Creating the message
            Message message = messageFactory.categoriseMessage(MessageIDTextBox, createBody.ToArray());

            //If message ID and body contain information - add to list of messages and display the message for the user to see
            //Else, add message to activity box for user to see that there was an error sending message
            if (!string.IsNullOrEmpty(message.MsgID) && !string.IsNullOrEmpty(message.MsgBody))
            {
                ActivityTextBlock += "[Sent " + typetext + " message] [ID]: " + message.MsgID + " [Body]: " + message.MsgBody + "\n";
                OnChanged(nameof(ActivityTextBlock));

                dataStore.listOfMessages.Add(message);
                dataStore.saveToJSON();
                ClearButtonClick(); //Used to clear the text boxes for next input
            } else
            {
                ActivityTextBlock += "Error Sending Message.\n";
                OnChanged(nameof(ActivityTextBlock));
            }
        }

        //Empties all text boxes
        public void ClearButtonClick()
        {
            MessageIDTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;
            TwitterIDTextBox = string.Empty;
            EmailSenderTextBox = string.Empty;
            EmailSubjectTextBox = string.Empty;
            SMSInternationalCodeTextBox = string.Empty;
            SMSPhoneNumberTextBox = string.Empty;
            OnChanged(MessageIDTextBox);
            OnChanged(MessageBodyTextBox);
            OnChanged(TwitterIDTextBox);
            OnChanged(EmailSenderTextBox);
            OnChanged(EmailSubjectTextBox);
            OnChanged(SMSInternationalCodeTextBox);
            OnChanged(SMSPhoneNumberTextBox);
        }

        //Used to apply visibility for fields when clicked. For example, when selecting email from combo box - apply type button should be pressed
        //This will cause fields irrelevant to email to be hidden (Twitter ID input) and will make email input boxes visible
        public void ApplyButtonClick()
        {
            if (MessageType == null)
            {
                MessageBox.Show("No type selected");
            } 
            else if (MessageType.Equals("Email Message"))
            {
                //Making fields for email inputs visible while hiding and removing data entered in irrelevant fields
                EmailVis = "Visible";
                OnChanged(nameof(EmailVis));
                TwitterVis = "Hidden";
                OnChanged(nameof(TwitterVis));
                SMSVis = "Hidden";
                OnChanged(nameof(SMSVis));
                SMSInternationalCodeTextBox = string.Empty;
                SMSPhoneNumberTextBox = string.Empty;
                TwitterIDTextBox = string.Empty;
                OnChanged(TwitterIDTextBox);
            }
            else if (MessageType.Equals("SMS Message"))
            {
                //Making fields for SMS inputs visible while hiding twitter and email fields and removing data entered in irrelevant fields
                SMSVis = "Visible";
                OnChanged(nameof(SMSVis));
                EmailVis = "Hidden";
                OnChanged(nameof(EmailVis));
                TwitterVis = "Hidden";
                OnChanged(nameof(TwitterVis));
                TwitterIDTextBox = string.Empty;
                EmailSenderTextBox = string.Empty;
                EmailSubjectTextBox = string.Empty;
                OnChanged(TwitterIDTextBox);
                OnChanged(EmailSenderTextBox);
                OnChanged(EmailSubjectTextBox);
            }
            else if (MessageType.Equals("Twitter Message"))
            {
                //Making fields for twitter inputs visible while hiding and removing data entered in irrelevant fields
                EmailSenderTextBox = string.Empty;
                EmailSubjectTextBox = string.Empty;
                OnChanged(EmailSenderTextBox);
                OnChanged(EmailSubjectTextBox);
                EmailVis = "Hidden";
                OnChanged(nameof(EmailVis));
                SMSVis = "Hidden";
                OnChanged(nameof(SMSVis));
                SMSInternationalCodeTextBox = string.Empty;
                SMSPhoneNumberTextBox = string.Empty;
                TwitterVis = "Visible";
                OnChanged(nameof(TwitterVis));
            }
        }
    }
}
