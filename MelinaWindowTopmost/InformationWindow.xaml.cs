using System.Windows;
using MelinaWindowTopmost.Services;
using MelinaWindowTopmost.ViewModels;

namespace MelinaWindowTopmost;

public partial class InformationWindow : Window
{
    public InformationWindow(WindowItemViewModel window, LocalizationService localization)
    {
        DataContext = new InformationWindowModel(window, localization);
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private sealed class InformationWindowModel
    {
        public InformationWindowModel(WindowItemViewModel window, LocalizationService localization)
        {
            Title = string.IsNullOrWhiteSpace(window.Title) ? localization["Information"] : window.Title;
            Header = window.Title;
            Subtitle = window.Subtitle;
            Handle = window.HandleText;
            WindowTitle = window.Title;
            Process = string.IsNullOrWhiteSpace(window.ProcessFileName) ? localization["NoProcessPath"] : window.ProcessFileName;
            Path = window.ProcessPath;
            HandleLabel = localization["Handle"];
            TitleLabel = localization["Title"];
            ProcessLabel = localization["Process"];
            PathLabel = localization["Path"];
            CloseText = localization["Close"];
        }

        public string Title { get; }

        public string Header { get; }

        public string Subtitle { get; }

        public string Handle { get; }

        public string WindowTitle { get; }

        public string Process { get; }

        public string Path { get; }

        public string HandleLabel { get; }

        public string TitleLabel { get; }

        public string ProcessLabel { get; }

        public string PathLabel { get; }

        public string CloseText { get; }
    }
}
