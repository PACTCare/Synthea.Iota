namespace Synthea.Iota.Ui
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using Pact.Fhir.Core.Usecase.CreateResource;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Repository;
  using Synthea.Iota.Core.Services;

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

    static TreeViewItem VisualUpwardSearch(DependencyObject source)
    {
      while (source != null && !(source is TreeViewItem))
      {
        source = VisualTreeHelper.GetParent(source);
      }

      return source as TreeViewItem;
    }

    private void PatientDetails_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (((TreeView)sender).SelectedItem is ParsedResource resource)
      {
      }
    }

    private void PatientDetails_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
      if (treeViewItem == null)
      {
        return;
      }

      if (treeViewItem.Items.GetItemAt(1) is ParsedResource resource)
      {
        Task.Factory.StartNew(
          () =>
            {
              var response = FhirInteractor.CreateResource(resource);
              new SqlLitePatientRepository().UpdateResource(new ParsedResource { Resource = response, Id = resource.Id });
            });
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
        treeViewItem.Items.Add(resource);

        this.PatientDetails.Items.Add(treeViewItem);
      }
    }
  }
}