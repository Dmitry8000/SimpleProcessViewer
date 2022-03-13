namespace SimpleProcessViewer.Models
{
    //its only for wpf example
    public class FullProcessInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Responding { get; set; }
        public string Memory { get; set; }
        public string Username { get; set; }
        public string ExecutablePath { get; set; }
    }
}
