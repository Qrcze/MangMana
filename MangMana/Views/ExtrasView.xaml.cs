using MangMana.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MangMana.Views
{
    /// <summary>
    /// Interaction logic for ExtrasView.xaml
    /// </summary>
    public partial class ExtrasView : UserControl
    {
        private ExtrasViewModel _vm;

        public ExtrasView()
        {
            InitializeComponent();
            _vm = DataContext as ExtrasViewModel;
        }

        private readonly List<string> _imageExtensions = new List<string> { ".jpg", ".png", ".bmp" };

        private void ImageDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] images = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var imgPath in images)
                {
                    if (_imageExtensions.Contains(Path.GetExtension(imgPath)))
                        _vm.AddImage(imgPath);
                }
            }
        }
    }
}