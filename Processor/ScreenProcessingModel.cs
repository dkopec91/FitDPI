using SixLabors.ImageSharp;

namespace MultiDpiProcessor
{
    public class ScreenProcessingModel : ScreenModel
    {
        public ScreenProcessingModel(ScreenModel s)
        {
            Ppi = s.Ppi;
            PixelWidth = s.PixelWidth;
            PixelHeight = s.PixelHeight;
            LocationX = s.LocationX;
            LocationY = s.LocationY;
            FrameOffset = s.FrameOffset;
        }

        private decimal _ratio;
        public decimal Ratio
        {
            get { return _ratio; }
            set
            {
                _ratio = value;
                AdjustedPixelHeight = (int)((PixelHeight) * Ratio);
                AdjustedPixelWidth = (int)((PixelWidth) * Ratio);
            }
        }

        public int AdjustedPixelWidth { get; set; }
        public int AdjustedPixelHeight { get; set; }
        public int AdjustedLocationX { get; set; }
        public int AdjustedLocationY { get; set; }
        public Point LocationPoint => new(LocationX, LocationY);
    }
}
