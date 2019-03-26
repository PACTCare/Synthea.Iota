namespace Synthea.Iota.Ui.Services
{
  using System.Windows;
  using System.Windows.Controls;

  public static class Navigator
  {
    public static Window MainWindow { get; set; }

    public static void SetContent(Page page)
    {
      MainWindow.Content = page;
    }
  }
}