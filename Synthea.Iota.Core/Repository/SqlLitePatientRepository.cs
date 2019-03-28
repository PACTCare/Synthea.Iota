namespace Synthea.Iota.Core.Repository
{
  using System.Collections.Generic;

  using Synthea.Iota.Core.Entity;

  public class SqlLitePatientRepository : IPatientRepository
  {
    /// <inheritdoc />
    public List<ParsedPatient> LoadPatients()
    {
      return null;
    }

    /// <inheritdoc />
    public void StorePatients(List<ParsedPatient> patients)
    {
    }
  }
}