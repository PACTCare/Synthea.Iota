namespace Synthea.Iota.Ui.Services
{
  using System.Windows;
  using System.Windows.Controls;

  using Synthea.Iota.Core.Repository;

  public static class ApplicationManager
  {
    public static string CurrentSyntheaVersion { get; set; }

    public static Window MainWindow { get; set; }

    public static IPatientRepository PatientRepository => new SqlLitePatientRepository();

    public static void SetContent(Page page)
    {
      MainWindow.Content = page;
    }
  }
}