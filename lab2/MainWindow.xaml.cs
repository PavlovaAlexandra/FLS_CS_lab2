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
            if (File.Exists("thrlistOld.xlsx"))
            {
                threats = Parsing("thrlistOld.xlsx");

                foreach (var item in threats)
                {
                    threatViews.Add(new ThreatView(item.Id, item.Name));
                }

                Pagination(1);
                pageNumber.Text = "1";
            }
            else
            {
                MessageBox.Show($"Локальная база отсутствует.\rСкачайте данные, нажав на кнопку \"UPDATE\".");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://bdu.fstec.ru/files/documents/thrlist.xlsx", "thrlist.xlsx");

                if (File.Exists("thrlistOld.xlsx")) 
                { 
                    updateCheck();

                    File.Delete("thrlistOld.xlsx");

                    threatViews.Clear();
                }
                else
                {
                    threats = Parsing("thrlist.xlsx");
                }

                foreach (var item in threats)
                {
                    threatViews.Add(new ThreatView(item.Id, item.Name));
                }

                Pagination(1);
                pageNumber.Text = "1";
            }
            catch (Exception)
            {
                MessageBox.Show($"Все сломалось:(\r{e}");
                throw;
            }
        }
        public List<Threat> Parsing(string fileName)
        {
            List<Threat> threatsParsing = new List<Threat>();

            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    DataSet result = streamReader.AsDataSet();
                    
                    streamReader.Read();
                    streamReader.Read();
                    while (streamReader.Read())
                    {
                        threatsParsing.Add(new Threat(streamReader.GetDouble(0),
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
                };
            };
            return threatsParsing;
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
            List<Threat> threatsOld = Parsing("thrlistOld.xlsx");

            threats.Clear();
            threats = Parsing("thrlist.xlsx");

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
                if (int.TryParse(pageNumber.Text, out int pageNum))
                {
                    Pagination(pageNum);
                }
                else
                {
                    MessageBox.Show("Некорректное занчение.\r Введите число.");
                    Pagination(1);
                }
            }
        }

        private void threatsElement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int i = threatsElement.SelectedIndex;
            threatInfo threatInfo = new threatInfo();
            threatInfo.idValue.Content = threats[threatsElement.SelectedIndex].Id;
            threatInfo.nameBlockValue.Text = threats[threatsElement.SelectedIndex].Name;
            threatInfo.descriptionBlockValue.Text = threats[threatsElement.SelectedIndex].Description;
            threatInfo.sourceBlockValue.Text = threats[threatsElement.SelectedIndex].SourceOfThreat;
            threatInfo.impactBlockValue.Text = threats[threatsElement.SelectedIndex].Impact;
            threatInfo.breachOfConfidentialityValue.Content = threats[threatsElement.SelectedIndex].BreachOfConfidentiality ? "да" : "нет";
            threatInfo.breachOfIntegrityValue.Content = threats[threatsElement.SelectedIndex].BreachOfIntegrity ? "да" : "нет";
            threatInfo.breachOfAvailabilityValue.Content = threats[threatsElement.SelectedIndex].BreachOfAvailability ? "да" : "нет";
            threatInfo.dateThreatValue.Content = threats[threatsElement.SelectedIndex].DateThreat;
            threatInfo.dateThreatLastChangeValue.Content = threats[threatsElement.SelectedIndex].DateThreatLastChange;
            threatInfo.Show();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            File.Delete("thrlistOld.xlsx");
            File.Copy("thrlist.xlsx", "thrlistOld.xlsx");
            MessageBox.Show("База успешно сохранена.");
        }

        private void previousPage_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(pageNumber.Text, out int pageNum))
            {
                pageNumber.Text = Convert.ToString(pageNum - 1);
                Pagination(pageNum - 1);
            }
            else
            {
                Pagination(1);
                pageNumber.Text = "1";
            }
        }

        private void nextPage_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(pageNumber.Text, out int pageNum))
            {
                pageNumber.Text = Convert.ToString(pageNum + 1);
                Pagination(pageNum + 1);
            }
            else
            {
                Pagination(1);
                pageNumber.Text = "1";
            }
        }
    }
}
