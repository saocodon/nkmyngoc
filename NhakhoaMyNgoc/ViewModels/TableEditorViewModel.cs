using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class TableEditorViewModel<T> : ObservableObject
    {
        [ObservableProperty]
        private object currentVM = new();

        [ObservableProperty]
        private string title = string.Empty;
    }
}
