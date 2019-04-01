namespace Synthea.Iota.Core.Entity
{
  using System.Collections.Generic;
  using System.Linq;

  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Newtonsoft.Json;

  public class ParsedResource
  {
    public Resource Resource { get; set; }

    public string TypeName => this.Resource.ResourceType.ToString();

    public IEnumerable<Base> Children => this.Resource.Children;

    public string Json => new FhirJsonSerializer().SerializeToString(this.Resource);

    public string FormattedJson => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(this.Json), Formatting.Indented);
  }
}