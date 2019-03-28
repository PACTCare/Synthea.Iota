namespace Synthea.Iota.Core.Repository
{
  using System.Collections.Generic;

  using Synthea.Iota.Core.Entity;

  public interface IPatientRepository
  {
    List<ParsedPatient> LoadPatients();

    void StorePatients(List<ParsedPatient> patients);
  }
}