using MangMana.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MangMana.Views
{
    /// <summary>
    /// Interaction logic for BookInfoView.xaml
    /// </summary>
    public partial class BookInfoView : UserControl
    {
        private BookInfoViewModel _vm;

        public BookInfoView()
        {
            InitializeComponent();
            _vm = DataContext as BookInfoViewModel;
        }

        private void ImageDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < paths.Length; i++)
                {
                    if (Global.ImageExtensions.Contains(Path.GetExtension(paths[i])))
                    {
                        _vm.ChangeThumbnail(paths[i]);
                        break;
                    }
                }
            }
        }
    }
}