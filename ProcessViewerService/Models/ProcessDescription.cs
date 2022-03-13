using System.Drawing;

namespace SimpleProcessViewer.Models
{
    public class ProcessDescription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Responding { get; set; }
        public string Memory { get; set; }
        
    }
}
