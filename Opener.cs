using System.Diagnostics;

namespace FurnitureSalon
{
    internal static class Opener
    {
        internal static void OpenCreatedFile(string filePath)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
    }
}
