namespace Synthea.Iota.Ui.Services
{
  using System.Windows;
  using System.Windows.Controls;

  public static class ApplicationManager
  {
    public static string CurrentSyntheaVersion { get; set; }

    public static Window MainWindow { get; set; }

    public static void SetContent(Page page)
    {
      MainWindow.Content = page;
    }
  }
}