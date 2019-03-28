namespace Synthea.Iota.Ui
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;

  using Synthea.Iota.Core.Services;
  using Synthea.Iota.Ui.Services;

  /// <summary>
  /// Interaction logic for MainMenu.xaml
  /// </summary>
  public partial class MainMenu : Page
  {
    public MainMenu()
    {
      this.InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      var spinner = new LoadingSpinner();
      ApplicationManager.SetContent(spinner);

      spinner.Start();

      SyntheaRunner.StartingSynthea += (o, args) =>
        {
          this.Dispatcher.BeginInvoke(new Action(() => { spinner.SetText("Creating Patient records."); }));
        };

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
          this.Dispatcher.BeginInvoke(new Action(() => { ApplicationManager.SetContent(new MainMenu()); }));
        };

      var patientAmount = int.Parse(((TextBox)this.FindName("PatientAmount")).Text);
      Task.Run(
        () =>
          {
            var patients = SyntheaRunner.CreatePatients(patientAmount, ApplicationManager.CurrentSyntheaVersion);
            this.Dispatcher.BeginInvoke(new Action(() => { ApplicationManager.SetContent(new PatientList(patients)); }));
          });
    }
  }
}