namespace Synthea.Iota.Ui
{
  using System.Collections.Generic;
  using System.Windows.Controls;
  using System.Windows.Input;

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

    private static TreeViewItem StripElementValue(Base resource)
    {
      if (resource is CodeableConcept codeable)
      {
        return new TreeViewItem { Header = codeable.Text };
      }

      if (resource is SimpleQuantity quantity && quantity.Value.HasValue)
      {
        return new TreeViewItem { Header = $"{quantity.Value} {quantity.Unit}" };
      }

      return new TreeViewItem { Header = resource.GetType().FullName };
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

        if (resource.Resource.GetType().GetProperty("Code") != null)
        {
          treeViewItem.Items.Add(
            new TreeViewItem
              {
                Header = ((CodeableConcept)resource.Resource.GetType().GetProperty("Code")?.GetValue(resource.Resource, null))?.Text
              });
          if (resource.Resource is Observation observation && observation.Value != null)
          {
            treeViewItem.Items.Add(StripElementValue(observation.Value));
          }
        }
        else
        {
          foreach (var child in resource.Children)
          {
            var childItem = new TreeViewItem { Header = child.TypeName };

            childItem.Items.Add(StripElementValue(child));

            treeViewItem.Items.Add(childItem);
          }
        }

        this.PatientDetails.Items.Add(treeViewItem);
      }
    }
  }
}