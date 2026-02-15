using Avalonia;
using Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UtilitiesManager;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly CheckDependencyCommand _checker = new();
    private readonly ChangeValueCommand _changer = new();  

    private int _soundLevel;
    private int _brightness;

    public int SoundLevel
    {
        get => _soundLevel;
        set
        {
            if (_soundLevel != value)
            {
                _soundLevel = value;
                _ = _changer.SetVolumeAsync(value);  // ← now actually sets volume
                OnPropertyChanged();
            }
        }
    }

    public int Brightness
    {
        get => _brightness;
        set
        {
            if (_brightness != value)
            {
                _brightness = value;
                _ = _changer.SetBrightnessAsync(value);  // ← now actually sets brightness
                OnPropertyChanged();
            }
        }
    }

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        DataContext = this;
        InitializeValues();
    }

    private async void InitializeValues()
    {
        await _checker.LoadOriginalValuesAsync();
        SoundLevel = _checker.OriginalValueSound;
        Brightness = _checker.OriginalValueLight;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}