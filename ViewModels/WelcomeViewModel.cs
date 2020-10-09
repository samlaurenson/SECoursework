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
                "- The 'Create Message' button will bring you to a page where you can input your message.\n\n" +
                "- The 'Load From File' button will take you to a page where you can select the file to use as the input data.";
        }
    }
}
