using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace MultiDpiProcessor
{
    public class MultiDpiProcessor
    {
        private List<ScreenProcessingModel> _screens = new();
        private int _totalAdjustedWidth = 0;
        private int _totalAdjustedHeight = 0;
        private int _totalWidth = 0;
        private int _totalHeight = 0;

        public void ProcessImage(List<ScreenModel> screens, MemoryStream imageStream, bool keepAspectRatio, out string extension)
        {
            if (screens == null || screens.Count == 0)
            {
                throw new Exception("Screens not configured");
            }

            _screens.Clear();
            screens.ForEach(s => _screens.Add(new ScreenProcessingModel(s)));

            SetScreenRatios();
            ConvertToZeroBasedScreenCoordinates();
            CalculateAdjustedScreenCoordinates();
            CalculateTotalDimensions();

            GenerateOutput(imageStream, keepAspectRatio, out extension);
        }

        private void GenerateOutput(MemoryStream imageStream, bool keepAspectRatio, out string extension)
        {
            extension = ""; // TO DO
            keepAspectRatio = true; // TO DO

            var imageSource = Image.Load(imageStream);
            var scaleX = _totalAdjustedWidth;
            var scaleY = _totalAdjustedHeight;

            if (keepAspectRatio)
            {
                var sizeChange = _totalAdjustedWidth / (float)imageSource.Width;
                var resizeByWidth = sizeChange * imageSource.Height >= _totalAdjustedHeight;

                scaleX = resizeByWidth ? _totalAdjustedWidth : 0;
                scaleY = resizeByWidth ? 0 : _totalAdjustedHeight;
            }

            imageSource.Mutate(x => x.Resize(scaleX, scaleY)
                .Crop(new Rectangle
                   (
                       (imageSource.Width - _totalAdjustedWidth) / 2,
                       (imageSource.Height - _totalAdjustedHeight) / 2,
                       _totalAdjustedWidth,
                       _totalAdjustedHeight
                   ))
            );

            imageStream.Position = 0;

            var imageOut = Image.Load(imageStream);
            imageOut.Mutate(x => x.Resize(_totalWidth, _totalHeight).Fill(Color.Black));

            foreach (var screen in _screens)
            {
                var piece =
                    imageSource.Clone(i => i
                                 .Crop(
                                        new Rectangle
                                        (
                                            screen.AdjustedLocationX,
                                            screen.AdjustedLocationY,
                                            screen.AdjustedPixelWidth,
                                            screen.AdjustedPixelHeight
                                        )
                                      )
                );

                if (screen.Ratio != 1)
                {
                    piece.Mutate(p => p.Resize(screen.PixelWidth, screen.PixelHeight));
                }
                imageOut.Mutate(p => p.DrawImage(piece, screen.LocationPoint, 1f));
            }

            imageStream.Position = 0;
            imageOut.SaveAsJpeg(imageStream);
            imageStream.Position = 0;
        }

        private void SetScreenRatios()
        {
            decimal maxDensity = _screens.OrderByDescending(s => s.Ppi).First().Ppi;
            _screens.ForEach(s => { s.Ratio = maxDensity / s.Ppi; });
        }

        private void ConvertToZeroBasedScreenCoordinates()
        {
            int leftEdge = (int)_screens.Min(s => s.LocationX);
            int topEdge = (int)_screens.Min(s => s.LocationY);

            foreach (var screen in _screens)
            {
                screen.LocationX -= leftEdge;
                screen.LocationY -= topEdge;
                screen.AdjustedLocationX = screen.LocationX;
                screen.AdjustedLocationY = screen.LocationY;
            }
        }

        private void CalculateTotalDimensions()
        {
            _totalAdjustedWidth = (int)_screens.Max(s => s.AdjustedLocationX + s.AdjustedPixelWidth);
            _totalAdjustedHeight = (int)_screens.Max(s => s.AdjustedLocationY + s.AdjustedPixelHeight);
            _totalWidth = (int)_screens.Max(s => s.LocationX + s.PixelWidth);
            _totalHeight = (int)_screens.Max(s => s.LocationY + s.PixelHeight);
        }

        private void CalculateAdjustedScreenCoordinates()
        {
            foreach (var screen in _screens)
            {
                HorizontalShiftScreens(screen);
                VerticalShiftScreens(screen);
            }
        }

        private void HorizontalShiftScreens(ScreenProcessingModel referenceScreen)
        {
            var screensOnTheRight = _screens
                .Where
                (
                    s => s.LocationX > referenceScreen.LocationX
                );

            var shiftLength = referenceScreen.AdjustedPixelWidth - referenceScreen.PixelWidth;

            foreach (var screen in screensOnTheRight)
            {
                screen.AdjustedLocationX += shiftLength;
            }
        }

        private void VerticalShiftScreens(ScreenProcessingModel referenceScreen)
        {
            var screensBelow = _screens
                .Where
                (
                    s => s.LocationY > referenceScreen.LocationY
                );

            var shiftHeight = referenceScreen.AdjustedPixelHeight - referenceScreen.PixelHeight;

            foreach (var screen in screensBelow)
            {
                screen.AdjustedLocationY += shiftHeight;
            }
        }
    }
}
