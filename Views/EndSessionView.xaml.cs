﻿using NBMMessageFiltering.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NBMMessageFiltering.Views
{
    /// <summary>
    /// Interaction logic for EndSessionView.xaml
    /// </summary>
    public partial class EndSessionView : UserControl
    {
        public EndSessionView()
        {
            InitializeComponent();

            this.DataContext = new EndSessionViewModel();
        }
    }
}
