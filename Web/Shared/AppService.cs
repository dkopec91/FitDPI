using Microsoft.AspNetCore.Components.Forms;
using MultiDpiProcessor;

namespace FitDPI.Shared
{
    public class AppService
    {
        public List<ScreenModel>? Screens { get; set; }

        public List<IBrowserFile> ImageFiles { get; set; }
    }
}
