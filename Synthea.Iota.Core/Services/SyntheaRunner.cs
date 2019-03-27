namespace Synthea.Iota.Core.Services
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  public static class SyntheaRunner
  {
    public static event EventHandler FinishedSynthea;

    public static event EventHandler ParsingSyntheaData;

    public static event EventHandler StartingSynthea;

    public static List<Resource> CreatePatients(int count, string currentVersion)
    {
      try
      {
        StartingSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);

        var process = StartSynthea($"-p {count}", currentVersion);
        process.WaitForExit();
        process.Close();

        return ParseSyntheaData(currentVersion);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    public static Process StartSynthea(string arguments, string currentVersion)
    {
      var directory = GetSyntheaDirectory(currentVersion);
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

    private static string GetSyntheaDirectory(string currentVersion)
    {
      var executionDir = Directory.GetCurrentDirectory();
      return $"{executionDir}\\Synthea\\synthea-{currentVersion}";
    }

    private static List<Resource> ParseSyntheaData(string currentVersion)
    {
      ParsingSyntheaData?.Invoke("SyntheaRunner", EventArgs.Empty);

      var createdResources = new List<Resource>();
      var resourceParser = new FhirJsonParser();
      var outputDirectory = $"{GetSyntheaDirectory(currentVersion)}\\output\\fhir";

      foreach (var file in Directory.GetFiles(outputDirectory))
      {
        createdResources.Add(resourceParser.Parse<Resource>(File.ReadAllText(file)));
      }

      Directory.Delete(outputDirectory, true);
      FinishedSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);

      return createdResources;
    }
  }
}