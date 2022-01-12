namespace MangMana.Models
{
    internal class BookMetadata
    {
        public string Name { get; }
        public string UID { get; }

        public BookMetadata(string name, string uID)
        {
            Name = name;
            UID = uID;
        }
    }
}