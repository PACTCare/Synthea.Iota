namespace Synthea.Iota.Core.Services
{
  using System;
  using System.IO;
  using System.IO.Compression;

  using Newtonsoft.Json;

  using RestSharp;
  using RestSharp.Extensions;

  public static class SyntheaInstaller
  {
    public static event EventHandler DownloadLatest;

    public static event EventHandler InstallLatest;

    public static event EventHandler SetupComplete;

    public static event EventHandler VersionCheck;

    public static string InstallOrUpdate()
    {
      if (!Directory.Exists("Synthea"))
      {
        Directory.CreateDirectory("Synthea");
      }

      VersionCheck?.Invoke("SyntheaInstaller", EventArgs.Empty);
      var client = new RestClient("https://github.com/synthetichealth/synthea");

      var response = client.Execute(new RestRequest("/releases/latest", Method.GET));
      var tag = (string)((dynamic)JsonConvert.DeserializeObject(response.Content)).tag_name;
      var currentVersion = tag.Substring(1);

      if (Directory.Exists($"Synthea/synthea-{currentVersion}"))
      {
        SetupComplete?.Invoke("SyntheaInstaller", EventArgs.Empty);
        return currentVersion;
      }

      DownloadLatest?.Invoke("SyntheaInstaller", EventArgs.Empty);
      client.DownloadData(new RestRequest($"/archive/{tag}.zip")).SaveAs("synthea.zip");
      ZipFile.ExtractToDirectory("synthea.zip", "Synthea");
      File.Delete("synthea.zip");

      InstallLatest?.Invoke("SyntheaInstaller", EventArgs.Empty);
      var synthea = SyntheaRunner.StartSynthea(string.Empty, currentVersion);
      synthea.WaitForExit();
      synthea.Close();

      Directory.Delete($"Synthea/synthea-{currentVersion}/output", true);

      SetupComplete?.Invoke("SyntheaInstaller", EventArgs.Empty);
      return currentVersion;
    }
  }
}