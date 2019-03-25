namespace Syntha.Iota.Ui
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media.Animation;

  using Synthea.Iota.Core.Services;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
      this.InitializeSynthea();
    }

    private void InitializeSynthea()
    {
      ((Storyboard)this.FindResource("WaitStoryboard")).Begin();

      SyntheaInstaller.VersionCheck += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(new Action(() => { ((TextBlock)this.FindName("UpdateDescription")).Text = "Checking Synthea Version"; }));
        };
      SyntheaInstaller.InstallLatest += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(new Action(() => { ((TextBlock)this.FindName("UpdateDescription")).Text = "Installing Latest Synthea"; }));
        };
      SyntheaInstaller.SetupComplete += (sender, args) =>
        {
          this.Dispatcher.BeginInvoke(
            new Action(
              () =>
                {
                  ((Storyboard)this.FindResource("WaitStoryboard")).Stop();
                  ((Grid)this.FindName("LoadingSpinner")).Visibility = Visibility.Hidden;
                }));
        };

      Task.Run(() => SyntheaInstaller.InstallOrUpdate());
    }
  }
}