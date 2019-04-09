namespace Synthea.Iota.Core.Services
{
  using Hl7.Fhir.Model;
  using Hl7.Fhir.Serialization;

  using RestSharp;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Repository;

  public static class FhirInteractor
  {
    public static ParsedResource CreateResource(ParsedResource resource)
    {
      var patientRepository = new SqlLitePatientRepository();
      var referencedResource = patientRepository.GetResource(resource.PatientId);

      if (resource.Resource.GetType().GetProperty("Subject") != null)
      {
        resource.Resource.GetType().GetProperty("Subject")?.SetValue(
          resource.Resource,
          new ResourceReference { Reference = $"urn:iota:{referencedResource.Resource.Id}" });
      }

      if (resource.Resource.GetType().GetProperty("Patient") != null)
      {
        resource.Resource.GetType().GetProperty("Patient")?.SetValue(
          resource.Resource,
          new ResourceReference { Reference = $"urn:iota:{referencedResource.Resource.Id}" });
      }

      var client = new RestClient("http://localhost:64264");
      var request = new RestRequest($"/api/fhir/create/{resource.TypeName}", Method.POST);
      request.AddHeader("Content-Type", "application/fhir+json");
      request.AddHeader("Prefer", "representation");
      request.AddParameter("application/fhir+json", resource.Json, ParameterType.RequestBody);

      var response = client.Execute(request);

      // TODO: Verify response
      var parsedResource = new ParsedResource
                             {
                               Id = resource.Id, PatientId = resource.PatientId, Resource = new FhirJsonParser().Parse<Resource>(response.Content)
                             };
      patientRepository.UpdateResource(parsedResource);

      return parsedResource;
    }
  }
}