namespace Synthea.Iota.Core.Services
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Repository;

  public static class SyntheaRunner
  {
    public static event EventHandler FinishedSynthea;

    public static event EventHandler ParsingSyntheaData;

    public static event EventHandler StartingSynthea;

    public static event EventHandler StoringSyntheaData;

    public static List<ParsedPatient> CreatePatients(int count, string currentVersion)
    {
      try
      {
        StartingSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);

        var process = StartSynthea($"-p {count}", currentVersion);
        process.WaitForExit();
        process.Close();

        var parsedPatients = ParseSyntheaData(currentVersion);
        StoreParsedPatients(parsedPatients);

        return parsedPatients;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    public static List<ParsedPatient> ParsePatientFromFiles(string[] fileNames)
    {
      ParsingSyntheaData?.Invoke("SyntheaRunner", EventArgs.Empty);

      var parsedPatients = new List<ParsedPatient>();
      var resourceParser = new FhirJsonParser(
        new ParserSettings { AcceptUnknownMembers = true, AllowUnrecognizedEnums = true, PermissiveParsing = true });

      foreach (var file in fileNames)
      {
        var json = File.ReadAllText(file);
        var parsedBundle = resourceParser.Parse<Bundle>(json);
        if (parsedBundle.Entry[0].Resource is Patient)
        {
          parsedPatients.Add(ParsedPatient.FromBundle(parsedBundle));
        }
      }

      return parsedPatients;
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
                            WindowStyle = ProcessWindowStyle.Hidden,
                          }
                      };

      process.Start();
      return process;
    }

    public static void StoreParsedPatients(List<ParsedPatient> parsedPatients)
    {
      StoringSyntheaData?.Invoke("SyntheaRunner", EventArgs.Empty);
      new SqlLitePatientRepository().StorePatients(parsedPatients);

      FinishedSynthea?.Invoke("SyntheaRunner", EventArgs.Empty);
    }

    private static string GetSyntheaDirectory(string currentVersion)
    {
      var executionDir = Directory.GetCurrentDirectory();
      return $"{executionDir}\\Synthea\\synthea-{currentVersion}";
    }

    private static List<ParsedPatient> ParseSyntheaData(string currentVersion)
    {
      var outputDirectory = $"{GetSyntheaDirectory(currentVersion)}\\output\\fhir";
      var parsedPatients = ParsePatientFromFiles(Directory.GetFiles(outputDirectory));
      parsedPatients.ForEach(p => p.Resources.ForEach(r =>
        {
          r.Id = r.Resource.Id;
          r.PatientId = p.Resources[0].Resource.Id;
        }));

      Directory.Delete(outputDirectory, true);
      return parsedPatients;
    }
  }
}