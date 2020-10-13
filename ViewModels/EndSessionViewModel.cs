using NBMMessageFiltering.Commands;
using NBMMessageFiltering.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NBMMessageFiltering.ViewModels
{
    class EndSessionViewModel
    {
        private DataStore dataStore = DataStore.Instance;
        public string TrendingListSubtitleTextBlock { get; private set; }
        public string MentionsListSubtitleTextBlock { get; private set; }
        public string SIRListSubtitleTextBlock { get; private set; }

        public string TrendingListTextBlock { get; set; }
        public string MentionListTextBlock { get; set; }
        public string SIRListTextBlock { get; set; }

        public string CloseApplicationButtonContent { get; private set; }
        public ICommand CloseApplicationButtonCommand { get; private set; }

        public EndSessionViewModel()
        {
            TrendingListSubtitleTextBlock = "Trending List:";
            MentionsListSubtitleTextBlock = "Mentions List:";
            SIRListSubtitleTextBlock = "Significant Incident Report List:";

            foreach (string key in dataStore.trendingDictionary.Keys)
            {
                TrendingListTextBlock += "Hashtag: " + key + ", Number of uses: " + dataStore.trendingDictionary[key] + "\n";
            }

            foreach (string twitterID in dataStore.mentionsList)
            {
                MentionListTextBlock += "Twitter ID: " + twitterID + "\n";
            }

            foreach (string key in dataStore.seriousIncidentReports.Keys)
            {
                SIRListTextBlock += "Sort Code: " + key + ", Incident: " + dataStore.seriousIncidentReports[key] + "\n";
            }

            CloseApplicationButtonContent = "Close Application";
            CloseApplicationButtonCommand = new RelayCommand(CloseApplicationButtonClick);
        }

        private void CloseApplicationButtonClick()
        {
            Application.Current.Shutdown();
        }
    }
}
