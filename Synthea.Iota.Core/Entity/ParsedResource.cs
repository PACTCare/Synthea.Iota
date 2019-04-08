namespace Synthea.Iota.Core.Entity
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Newtonsoft.Json;

  using Tangle.Net.Utils;

  public class ParsedResource
  {
    public string FormattedJson => JsonConvert.SerializeObject(JsonConvert.DeserializeObject(this.Json), Formatting.Indented);

    public string Id { get; set; }

    public string Json => new FhirJsonSerializer().SerializeToString(this.Resource);

    public string PatientId { get; set; }

    public Resource Resource { get; set; }

    public string TypeName => this.Resource.ResourceType.ToString();

    public bool IsIotaResource => this.Resource.Id != null && InputValidator.IsTrytes(this.Resource.Id);
  }
}