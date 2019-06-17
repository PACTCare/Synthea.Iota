namespace Synthea.Iota.Core.Services
{
  using RestSharp;

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

    public static IIotaRepository IotaRepository =>
      new RestIotaRepository(
        new RestIotaClient(new RestClient("https://nodes.thetangle.org:443")),
        new PoWSrvService());
  }
}