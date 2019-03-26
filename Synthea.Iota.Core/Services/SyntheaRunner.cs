namespace Synthea.Iota.Core.Services
{
  using System;
  using System.Diagnostics;
  using System.IO;

  public static class SyntheaRunner
  {
    public static event EventHandler FinishedSynthea;

    public static event EventHandler StartingSynthea;

    public static void CreatePatients(int count)
    {
      try
      {
        StartingSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);
        var process = StartSynthea($"-p {count}");

        process.WaitForExit();
        process.Close();
        FinishedSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    public static Process StartSynthea(string arguments)
    {
      var executionDir = Directory.GetCurrentDirectory();
      var directory = $"{executionDir}\\Synthea\\synthea-2.4.0";

      var process = new Process
                      {
                        StartInfo =
                          {
                            WorkingDirectory = directory,
                            FileName = $"{directory}\\run_synthea.bat",
                            CreateNoWindow = true,
                            Arguments = arguments,
                            WindowStyle = ProcessWindowStyle.Hidden
                          }
                      };

      process.Start();
      return process;
    }
  }
}