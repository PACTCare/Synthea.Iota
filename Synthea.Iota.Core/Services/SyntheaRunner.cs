namespace Synthea.Iota.Core.Services
{
  using System;
  using System.Diagnostics;
  using System.IO;

  public static class SyntheaRunner
  {
    public static void CreatePatients(int count)
    {
      try
      {
        var executionDir = Directory.GetCurrentDirectory();
        var directory = $"{executionDir}\\Synthea\\synthea-2.4.0";

        var process = new Process
                        {
                          StartInfo =
                            {
                              WorkingDirectory = directory,
                              FileName = $"{directory}\\run_synthea.bat",
                              CreateNoWindow = false,
                              Arguments = $"-p {count}",
                              WindowStyle = ProcessWindowStyle.Normal
                            }
                        };

        process.Start();
        process.WaitForExit();
        process.Close();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }
  }
}