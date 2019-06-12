namespace Synthea.Iota.Ui
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  using Synthea.Iota.Core.Entity;
  using Synthea.Iota.Core.Exception;
  using Synthea.Iota.Core.Extensions;
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

    private TreeViewItem CreateTreeViewItem(ParsedResource resource)
    {
      var menuItem = new MenuItem { Header = "Upload to Tangle" };
      var treeViewItem = new TreeViewItem
                           {
                             Header = new Label
                                        {
                                          Content = resource.TypeName,
                                          Foreground = resource.IsIotaResource ? Brushes.DarkGreen : Brushes.Black,
                                          FontWeight = FontWeights.Bold
                                        },
                             ContextMenu = new ContextMenu { ItemsSource = new List<MenuItem> { menuItem } }
                           };

      menuItem.Click += (sender, args) =>
        {
          if (sender is MenuItem)
          {
            this.UploadResource(treeViewItem, resource);
          }
        };

      var jsonViewItem = new TreeViewItem { Header = "Json View" };
      jsonViewItem.Items.Add(new TextBox { Text = resource.FormattedJson, IsReadOnly = true });
      treeViewItem.Items.Add(jsonViewItem);

      if (resource.IsIotaResource)
      {
        var tangleDetails = new TreeViewItem { Header = "Tangle information" };
        tangleDetails.Items.Add(
          new TextBox { Text = $"http://pactfhir.azurewebsites.net/api/fhir/{resource.TypeName}/{resource.Resource.Id}", IsReadOnly = true });
        treeViewItem.Items.Add(tangleDetails);
      }

      treeViewItem.Items.Add(resource);
      return treeViewItem;
    }

    private void UploadResource(TreeViewItem treeViewItem, ParsedResource resource)
    {
      try
      {
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
                        var newItem = this.CreateTreeViewItem(updatedResource);

                        foreach (ParsedPatient patient in this.Patients.ItemsSource)
                        {
                          if (patient.Resources.First().PatientId != updatedResource.PatientId)
                          {
                            continue;
                          }

                          var resourceToUpdate = patient.Resources.First(r => r.Id == updatedResource.Id);
                          if (resourceToUpdate != null)
                          {
                            resourceToUpdate.Resource = updatedResource.Resource;
                          }
                        }

                        var selectedIndex = this.PatientDetails.Items.IndexOf(treeViewItem);
                        this.PatientDetails.Items.RemoveAt(selectedIndex);
                        this.PatientDetails.Items.Insert(selectedIndex, newItem);

                        this.PatientDetails.Items.Refresh();
                        this.PatientDetails.UpdateLayout();

                        spinner.Stop();
                      }));
              }
              catch (ResourceException exception)
              {
                this.Dispatcher.BeginInvoke(
                  new Action(
                    () =>
                      {
                        MessageBox.Show(exception.Outcome.ToFormattedJson(), "Creation Failed");
                      }));
              }
              catch (Exception exception)
              {
                this.Dispatcher.BeginInvoke(
                  new Action(
                    () =>
                      {
                        MessageBox.Show(exception.StackTrace, exception.Message);
                      }));
              }
              finally
              {
                this.Dispatcher.BeginInvoke(
                  new Action(
                    () =>
                      {
                        ApplicationManager.SetContent(this);
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
        this.PatientDetails.Items.Add(this.CreateTreeViewItem(resource));
      }
    }
  }
}