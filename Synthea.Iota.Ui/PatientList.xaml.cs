namespace Synthea.Iota.Ui
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Services;
  using Synthea.Iota.Ui.Services;

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

    private static TreeViewItem VisualUpwardSearch(DependencyObject source)
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
      try
      {
        var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
        if (treeViewItem == null || treeViewItem.Items.Count < 2
                                 || !(treeViewItem.Items.GetItemAt(treeViewItem.Items.Count - 1) is ParsedResource resource))
        {
          return;
        }

        var spinner = new LoadingSpinner();
        spinner.SetText("Creating resource on Tangle");
        ApplicationManager.SetContent(spinner);

        spinner.Start();

        Task.Factory.StartNew(
          () =>
            {
              FhirInteractor.CreateResource(resource);

              this.Dispatcher.BeginInvoke(
                new Action(
                  () =>
                    {
                      spinner.Stop();
                      ApplicationManager.SetContent(new PatientList(ApplicationManager.PatientRepository.LoadPatients()));
                    }));
            });
      }
      catch
      {
        // ignored
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

        var jsonViewItem = new TreeViewItem { Header = "Json View" };
        jsonViewItem.Items.Add(new TextBox { Text = resource.FormattedJson, IsReadOnly = true });
        treeViewItem.Items.Add(jsonViewItem);

        if (resource.IsIotaResource)
        {
          var tangleDetails = new TreeViewItem { Header = "Tangle information" };
          tangleDetails.Items.Add(
            new TextBox { Text = $"http://localhost:64264/api/fhir/{resource.TypeName}/{resource.Resource.Id}", IsReadOnly = true });
          treeViewItem.Items.Add(tangleDetails);
        }

        treeViewItem.Items.Add(resource);

        this.PatientDetails.Items.Add(treeViewItem);
      }
    }
  }
}