namespace Synthea.Iota.Core.Services
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Repository;

  public static class SyntheaRunner
  {
    public static event EventHandler FinishedSynthea;

    public static event EventHandler ParsingSyntheaData;

    public static event EventHandler StoringSyntheaData;

    public static event EventHandler StartingSynthea;

    public static List<ParsedPatient> CreatePatients(int count, string currentVersion)
    {
      try
      {
        StartingSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);

        var process = StartSynthea($"-p {count}", currentVersion);
        process.WaitForExit();
        process.Close();

        var parsedPatients = ParseSyntheaData(currentVersion);

        StoringSyntheaData?.Invoke("SyntheaRunner", EventArgs.Empty);
        new SqlLitePatientRepository().StorePatients(parsedPatients);

        FinishedSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);
        return parsedPatients;
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

    private static List<ParsedPatient> ParseSyntheaData(string currentVersion)
    {
      ParsingSyntheaData?.Invoke("SyntheaRunner", EventArgs.Empty);

      var parsedPatients = new List<ParsedPatient>();
      var resourceParser = new FhirJsonParser();
      var outputDirectory = $"{GetSyntheaDirectory(currentVersion)}\\output\\fhir";

      foreach (var file in Directory.GetFiles(outputDirectory))
      {
        var parsedBundle = resourceParser.Parse<Bundle>(File.ReadAllText(file));
        if (parsedBundle.Entry[0].Resource is Patient)
        {
          parsedPatients.Add(new ParsedPatient { Resources = parsedBundle.Entry.Select(e => e.Resource).ToList() });
        }
      }

      Directory.Delete(outputDirectory, true);
      return parsedPatients;
    }
  }
}