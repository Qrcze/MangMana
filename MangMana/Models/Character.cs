using MangMana.Helpers;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace MangMana.Models
{
    public class Character : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; } = "------------";
        public string Description { get; set; }

        public ObservableCollection<string> ImageUIDs { get; set; } = new();

        [JsonIgnore]
        [DoNotNotify]
        public int CurrentImage { get; set; }

        [JsonIgnore]
        [DoNotNotify]
        public string BookUID { get; set; }

        public string GetDirectoryPath() => $"{Global.DataCatalog}/thumbnails/{BookUID}";

        public string GetImagePath(int index) => $"{GetDirectoryPath()}/{ImageUIDs[index]}.thumb";

        public string GetImagePath(string uid) => $"{GetDirectoryPath()}/{uid}.thumb";

        public void AddImage(string fileName)
        {
            var img = ImageHelpers.CreateBitmapImage(fileName, 0, 400);

            AddImage(img);
        }

        public void AddImage(BitmapSource img)
        {
            string uid = SimpleUID.Generate(uid => !File.Exists(GetImagePath(uid)));

            ImageHelpers.SaveImage(img, GetImagePath(uid));

            ImageUIDs.Add(uid);
        }

        public void RemoveImage(int imageNumber)
        {
            File.Delete(ImageUIDs[imageNumber]);
            ImageUIDs.Remove(ImageUIDs[imageNumber]);
        }

        public BitmapImage GetCurrentImage()
        {
            if (ImageUIDs.Count == 0)
                return null;
            return ImageHelpers.CreateBitmapImage(GetImagePath(CurrentImage), 0, 400);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}