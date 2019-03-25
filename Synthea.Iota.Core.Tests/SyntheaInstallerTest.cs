namespace Synthea.Iota.Core.Tests
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Synthea.Iota.Core.Services;

  [TestClass]
  public class SyntheaInstallerTest
  {
    [TestMethod]
    public void TestReleaseDownload()
    {
      var currentVersion = SyntheaInstaller.InstallOrUpdate();
      Assert.AreEqual("2.4.0", currentVersion);

      SyntheaRunner.CreatePatients(5);
    }
  }
}