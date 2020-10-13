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
        private DataStore abbreviations = DataStore.Instance;
        
        public string CreateMessageButtonContent { get; private set; }
        public string LoadFromFileButtonContent { get; private set; }
        public string EndSessionButtonContent { get; private set; }

        public ICommand CreateMessageButtonCommand { get; private set; }
        public ICommand LoadFromFileButtonCommand { get; private set; }
        public ICommand EndSessionButtonCommand { get; private set; }

        public UserControl ContentControlBinding { get; private set; }

        public MainWindowViewModel()
        {
            abbreviations.loadAbbreviations(); //Loading abbreviations on application start up for later use

            CreateMessageButtonContent = "Create Message";
            LoadFromFileButtonContent = "Load From File";
            EndSessionButtonContent = "End Session";

            LoadFromFileButtonCommand = new RelayCommand(LoadFromFileButtonClick);
            CreateMessageButtonCommand = new RelayCommand(CreateMessageButtonClick);
            EndSessionButtonCommand = new RelayCommand(EndSessionButtonClick);
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

        private void EndSessionButtonClick()
        {
            ContentControlBinding = new EndSessionView();
            OnChanged(nameof(ContentControlBinding));
        }
    }
}
