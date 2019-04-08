namespace Synthea.Iota.Core.Services
{
  using System.Collections.Generic;

  using Hl7.Fhir.Serialization;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.SqlLite.Encryption;
  using Pact.Fhir.Iota.SqlLite.Services;

  using RestSharp;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Cryptography.Curl;
  using Tangle.Net.Cryptography.Signing;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public static class DependencyResolver
  {
    public static MamChannelFactory ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, IotaRepository);

    public static MamChannelSubscriptionFactory SubscriptionFactory = new MamChannelSubscriptionFactory(
      IotaRepository,
      CurlMamParser.Default,
      CurlMask.Default);

    public static IEncryption Encryption = new RijndaelEncryption("somenicekey", "somenicesalt");

    public static CreateResourceInteractor CreateResourceInteractor => new CreateResourceInteractor(FhirRepository, new FhirJsonParser());

    public static IFhirRepository FhirRepository =>
      new IotaFhirRepository(
        IotaRepository,
        new FhirJsonTryteSerializer(),
        new SqlLiteResourceTracker(ChannelFactory, SubscriptionFactory, Encryption),
        new SqlLiteDeterministicCredentialProvider(
          new SqlLiteResourceTracker(ChannelFactory, SubscriptionFactory, Encryption),
          new IssSigningHelper(new Curl(), new Curl(), new Curl()),
          new AddressGenerator(),
          IotaRepository),
        new SqlLiteReferenceResolver(Encryption));

    public static IIotaRepository IotaRepository =>
      new RestIotaRepository(
        new RestIotaClient(new RestClient("https://nodes.thetangle.org:443")),
        new PoWSrvService());
  }
}