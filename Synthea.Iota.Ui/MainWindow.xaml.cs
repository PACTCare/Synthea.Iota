namespace Syntha.Iota.Ui
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;

  using Synthea.Iota.Core.Entity;
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
      ApplicationManager.MainWindow = this;

      this.InitializeComponent();
      this.InitializeSynthea();
    }

    private void InitializeSynthea()
    {
      var spinner = new LoadingSpinner();
      ApplicationManager.SetContent(spinner);

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
                  var patients = ApplicationManager.PatientRepository.LoadPatients();
                  if (patients.Count > 0)
                  {
                    ApplicationManager.SetContent(new PatientList(patients));
                  }
                  else
                  {
                    ApplicationManager.SetContent(new PatientCreation());
                  }
                }));
        };

      Task.Run(() => ApplicationManager.CurrentSyntheaVersion = SyntheaInstaller.InstallOrUpdate());
    }

    private void GeneratePatients_OnClick(object sender, RoutedEventArgs e)
    {
      ApplicationManager.SetContent(new PatientCreation());
    }

    private void PatientOverview_OnClick(object sender, RoutedEventArgs e)
    {
      ApplicationManager.SetContent(new PatientList(ApplicationManager.PatientRepository.LoadPatients()));
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}