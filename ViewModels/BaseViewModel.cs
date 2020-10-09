using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NBMMessageFiltering.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
