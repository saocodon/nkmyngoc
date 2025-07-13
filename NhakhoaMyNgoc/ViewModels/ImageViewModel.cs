using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Models;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ImageViewModel : ObservableObject
    {
        private readonly DataContext _db;

        public ImageViewModel()
        {
            _db = new DataContext();
        }

        public string RelativePath { get; set; } = "";
        // TODO
        public string FullPath => "";
    }
}
