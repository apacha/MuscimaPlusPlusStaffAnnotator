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

namespace MuscimaPlusPlusStaffAnnotator
{
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _rectangleX;

        private double _rectangleY;

        private Rectangle _rectangleBeingDrawn;

        public MainWindow()
        {
            InitializeComponent();
        }

        public List<string> ImagePaths { get; set; }

        public string CurrentImage { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImagePaths = new List<string>();
            var pathToImages = @"C:\Users\Alex\Repositories\CVC-MUSCIMA\CVCMUSCIMA_SR\CvcMuscima-Distortions\ideal";
            var writerDirectories = Directory.EnumerateDirectories(pathToImages);
            foreach (var writerDirectory in writerDirectories)
            {
                var idealImages = Path.Combine(writerDirectory, "image");
                foreach (var image in Directory.EnumerateFiles(idealImages))
                {
                    ImagePaths.Add(image);
                }
            }

            LoadImage(0);
        }

        private void ButtonPreviousImageClick(object sender, RoutedEventArgs e)
        {
            var index = ImagePaths.IndexOf(CurrentImage);
            LoadImage(index - 1);
        }

        private void ButtonNextImageClick(object sender, RoutedEventArgs e)
        {
            var index = ImagePaths.IndexOf(CurrentImage);
            LoadImage(index + 1);
        }

        private void LoadImage(int index)
        {
            if (index < 0 || index > ImagePaths.Count - 1)
            {
                return;
            }

            var currentImage = ImagePaths[index];
            var page = Path.GetFileName(currentImage).TrimEnd('.', 'p', 'n', 'g');
            var writer = Regex.Match(currentImage, @".*(?<writer>w-\d{2}).*").Groups["writer"];
            currentImageTextBox.Text = writer + ";" + page + ";";
            var uriSource = new Uri(currentImage, UriKind.Absolute);
            CurrentImage = currentImage;
            image.Source = new BitmapImage(uriSource);
            imageCanvas.Children.Clear();
        }

        private void imageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Sienna;
            ellipse.Width = 100;
            ellipse.Height = 100;
            ellipse.StrokeThickness = 2;

            imageCanvas.Children.Add(ellipse);

            Canvas.SetLeft(ellipse, e.GetPosition(image).X);
            Canvas.SetTop(ellipse, e.GetPosition(image).Y);
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _rectangleX = e.GetPosition(image).X;
            _rectangleY = e.GetPosition(image).Y;
            _rectangleBeingDrawn = new Rectangle();
        }

        private void DrawRectangle(MouseButtonEventArgs e)
        {
            try
            {
                Canvas.SetLeft(_rectangleBeingDrawn, _rectangleX);
                Canvas.SetTop(_rectangleBeingDrawn, _rectangleY);
                _rectangleBeingDrawn.Width = e.GetPosition(image).X - _rectangleX;
                _rectangleBeingDrawn.Height = e.GetPosition(image).Y - _rectangleY;
                _rectangleBeingDrawn.StrokeThickness = 2;
                _rectangleBeingDrawn.Stroke = new SolidColorBrush(Colors.Red);
                imageCanvas.Children.Add(_rectangleBeingDrawn);

                coordinatesTextBlock.Text = _rectangleY + ":" + e.GetPosition(image).Y;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        
        private void image_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_rectangleBeingDrawn != null)
            {
                imageCanvas.Children.Remove(_rectangleBeingDrawn);
                DrawRectangle(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
            }
            e.Handled = false;
        }

        private void imageCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _rectangleBeingDrawn = null;
        }
    }
}
