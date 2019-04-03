namespace Synthea.Iota.Core.Entity
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  using Tangle.Net.Entity;

  using Bundle = Hl7.Fhir.Model.Bundle;

  public class ParsedPatient
  {
    public string Name
    {
      get
      {
        var name = ((Patient)this.Resources[0].Resource).Name[0];
        return $"{name.GivenElement[0].Value} {name.Family}";
      }
    }

    public int ResourceCount => this.Resources.Count;

    public List<ParsedResource> Resources { get; set; }

    public Seed Seed { get; set; }

    public static ParsedPatient FromBundle(Bundle bundle)
    {
      return new ParsedPatient { Resources = bundle.Entry.Select(e => new ParsedResource { Resource = e.Resource }).ToList(), Seed = Seed.Random() };
    }
  }
}