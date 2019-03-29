namespace Synthea.Iota.Core.Entity
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;

  public class ParsedResource
  {
    public Resource Resource { get; set; }

    public string TypeName => this.Resource.ResourceType.ToString();

    public IEnumerable<Base> Children => this.Resource.Children;
  }
}