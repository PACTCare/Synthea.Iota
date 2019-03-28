namespace Synthea.Iota.Ui
{
  using System.Collections.Generic;
  using System.Windows.Controls;

  using Synthea.Iota.Core.Entity;

  /// <summary>
  /// Interaction logic for PatientList.xaml
  /// </summary>
  public partial class PatientList : Page
  {
    public PatientList(IEnumerable<ParsedPatient> patients)
    {
      this.InitializeComponent();
      this.Patients.ItemsSource = patients;
    }

    private void Patients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var patient = e.AddedItems[0] as ParsedPatient;
      this.PatientDetails.ItemsSource = patient?.Resources;
    }
  }
}