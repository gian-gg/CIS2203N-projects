using System.Globalization;

namespace BmiCalcMaui;

public partial class MainPage : ContentPage
{
    private const string DefaultHint = "Healthy BMI range is about 18.5 to 24.9";

    public MainPage()
    {
        InitializeComponent();
        UnitPicker.SelectedIndex = 0;
        SetInputPlaceholders();
        ResetResult();
    }

    private void OnUnitPickerChanged(object? sender, EventArgs e)
    {
        SetInputPlaceholders();
        ResetResult();
    }

    private void OnCalculateClicked(object? sender, EventArgs e)
    {
        var isMetric = UnitPicker.SelectedIndex != 1;

        if (!TryReadPositiveNumber(HeightEntry.Text, out var height))
        {
            ShowError(isMetric ? "Enter a valid height in cm." : "Enter a valid height in inches.");
            return;
        }

        if (!TryReadPositiveNumber(WeightEntry.Text, out var weight))
        {
            ShowError(isMetric ? "Enter a valid weight in kg." : "Enter a valid weight in lb.");
            return;
        }

        var bmi = isMetric
            ? CalculateMetricBmi(height, weight)
            : CalculateImperialBmi(height, weight);

        var category = GetBmiCategory(bmi);

        BmiValueLabel.Text = bmi.ToString("0.0", CultureInfo.InvariantCulture);
        BmiHintLabel.Text = $"{category} | Healthy range: 18.5 to 24.9";
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        HeightEntry.Text = string.Empty;
        WeightEntry.Text = string.Empty;
        UnitPicker.SelectedIndex = 0;
        SetInputPlaceholders();
        ResetResult();
    }

    private static bool TryReadPositiveNumber(string? rawValue, out double parsedValue)
    {
        var normalizedValue = (rawValue ?? string.Empty).Trim().Replace(',', '.');

        var wasParsed = double.TryParse(
            normalizedValue,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out parsedValue);

        return wasParsed && parsedValue > 0;
    }

    private static double CalculateMetricBmi(double heightCm, double weightKg)
    {
        var heightMeters = heightCm / 100d;
        return weightKg / (heightMeters * heightMeters);
    }

    private static double CalculateImperialBmi(double heightInches, double weightPounds)
    {
        return (703d * weightPounds) / (heightInches * heightInches);
    }

    private static string GetBmiCategory(double bmi)
    {
        if (bmi < 18.5)
        {
            return "Underweight";
        }

        if (bmi < 25)
        {
            return "Healthy";
        }

        if (bmi < 30)
        {
            return "Overweight";
        }

        return "Obese";
    }

    private void SetInputPlaceholders()
    {
        var isMetric = UnitPicker.SelectedIndex != 1;

        HeightEntry.Placeholder = isMetric ? "Height (cm)" : "Height (in)";
        WeightEntry.Placeholder = isMetric ? "Weight (kg)" : "Weight (lb)";
    }

    private void ResetResult()
    {
        BmiValueLabel.Text = "--";
        BmiHintLabel.Text = DefaultHint;
    }

    private void ShowError(string message)
    {
        BmiValueLabel.Text = "--";
        BmiHintLabel.Text = message;
    }
}
