using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MelinaWindowTopmost.Services;

namespace MelinaWindowTopmost;

public partial class OpacityWindow : Window
{
    private readonly OpacityWindowModel _model;

    public OpacityWindow(int initialOpacityPercent, LocalizationService localization)
    {
        _model = new OpacityWindowModel(initialOpacityPercent, localization);
        DataContext = _model;
        InitializeComponent();
    }

    public int OpacityPercent => _model.OpacityPercent;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private sealed class OpacityWindowModel : INotifyPropertyChanged
    {
        private int _opacityPercent;

        public OpacityWindowModel(int initialOpacityPercent, LocalizationService localization)
        {
            _opacityPercent = Math.Clamp(initialOpacityPercent, 0, 100);
            TitleText = localization["MenuSetOpacity"];
            HeaderText = localization["MenuSetOpacity"];
            OkText = localization.Language == Models.AppLanguage.ZhHans ? "确定" : "OK";
            CancelText = localization.Language == Models.AppLanguage.ZhHans ? "取消" : "Cancel";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string TitleText { get; }

        public string HeaderText { get; }

        public string OkText { get; }

        public string CancelText { get; }

        public int OpacityPercent
        {
            get => _opacityPercent;
            set
            {
                int clamped = Math.Clamp(value, 0, 100);
                if (_opacityPercent == clamped)
                {
                    return;
                }

                _opacityPercent = clamped;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OpacityText));
            }
        }

        public string OpacityText
        {
            get => $"{OpacityPercent}%";
            set
            {
                string normalized = value.Replace("%", string.Empty).Trim();
                if (int.TryParse(normalized, out int percent))
                {
                    OpacityPercent = percent;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
