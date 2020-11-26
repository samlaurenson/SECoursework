using System;
using System.Collections.Generic;
using System.Text;

namespace NBMMessageFiltering.ViewModels
{
    class WelcomeViewModel
    {
        public string titleTextBlock { get; private set; }
        public string descriptionTextBlock { get; private set; }
        public WelcomeViewModel()
        {
            titleTextBlock = "Welcome to Napier Banking Message Filtering Service";
            descriptionTextBlock = "To use this service: Use the navigation buttons on the left. \n\n" +
                "- The 'Create Message' button will bring you to a page where you can input your message. Be sure to select the message type from the combo box and apply the type. After that you insert 9 numbers into the message ID text box.\n\n" +
                "- The 'Load From File' button will take you to a page where you can select the file to use as the input data.\n\n" + 
                "- The 'End Session' button will show you the details of the session (mentions list, hashtag list and URL quarantine list) and provide a button used to close the program.\n\n" +
                "The program will gather all the details of the message and create a message of either SMS, Twitter or Email type and will process the message accordingly.";
        }
    }
}
