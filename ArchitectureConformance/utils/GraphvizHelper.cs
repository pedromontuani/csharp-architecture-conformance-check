using System.Diagnostics;

namespace ArchitetureConformance.utils;

public static class GraphvizHelper
{
    public static void RenderGraph(string dotCode, string outputPath)
    {
        string tempDotFile = Path.GetTempFileName() + ".dot";
        File.WriteAllText(tempDotFile, dotCode);

        ProcessStartInfo psi = new ProcessStartInfo("dot", $"-Tpng -Gsize=50,50 -Gdpi=300 -o{outputPath} {tempDotFile}");
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        using (Process process = new Process())
        {
            process.StartInfo = psi;
            process.Start();
            process.WaitForExit();
        }

        File.Delete(tempDotFile);
    }
}
