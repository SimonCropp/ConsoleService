using System.IO;

static class SampleLocation
{
    static SampleLocation()
    {
        var assembly = typeof(SampleLocation).Assembly;

        var path = assembly.CodeBase.Replace("file:///", "")
            .Replace("file://", "")
            .Replace(@"file:\\\", "")
            .Replace(@"file:\\", "");

        var directory = Path.GetDirectoryName(path);

        SampleAssembly = Path.Combine(directory.Replace("Tests", "Sample"), "Sample.dll");
    }

    public static string SampleAssembly;
}