namespace Synthea.Iota.Ui
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

  using Hl7.Fhir.Serialization;

  using Newtonsoft.Json;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Exception;
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

    private static TreeViewItem CreateTreeViewItem(ParsedResource resource)
    {
      var treeViewItem = new TreeViewItem
                           {
                             Header = new Label
                                        {
                                          Content = resource.TypeName,
                                          Foreground = resource.IsIotaResource ? Brushes.DarkGreen : Brushes.Black,
                                          FontWeight = FontWeights.Bold
                                        }
                           };

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
      return treeViewItem;
    }

    private static TreeViewItem VisualUpwardSearch(DependencyObject source)
    {
      while (source != null && !(source is TreeViewItem))
      {
        source = VisualTreeHelper.GetParent(source);
      }

      return source as TreeViewItem;
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
              try
              {
                var updatedResource = FhirInteractor.CreateResource(resource);

              this.Dispatcher.BeginInvoke(
                new Action(
                  () =>
                    {
                      spinner.Stop();
                      var newItem = CreateTreeViewItem(updatedResource);

                      var selectedIndex = this.PatientDetails.Items.IndexOf(treeViewItem);

                      foreach (ParsedPatient patient in this.Patients.ItemsSource)
                      {
                        if (patient.Resources.First().PatientId != updatedResource.PatientId)
                        {
                          continue;
                        }

                        var resourceToUpdate = patient.Resources.FirstOrDefault(r => r.Id == updatedResource.Id);
                        if (resourceToUpdate != null)
                        {
                          resourceToUpdate.Resource = updatedResource.Resource;
                        }
                      }

                      this.PatientDetails.Items.RemoveAt(selectedIndex);
                      this.PatientDetails.Items.Insert(selectedIndex, newItem);
                      this.PatientDetails.Items.Refresh();
                      this.PatientDetails.UpdateLayout();

                      ApplicationManager.SetContent(this);
                    }));
              }
              catch (ResourceException exception)
              {
                this.Dispatcher.BeginInvoke(
                  new Action(
                    () =>
                      {
                        ApplicationManager.SetContent(this);
                        MessageBox.Show(
                          JsonConvert.SerializeObject(JsonConvert.DeserializeObject(exception.Outcome.ToJson()), Formatting.Indented),
                          "Creation Failed");
                      }));
              }
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
        this.PatientDetails.Items.Add(CreateTreeViewItem(resource));
      }
    }
  }
}