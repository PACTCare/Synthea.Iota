namespace Synthea.Iota.Core.Extensions
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using Newtonsoft.Json;

  public static class ResourceExtensions
  {
    public static string ToFormattedJson(this Resource resource)
    {
      return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(resource.ToJson());
    }
  }
}