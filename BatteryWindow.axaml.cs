using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace UtilitiesManager;

public partial class BatteryWindow : Window  // ← assuming you renamed the class too
{
    public BatteryWindow()
    {
        InitializeComponent();
        Opened += BatteryWindow_Opened;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void BatteryWindow_Opened(object? sender, EventArgs e)
    {
        await RefreshBatteryDataAsync();
    }

    private async Task RefreshBatteryDataAsync()
    {
        var checker = new CheckDependencyCommand();
        var status = await checker.GetBatteryAsync();

        if (this.FindControl<TextBlock>("PercentageText") is TextBlock pct)
        {
            pct.Text = status.Percentage >= 0 ? $"{status.Percentage}%" : "N/A";
        }

        if (this.FindControl<TextBlock>("StateText") is TextBlock state)
        {
            state.Text = status.State;
        }

        if (this.FindControl<TextBlock>("TimeText") is TextBlock time && !string.IsNullOrEmpty(status.State))
        {
            time.Text = status.State.Contains("discharging", StringComparison.OrdinalIgnoreCase)
                ? $"~{status.TimeToEmpty} left"
                : status.State.Contains("charging", StringComparison.OrdinalIgnoreCase)
                    ? $"~{status.TimeToFull} to full"
                    : "N/A";
        }

        if (this.FindControl<TextBlock>("PowerText") is TextBlock power)
        {
            power.Text = status.EnergyRate >= 0 ? $"{status.EnergyRate:F1} W" : "N/A";
        }
    }

    private void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        _ = RefreshBatteryDataAsync();  // async void is fine here since it's fire-and-forget UI
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}