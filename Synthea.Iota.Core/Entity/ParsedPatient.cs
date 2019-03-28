namespace Synthea.Iota.Core.Entity
{
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  public class ParsedPatient
  {
    public string Name
    {
      get
      {
        var name = ((Patient)this.Resources[0]).Name[0];
        return $"{name.GivenElement[0].Value} {name.Family}";
      }
    }

    public int ResourceCount => this.Resources.Count;

    public List<Resource> Resources { get; set; }

    public string Seed { get; set; }
  }
}