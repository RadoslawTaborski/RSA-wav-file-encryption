using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using AForge.Math;
using System.Windows.Controls.DataVisualization;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace wav2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WavFile wav;
        public byte[] xyz;
        public ObservableCollection<DataPoint> Points { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            this.MinWidth = 550;
            this.MinHeight = 488;
            dataGrid.Items.Add(new { Name = "ChrunkID", Value = "" });
            dataGrid.Items.Add(new { Name = "ChrunkSize", Value = "" });
            dataGrid.Items.Add(new { Name = "Format", Value = "" });
            dataGrid.Items.Add(new { Name = "Subchunk1ID", Value = "" });
            dataGrid.Items.Add(new { Name = "Subchunk1Size", Value = "" });
            dataGrid.Items.Add(new { Name = "AudioFormat", Value = "" });
            dataGrid.Items.Add(new { Name = "NumChanels", Value = "" });
            dataGrid.Items.Add(new { Name = "SampleRate", Value = "" });
            dataGrid.Items.Add(new { Name = "ByteRate", Value = "" });
            dataGrid.Items.Add(new { Name = "BlockAlign", Value = "" });
            dataGrid.Items.Add(new { Name = "BitsPerSample", Value = "" });
            dataGrid.Items.Add(new { Name = "Subchunk2ID", Value = "" });
            dataGrid.Items.Add(new { Name = "Subchunk2Size", Value = "" });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog okienko = new OpenFileDialog();
            okienko.Filter = "Pliki (wav)|*.wav";
            if (okienko.ShowDialog() == true)
            {
                Chart1.Series.Clear();
                Chart1.Axes.Clear();

                tbPath.Text = okienko.FileName;
                Points = new ObservableCollection<DataPoint>();
                byte[] bytes = System.IO.File.ReadAllBytes(okienko.FileName);
                wav = new WavFile(bytes);

                dataGrid.Items.Clear();
                dataGrid.Items.Add(new { Name = "ChrunkID", Value = wav.ChrunkID });
                dataGrid.Items.Add(new { Name = "ChrunkSize", Value = wav.ChrunkSize });
                dataGrid.Items.Add(new { Name = "Format", Value = wav.Format });
                dataGrid.Items.Add(new { Name = "Subchunk1ID", Value = wav.Subchunk1ID });
                dataGrid.Items.Add(new { Name = "Subchunk1Size", Value = wav.Subchunk1Size });
                dataGrid.Items.Add(new { Name = "AudioFormat", Value = wav.AudioFormat });
                dataGrid.Items.Add(new { Name = "NumChanels", Value = wav.NumChanels });
                dataGrid.Items.Add(new { Name = "SampleRate", Value = wav.SampleRate });
                dataGrid.Items.Add(new { Name = "ByteRate", Value = wav.ByteRate });
                dataGrid.Items.Add(new { Name = "BlockAlign", Value = wav.BlockAlign });
                dataGrid.Items.Add(new { Name = "BitsPerSample", Value = wav.BitsPerSample });
                dataGrid.Items.Add(new { Name = "Subchunk2ID", Value = wav.Subchunk2ID });
                dataGrid.Items.Add(new { Name = "Subchunk2Size", Value = wav.Subchunk2Size });

                var tabCom = new Complex[1024];
                for (int i = 0; i < 1024; i++)
                {
                    tabCom[i] = new Complex(wav.Data[0, i], 0);
                }

                FourierTransform.FFT(tabCom, FourierTransform.Direction.Forward);

                for (int i = 0; i < 512; i++)
                {
                    Points.Add(new DataPoint() { X = (wav.SampleRate * i) / 511, Y = tabCom[i].Magnitude * 1000 });
                }

                var style = new Style(typeof(Polyline));
                style.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, 1d));

                var pointStyle = new Style(typeof(LineDataPoint));
                pointStyle.Setters.Add(new Setter(LineDataPoint.TemplateProperty, null));

                //var axisStyle = new Style(typeof(NumericAxisLabel));
                // pointStyle.Setters.Add(new Setter(NumericAxisLabel.StringFormatProperty, "{0:0.00000}"));

                var HideLegendStyle = new Style(typeof(Legend));
                HideLegendStyle.Setters.Add(new Setter(Legend.WidthProperty, 0.0));
                HideLegendStyle.Setters.Add(new Setter(Legend.HeightProperty, 0.0));
                HideLegendStyle.Setters.Add(new Setter(Legend.VisibilityProperty, Visibility.Collapsed));

                var series = new LineSeries
                {
                    PolylineStyle = style,
                    ItemsSource = Points,
                    DependentValuePath = "Y",
                    IndependentValuePath = "X",
                    DataPointStyle = pointStyle,
                    LegendItemStyle = null,
                };

                var axisY = new LinearAxis { Orientation = AxisOrientation.Y, Title = "Amplitude", ShowGridLines = true, };
                var axisX = new LinearAxis { Orientation = AxisOrientation.X, Title = "Frequence[Hz]", ShowGridLines = true, };

                Chart1.Series.Add(series);
                Chart1.Axes.Add(axisX);
                Chart1.Axes.Add(axisY);
                Chart1.LegendStyle = HideLegendStyle;

            }
        }

        private void Keys_Click(object sender, RoutedEventArgs e)
        {
            rsa.createKeys();
            //byte[] tabelka = new byte[] { 241, 127, 50, 185};
            //xyz = rsa.encrypt(tabelka);
            xyz =rsa.encrypt(wav.DataByte);
            File.WriteAllBytes("C:\\Users\\rados\\Desktop\\sinusEncrypt.wav", wav.HeaderByte);
            AppendAllBytes("C:\\Users\\rados\\Desktop\\sinusEncrypt.wav", xyz);
            Console.WriteLine("Koniec");
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            byte[] data;
            data = rsa.decrypt(xyz);
            File.WriteAllBytes("C:\\Users\\rados\\Desktop\\sinusDecrypt.wav", wav.HeaderByte);
            AppendAllBytes("C:\\Users\\rados\\Desktop\\sinusDecrypt.wav", data);
            Console.WriteLine("Koniec");
        }

        public static void AppendAllBytes(string path, byte[] bytes)
        {
            //argument-checking here.

            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }

    public class DataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    } // class

}
