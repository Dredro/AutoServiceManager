using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Views
{
    public partial class DirectorDashboardPage : Page
    {
        public DirectorDashboardPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ProfitPieChart == null)
            {
                MessageBox.Show("ProfitPieChart is NULL!");
                return;
            }

            var profitData = new List<ProfitData>
            {
                new ProfitData { Label = "Styczeń", Value = 12000 },
                new ProfitData { Label = "Luty", Value = 15000 },
                new ProfitData { Label = "Marzec", Value = 13000 },
                new ProfitData { Label = "Kwiecień", Value = 16000 }
            };

            ProfitPieChart.Series = new SeriesCollection();
            foreach (var item in profitData)
            {
                ProfitPieChart.Series.Add(new PieSeries
                {
                    Title = item.Label,
                    Values = new ChartValues<double> { item.Value },
                    DataLabels = true,
                    LabelPoint = point => point.Sum.ToString()
                });
            }

            ProfitPieChart.LegendLocation = LegendLocation.Right;
        }

        public class ProfitData
        {
            public string Label { get; set; }
            public double Value { get; set; }
        }
    }
}