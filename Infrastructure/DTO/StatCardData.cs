using System.Windows.Media;

namespace Infrastructure.DTO
{
    public class StatCardData
    {
        public string Label { get; set; }
        public int Count { get; set; }
        public Geometry IconGeometry { get; set; }
        public Brush ForegroundColor { get; set; }
        public Brush BackgroundColor { get; set; }
        public Color HoverColor { get; set; } // Для анимации рамки
    }
}
