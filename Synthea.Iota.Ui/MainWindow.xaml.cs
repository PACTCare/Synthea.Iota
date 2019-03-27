namespace Syntha.Iota.Ui
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media.Animation;

  using Synthea.Iota.Core.Services;
  using Synthea.Iota.Ui;
  using Synthea.Iota.Ui.Services;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
      this.InitializeSynthea();

      ApplicationManager.MainWindow = this;
    }

    private void InitializeSynthea()
    {
      var spinner = new LoadingSpinner();
      this.Content = spinner;

      spinner.Start();

      SyntheaInstaller.VersionCheck += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(new Action(() => { spinner.SetText("Checking Synthea Version"); }));
        };
      SyntheaInstaller.DownloadLatest += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(new Action(() => { spinner.SetText("Downloading Latest Synthea"); }));
        };
      SyntheaInstaller.InstallLatest += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(new Action(() => { spinner.SetText("Installing Latest Synthea"); }));
        };
      SyntheaInstaller.SetupComplete += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(
            new Action(
              () =>
                {
                  this.Content = new MainMenu();
                }));
        };

      Task.Run(() => ApplicationManager.CurrentSyntheaVersion = SyntheaInstaller.InstallOrUpdate());
    }
  }
}