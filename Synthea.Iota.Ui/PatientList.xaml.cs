namespace Synthea.Iota.Ui
{
  using System.Collections.Generic;
  using System.Windows.Controls;
  using System.Windows.Input;

  using Hl7.Fhir.Model;

  using Newtonsoft.Json;

  using Synthea.Iota.Core.Entity;

  /// <summary>
  /// Interaction logic for PatientList.xaml
  /// </summary>
  public partial class PatientList : UserControl
  {
    public PatientList(IEnumerable<ParsedPatient> patients)
    {
      this.InitializeComponent();
      this.Patients.ItemsSource = patients;
    }

    private void PatientDetails_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (((TreeView)sender).SelectedItem is ParsedResource resource)
      {
      }
    }

    private void Patients_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!(e.AddedItems[0] is ParsedPatient patient))
      {
        return;
      }

      this.PatientDetails.Items.Clear();
      foreach (var resource in patient.Resources)
      {
        var treeViewItem = new TreeViewItem { Header = resource.TypeName };
        treeViewItem.Items.Add(new TextBox { Text = resource.FormattedJson });

        this.PatientDetails.Items.Add(treeViewItem);
      }
    }
  }
}