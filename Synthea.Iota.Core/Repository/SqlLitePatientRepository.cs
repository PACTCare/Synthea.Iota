namespace Synthea.Iota.Core.Repository
{
  using System.Collections.Generic;
  using System.Data.SQLite;
  using System.IO;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Synthea.Iota.Core.Entity;

  using Tangle.Net.Entity;

  public class SqlLitePatientRepository : IPatientRepository
  {
    private const string DatabaseFilename = "patientstore.sqlite";

    public SqlLitePatientRepository()
    {
      Init();
    }

    /// <inheritdoc />
    public List<ParsedPatient> LoadPatients()
    {
      var parser = new FhirJsonParser();
      var parsedPatients = new List<ParsedPatient>();

      using (var connection = new SQLiteConnection($"Data Source={DatabaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand("SELECT * FROM Patient", connection))
        {
          var patientRecords = command.ExecuteReader();
          while (patientRecords.Read())
          {
            var parsedPatient = new ParsedPatient { Seed = new Seed(patientRecords["Seed"] as string), Resources = new List<ParsedResource>() };

            var patientId = patientRecords["Id"] as string;
            using (var innerCommand = new SQLiteCommand($"SELECT * FROM Resource WHERE PatientId='{patientId}'", connection))
            {
              var resources = innerCommand.ExecuteReader();
              while (resources.Read())
              {
                parsedPatient.Resources.Add(new ParsedResource { Resource = parser.Parse<Resource>(resources["Payload"] as string) });
              }
            }

            parsedPatients.Add(parsedPatient);
          }
        }
      }

      return parsedPatients;
    }

    /// <inheritdoc />
    public void StorePatients(List<ParsedPatient> patients)
    {
      var serializer = new FhirJsonSerializer();
      using (var connection = new SQLiteConnection($"Data Source={DatabaseFilename};Version=3;"))
      {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
          foreach (var parsedPatient in patients)
          {
            using (var command = new SQLiteCommand($"INSERT OR IGNORE INTO Patient (Id, Seed) VALUES ('{parsedPatient.Resources[0].Resource.Id}', '{parsedPatient.Seed.Value}')", connection, transaction))
            {
              command.ExecuteNonQuery();
            }

            foreach (var resource in parsedPatient.Resources)
            {
              using (var command = new SQLiteCommand(
                $"INSERT OR IGNORE INTO Resource (Id, PatientId, Payload) VALUES ('{resource.Resource.Id}', '{parsedPatient.Resources[0].Resource.Id}', @payload)",
                connection))
              {
                command.Parameters.AddWithValue("payload", serializer.SerializeToString(resource.Resource));
                command.ExecuteNonQuery();
              }
            }
          }

          transaction.Commit();
        }
      }
    }

    private static void Init()
    {
      if (File.Exists(DatabaseFilename))
      {
        return;
      }

      SQLiteConnection.CreateFile(DatabaseFilename);
      using (var connection = new SQLiteConnection($"Data Source={DatabaseFilename};Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand("CREATE TABLE Patient (Id TEXT NOT NULL PRIMARY KEY, Seed TEXT NULL)", connection))
        {
          command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(
          "CREATE TABLE Resource (Id TEXT NOT NULL PRIMARY KEY, PatientId TEXT NOT NULL, Payload TEXT NOT NULL, FOREIGN KEY (PatientId) REFERENCES Patient(Id))",
          connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}