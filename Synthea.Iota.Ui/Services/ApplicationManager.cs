namespace Synthea.Iota.Ui.Services
{
  using System.Windows;
  using System.Windows.Controls;

  using Syntha.Iota.Ui;

  using Synthea.Iota.Core.Repository;

  public static class ApplicationManager
  {
    public static string CurrentSyntheaVersion { get; set; }

    public static MainWindow MainWindow { get; set; }

    public static IPatientRepository PatientRepository => new SqlLitePatientRepository();

    public static void SetContent(UserControl page)
    {
      MainWindow.ContentControl.Content = page;
    }
  }
}