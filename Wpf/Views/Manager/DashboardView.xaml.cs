using LiveCharts.Wpf;
using LiveCharts;
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

namespace Wpf.Views.Manager
{
    /// <summary>
    /// Logika interakcji dla klasy DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
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
