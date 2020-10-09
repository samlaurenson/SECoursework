using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using NBMMessageFiltering.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace NBMMessageFiltering.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private AbbreviationsStore abbreviations = AbbreviationsStore.Instance;
        
        public string CreateMessageButtonContent { get; private set; }
        public string LoadFromFileButtonContent { get; private set; }

        public ICommand CreateMessageButtonCommand { get; private set; }
        public ICommand LoadFromFileButtonCommand { get; private set; }

        public UserControl ContentControlBinding { get; private set; }

        public MainWindowViewModel()
        {
            abbreviations.loadAbbreviations(); //Loading abbreviations on application start up for later use

            CreateMessageButtonContent = "Create Message";
            LoadFromFileButtonContent = "Load From File";

            LoadFromFileButtonCommand = new RelayCommand(LoadFromFileButtonClick);
            CreateMessageButtonCommand = new RelayCommand(CreateMessageButtonClick);
            ContentControlBinding = new WelcomeView();
        }

        private void CreateMessageButtonClick()
        {
            //Message message = messageFactory.categoriseMessage(MessageHeaderTextBox, MessageBodyTextBox);
            ContentControlBinding = new CreateMessageView();
            OnChanged(nameof(ContentControlBinding));
        }

        private void LoadFromFileButtonClick()
        {
            ContentControlBinding = new LoadFromFileView();
            OnChanged(nameof(ContentControlBinding));
        }
    }
}
