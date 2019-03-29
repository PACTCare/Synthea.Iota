namespace Synthea.Iota.Ui
{
  using System.Windows.Controls;
  using System.Windows.Media.Animation;

  /// <summary>
  /// Interaction logic for LoadingSpinner.xaml
  /// </summary>
  public partial class LoadingSpinner : UserControl
  {
    public LoadingSpinner()
    {
      this.InitializeComponent();
    }

    public void SetText(string text)
    {
      ((TextBlock)this.FindName("UpdateDescription")).Text = text;
    }

    public void Start()
    {
      ((Storyboard)this.FindResource("WaitStoryboard")).Begin();
    }

    public void Stop()
    {
      ((Storyboard)this.FindResource("WaitStoryboard")).Stop();
    }
  }
}