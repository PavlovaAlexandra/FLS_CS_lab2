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
using System.Net;
using System.IO;
using ExcelDataReader;
using System.Data;

namespace lab2
{
    public partial class MainWindow : Window
    {
        public List<Threat> threats = new List<Threat>();
        public List<ThreatView> threatViews = new List<ThreatView>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", "thrlist.xlsx");
                updateCheck();
                File.Delete("thrlistOld.xlsx");
                threats.Clear();
                threatViews.Clear();
                File.Copy("thrlist.xlsx", "thrlistOld.xlsx");
                Parsing("thrlist.xlsx");
            }
            catch (Exception)
            {
                MessageBox.Show($"Все сломалось:(\r{e}");
                throw;
            }
        }
        public void Parsing(string fileName)
        {
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    DataSet result = streamReader.AsDataSet();
                    
                    streamReader.Read();
                    streamReader.Read();
                    while (streamReader.Read())
                    {
                        threats.Add(new Threat(streamReader.GetDouble(0),
                            streamReader.GetString(1),
                            streamReader.GetString(2),
                            streamReader.GetString(3),
                            streamReader.GetString(4),
                            streamReader.GetDouble(5) == 1 ? true : false,
                            streamReader.GetDouble(6) == 1 ? true : false,
                            streamReader.GetDouble(7) == 1 ? true : false,
                            streamReader.GetDateTime(8),
                            streamReader.GetDateTime(9)));
                    };
                    
                    foreach (var item in threats)
                    {
                        threatViews.Add(new ThreatView(item.Id, item.Name));
                    }

                    Pagination(1);
                    pageNumber.Text = "1";
                };
            };
        }
        public void Pagination(int inputValue)
        {
            List<ThreatView> threatViewsShort = new List<ThreatView>();
            if ((inputValue > threats.Count/15 + 1)||(inputValue <= 0))
            {
                Pagination(1);
                pageNumber.Text = "1";
                return;
            }

            for (int i = 0; i < 15; i++)
            {
                if (i + inputValue * 15 - 15 < threats.Count)
                {
                    threatViewsShort.Add(threatViews[i + inputValue * 15 - 15]);
                }
                else
                {
                    break;
                }
            }
            this.threatsElement.ItemsSource = threatViewsShort;
        }
        public void updateCheck()
        {
            List<Threat> threatsOld = threats;
            Parsing("thrlist.xlsx");
            List<Threat> threatsChangedOld = new List<Threat>();
            List<Threat> threatsChangedNew = new List<Threat>();

            for (int i = 0; i < threatsOld.Count; i++)
            {
                if (threatsOld[i].DateThreatLastChange != threats[i].DateThreatLastChange)
                {
                    threatsChangedOld.Add(threatsOld[i]);
                    threatsChangedNew.Add(threats[i]);
                }
            }

            if (threatsChangedOld.Count == 0)
            {
                MessageBox.Show($"Успешно!\rКоличество обновленных записей: 0");
            }
            else
            {
                Changes changes = new Changes();
                changes.threatsElementOld.ItemsSource = threatsChangedOld;
                changes.threatsElementNew.ItemsSource = threatsChangedNew;
                changes.Show();
            }
        }
        private void pageNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Pagination(Convert.ToInt32(pageNumber.Text));
            }
        }
    }
}
