using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Weather
{
    public partial class MainWindow : Window
    {
        private List<int> temps = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddDay_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TempInput.Text, out int temperature))
            {
                temps.Add(temperature);
                UpdateOutput();
                UpdateListBox();
                TempInput.Clear();
            }
            else
            {
                MessageBox.Show("Введите допустимое значение температуры.");
            }
        }

        private void UpdateOutput()
        {
            if (temps.Count == 0) return;
            double total = 0;
            foreach (var temp in temps)
            {
                total += temp;
            }
            double average = total / temps.Count;
            int max = int.MinValue;
            int min = int.MaxValue;
            foreach (var temp in temps)
            {
                if (temp > max)
                    max = temp;
                if (temp < min)
                    min = temp;
            }
            Dictionary<int, int> tempCounts = new Dictionary<int, int>();
            for (int i = 0; i < temps.Count; i++)
            {
                if (tempCounts.ContainsKey(temps[i]))
                {
                    tempCounts[temps[i]]++;
                }
                else
                {
                    tempCounts[temps[i]] = 1;
                }
            }
            int maxRepeats = int.MinValue;
            foreach (var kvp in tempCounts)
            {
                if (kvp.Value > maxRepeats)
                {
                    maxRepeats = kvp.Value;
                }
            }
            List<int> maxRepeatDays = new List<int>();
            for (int i = 0; i < temps.Count; i++)
            {
                if (tempCounts[temps[i]] == maxRepeats)
                {
                    maxRepeatDays.Add(i + 1);
                }
            }
            List<(int day1, int day2, int diff)> anomalFall = new List<(int, int, int)>();
            List<(int day1, int day2, int diff)> anomalUp = new List<(int, int, int)>();

            for (int i = 1; i < temps.Count; i++)
            {
                int diff = temps[i] - temps[i - 1];
                if (Math.Abs(diff) >= 10)
                {
                    if (diff > 0)
                        anomalUp.Add((i, i + 1, diff));
                    else
                        anomalFall.Add((i, i + 1, diff));
                }
            }

            AverageTempTextBlock.Text = $"{average:F2}°C";
            MaxTempTextBlock.Text = $"{max}°C";
            MinTempTextBlock.Text = $"{min}°C";
            MaxRepeatDaysTextBlock.Text = string.Join(", ", maxRepeatDays);

            if (anomalFall.Count > 0)
            {
                string anomalFallText = "";
                foreach (var item in anomalFall)
                {
                    anomalFallText += $"День {item.day1} -> День {item.day2} ({item.diff}°C), ";
                }
                anomalFallText = anomalFallText.TrimEnd(',', ' ');
                AnomalFallTextBlock.Text = anomalFallText;
            }
            else
            {
                AnomalFallTextBlock.Text = "Нет";
            }

            if (anomalUp.Count > 0)
            {
                string anomalUpText = "";
                foreach (var item in anomalUp)
                {
                    anomalUpText += $"День {item.day1} -> День {item.day2} ({item.diff}°C), ";
                }
                anomalUpText = anomalUpText.TrimEnd(',', ' ');
                AnomalUpTextBlock.Text = anomalUpText;
            }
            else
            {
                AnomalUpTextBlock.Text = "Нет";
            }
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListBox();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            if (temps.Count == 0) return;

            var filteredtemp = temps
                .Select((temp, index) => new { Temp = temp, Day = index + 1 })
                .ToList();

            if (Filter.SelectedItem is ComboBoxItem filterItem)
            {
                if (filterItem.Content.ToString() == "Минусовые температуры")
                {
                    filteredtemp = filteredtemp.Where(x => x.Temp < 0).ToList();
                }
                else if (filterItem.Content.ToString() == "Плюсовые температуры")
                {
                    filteredtemp = filteredtemp.Where(x => x.Temp >= 0).ToList();
                }
            }

            if (Sort.SelectedItem is ComboBoxItem sortItem)
            {
                if (sortItem.Content.ToString() == "По возрастанию дней")
                {
                    filteredtemp = filteredtemp.OrderBy(x => x.Day).ToList();
                }
                else if (sortItem.Content.ToString() == "По возрастанию температуры")
                {
                    filteredtemp = filteredtemp.OrderBy(x => x.Temp).ToList();
                }
            }

            DaysListBox.Items.Clear();
            foreach (var item in filteredtemp)
            {
                DaysListBox.Items.Add($"День {item.Day}: {item.Temp}°C");
            }
        }
    }
}