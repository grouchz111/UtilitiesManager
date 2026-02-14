using Avalonia;
using Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UtilitiesManager;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private int _soundLevel = 50;
    private int _brightness = 75;

    public int SoundLevel
    {
        get => _soundLevel;
        set
        {
            if (_soundLevel != value)
            {
                _soundLevel = value;
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

        // Optional: set initial DataContext to self (code-behind style)
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}