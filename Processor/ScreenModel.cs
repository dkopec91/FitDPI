namespace MultiDpiProcessor
{
    public class ScreenModel
    {
        public ScreenModel() { }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Ppi { get; set; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public FrameOffset FrameOffset { get; set; } = new FrameOffset();
    }

    public class FrameOffset
    {
        public int Left, Top, Right, Bottom;
    }
}
