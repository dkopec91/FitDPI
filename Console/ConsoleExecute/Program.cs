using MultiDpiProcessor;
using System.Diagnostics;

var screens = new List<ScreenModel>()
{
    new ScreenModel()
    {
        PixelWidth = 2560,
        PixelHeight = 1600,
        Ppi = 101
    },
    new ScreenModel()
    {
        PixelWidth = 1920,
        PixelHeight = 1200,
        Ppi = 94,
        LocationX = -1920,
        LocationY = 0,
    },
    new ScreenModel()
    {
        PixelWidth = 1920,
        PixelHeight = 1200,
        Ppi = 94,
        LocationX = 2560,
        LocationY = 0
    }
};

var testFile = File.OpenRead("testFile.jpg");
var inputFileStream = new FileStream(testFile.SafeFileHandle, FileAccess.Read);

var memoryStream = new MemoryStream();
inputFileStream.CopyTo(memoryStream);
memoryStream.Position = 0;

new MultiDpiProcessor.MultiDpiProcessor().ProcessImage(screens, memoryStream, false, out string ext);

var outputFile = File.OpenWrite("output.jpg");
var outputFileStream = new FileStream(outputFile.SafeFileHandle, FileAccess.Write);

memoryStream.CopyTo(outputFileStream);
outputFileStream.Close();

Process runProg = new Process();
runProg.StartInfo.FileName = $"C:\\Program Files\\IrfanView\\i_view64.exe";
runProg.StartInfo.Arguments = $"-o {Path.GetFullPath("output.jpg")}";
runProg.Start();
