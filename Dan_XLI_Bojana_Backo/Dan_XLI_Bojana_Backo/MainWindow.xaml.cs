using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Dan_XLI_Bojana_Backo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static BackgroundWorker _bw;
        private static int numberOfPage;
        private string file = @"..\..\";
        private string file1 = ".txt";
        public MainWindow()
        {
            _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            InitializeComponent();
        }

        

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            int number = numberOfPage;
            int time = 100 / number;
            int x = 0;
            for (int i = 1; i <= number; i++)
            {
                Thread.Sleep(100);
                x = i * time;
                // Calling ReportProgress() method raises ProgressChanged event
                // To this method pass the percentage of processing that is complete
                _bw.ReportProgress(x);

                // Check if the cancellation is requested
                if (_bw.CancellationPending)
                {
                    // Set Cancel property of DoWorkEventArgs object to true
                    e.Cancel = true;
                    // Reset progress percentage to ZERO and return
                    _bw.ReportProgress(0);
                    return;
                }
            }
            // Store the result in Result property of DoWorkEventArgs object
            e.Result = x;
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            label.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                label.Text = "Printing cancelled";
            }
            else if (e.Error != null)
            {
                label.Text = e.Error.Message;
            }
            else
            {
                if ((int)e.Result > 100)
                {
                    label.Text = "Print completed";
                    MessageBox.Show("Text printed to files!!!");
                    btnCancel.IsEnabled = false;
                    btnPrinting.IsEnabled = true;
                }
                else
                    label.Text = e.Result.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btnPrinting.IsEnabled = false;
            btnCancel.IsEnabled = true;
            int number = int.Parse(tbPageNumber.Text);
            numberOfPage = number;
            // Check if the backgroundWorker is already busy running the asynchronous operation
            if (!_bw.IsBusy)
            {
                // This method will start the execution asynchronously in the background
                _bw.RunWorkerAsync();
            }
            Print();
        }

        public void Print()
        {
            DateTime dateTime = DateTime.Now;
            int number = int.Parse(tbPageNumber.Text);
            string textBoxContents = tbText.Text;
            for (int i = 1; i <= number; i++)
            {
                string day = dateTime.Day.ToString();
                string month = dateTime.Month.ToString();
                string year = dateTime.Year.ToString();
                string hour = dateTime.Hour.ToString();
                string minute = dateTime.Minute.ToString();
                string fullString = string.Format($"{file}{i}.{day}_{month}_{year}_{hour}_{minute}{file1}");
                File.Create(fullString).Close();
                File.WriteAllText(fullString, textBoxContents);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_bw.IsBusy)
            {
                // Cancel the asynchronous operation if still in progress
                _bw.CancelAsync();
            }
        }

        private void tbPageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(tbPageNumber.Text, "[^0-9]"))
            {
                MessageBox.Show("Enter the NUMBER of pages");
                tbPageNumber.Text = tbPageNumber.Text.Remove(tbPageNumber.Text.Length - 1);
            }
        }

        private void tbText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbText.Text.Length == 0)
            {
                btnPrinting.IsEnabled = false;
            }
            else
            {
                btnPrinting.IsEnabled = true;
            }
        }
    }
}
