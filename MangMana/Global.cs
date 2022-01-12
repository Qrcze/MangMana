using System.Collections.Generic;

namespace MangMana
{
    internal static class Global
    {
        public const string DataCatalog = "data";
        public const string ThumbnailsCatalog = "data/thumbnails";
        public const string MetadataPath = "data/metadata.json";

        public static List<string> ImageExtensions { get; } = new List<string> { ".jpg", ".png", ".bmp" };
    }
}