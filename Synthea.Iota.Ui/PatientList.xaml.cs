namespace Synthea.Iota.Ui
{
  using System.Collections.Generic;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media.Animation;

  using Hl7.Fhir.Model;

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

        if (resource.Resource.GetType().GetProperty("Code") != null)
        {
          treeViewItem.Items.Add(new TreeViewItem { Header = ((CodeableConcept)resource.Resource.GetType().GetProperty("Code")?.GetValue(resource.Resource, null))?.Text });
        }
        else
        {
          foreach (var child in resource.Children)
          {
            var childItem = new TreeViewItem { Header = child.TypeName };

            if (child is CodeableConcept codeable)
            {
              childItem.Items.Add(new TreeViewItem { Header = codeable.Text });
            }

            treeViewItem.Items.Add(childItem);
          }
        }

        this.PatientDetails.Items.Add(treeViewItem);
      }
    }

    private void PatientDetails_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (((TreeView)sender).SelectedItem is ParsedResource resource)
      {

      }
    }
  }
}