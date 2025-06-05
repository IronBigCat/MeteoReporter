using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MeteoReporterTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            SaveButton.Click += SaveButton_Click;
            ReportButton.Click += ReportButton_Click;
        }

        private async void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out double tempC, out string condition, out string comment))
                return;

            DateTime now = DateTime.Now;

            string content = $"Дата/Время: {now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}"
                           + $"Температура: {tempC:F1} °C{Environment.NewLine}"
                           + $"Состояние: {condition}{Environment.NewLine}"
                           + $"Комментарий: {comment}{Environment.NewLine}";

#pragma warning disable CS0618
            var folderDialog = new OpenFolderDialog
            {
                Title = "Выберите папку для сохранения файла"
            };
#pragma warning restore CS0618

            string? folderPath = await folderDialog.ShowAsync(this);
            if (string.IsNullOrEmpty(folderPath))
            {
                OutputTextBlock.Text = "Сохранение отменено пользователем.";
                return;
            }

            try
            {
                string fileName = $"weather_{now:yyyyMMdd_HHmmss}.txt";
                string fullPath = Path.Combine(folderPath, fileName);

                await File.WriteAllTextAsync(fullPath, content);
                OutputTextBlock.Text = $"Данные сохранены в файл:{Environment.NewLine}{fullPath}";
            }
            catch (Exception ex)
            {
                OutputTextBlock.Text = $"Ошибка при сохранении файла:{Environment.NewLine}{ex.Message}";
            }
        }

        private void ReportButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out double tempC, out string condition, out string comment))
                return;

            DateTime now = DateTime.Now;

            string report = $"ОТЧЁТ{Environment.NewLine}"
                          + $"Дата/Время: {now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}"
                          + $"Температура: {tempC:F1} °C{Environment.NewLine}"
                          + $"Состояние: {condition}{Environment.NewLine}"
                          + $"Комментарий: {comment}";

            OutputTextBlock.Text = report;

            _ = CloseWithDelay();
        }

        private bool ValidateInputs(out double tempC, out string condition, out string comment)
        {
            tempC = 0;
            condition = string.Empty;
            comment = string.Empty;

            string tempText = TemperatureTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(tempText))
            {
                OutputTextBlock.Text = "Ошибка: введите температуру.";
                return false;
            }

            if (!double.TryParse(tempText.Replace(',', '.'), out tempC))
            {
                OutputTextBlock.Text = "Ошибка: температура должна быть числом (например, 21.5).";
                return false;
            }

            if (tempC < -50 || tempC > 60)
            {
                OutputTextBlock.Text = "Ошибка: температура должна быть в диапазоне от -50 до 60 °C.";
                return false;
            }

            if (ConditionComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                condition = selectedItem.Content?.ToString() ?? "";
                if (string.IsNullOrEmpty(condition))
                {
                    OutputTextBlock.Text = "Ошибка: выберите состояние погоды.";
                    return false;
                }
            }
            else
            {
                OutputTextBlock.Text = "Ошибка: выберите состояние погоды.";
                return false;
            }

            comment = CommentTextBox.Text?.Trim() ?? "";
            if (comment.Length > 200)
            {
                OutputTextBlock.Text = "Ошибка: комментарий слишком длинный (максимум 200 символов).";
                return false;
            }

            return true;
        }

        private async System.Threading.Tasks.Task CloseWithDelay()
        {
            await System.Threading.Tasks.Task.Delay(2000);
            this.Close();
        }
    }
}