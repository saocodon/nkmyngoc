﻿using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NhakhoaMyNgoc.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public string AppVersion => $"v{Assembly.GetExecutingAssembly().GetName().Version!.ToString(2)}";
        public AboutWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
