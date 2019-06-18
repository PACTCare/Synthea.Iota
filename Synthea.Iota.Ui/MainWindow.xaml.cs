namespace Syntha.Iota.Ui
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using System.Windows;

  using Microsoft.Win32;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Exception;
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
      this.MainMenu.IsEnabled = false;

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
                  this.MainMenu.IsEnabled = true;

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

      Task.Run(() =>
        {
          try
          {
            ApplicationManager.CurrentSyntheaVersion = SyntheaInstaller.InstallOrUpdate();
          }
          catch (JavaNotInstalledException)
          {
            this.Dispatcher.BeginInvoke(
              new Action(
                () =>
                  {
                    MessageBox.Show(
                      "Synthea needs the Java 1.8 JDK or higher to be installed. Please go to https://www.oracle.com/technetwork/java/javase/downloads/index.html and install the latest JDK version.",
                      "Java not installed or incorrect version!");

                    Application.Current.Shutdown();
                  }));
          }
          catch (JavaHomeNotSetException)
          {
            this.Dispatcher.BeginInvoke(
              new Action(
                () =>
                  {
                    MessageBox.Show(
                      "Please set the java home variable to your JDK installation (see https://www.google.com/search?q=how+to+set+java_home&oq=how+to+set+java)",
                      "Java path (JAVA_HOME) not set!");

                    Application.Current.Shutdown();
                  }));
          }
          catch (Exception exception)
          {
            this.Dispatcher.BeginInvoke(
              new Action(
                () =>
                  {
                    MessageBox.Show(exception.StackTrace, exception.Message);
                    Application.Current.Shutdown();
                  }));
          }
        });
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

    private void PatientImport_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog { Multiselect = true };
      if (dialog.ShowDialog() == true)
      {
        var spinner = new LoadingSpinner();
        ApplicationManager.SetContent(spinner);

        spinner.Start();
        this.MainMenu.IsEnabled = false;

        SyntheaRunner.ParsingSyntheaData += (o, args) =>
          {
            this.Dispatcher.BeginInvoke(new Action(() => { spinner.SetText("Parsing Patient records."); }));
          };

        SyntheaRunner.StoringSyntheaData += (o, args) =>
          {
            this.Dispatcher.BeginInvoke(new Action(() => { spinner.SetText("Storing Patient records."); }));
          };

        SyntheaRunner.FinishedSynthea += (o, args) =>
          {
            this.Dispatcher.BeginInvoke(
              new Action(
                () =>
                  {
                    this.MainMenu.IsEnabled = true;
                    ApplicationManager.SetContent(new PatientCreation());
                  }));
          };

        var parsedPatients = SyntheaRunner.ParsePatientFromFiles(dialog.FileNames);
        SyntheaRunner.StoreParsedPatients(parsedPatients);
      }
    }
  }
}