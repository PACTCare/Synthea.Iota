namespace Synthea.Iota.Core.Services
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using RestSharp;

  using Synthea.Iota.Core.Entity;

  public static class FhirInteractor
  {
    public static Resource CreateResource(ParsedResource resource)
    {
      // TODO: Get patient for resource


      var client = new RestClient("http://localhost:64264");
      var request = new RestRequest($"/api/fhir/create/{resource.TypeName}", Method.POST);
      request.AddHeader("Content-Type", "application/fhir+json");
      request.AddHeader("Prefer", "representation");
      request.AddParameter("application/fhir+json", resource.Json, ParameterType.RequestBody);

      var response = client.Execute(request);

      return new FhirJsonParser().Parse<Resource>(response.Content);
    }
  }
}