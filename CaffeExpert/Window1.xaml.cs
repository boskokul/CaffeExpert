using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace CaffeExpert
{
    public partial class Window1 : Window
    {
        private bool _dragging = false;
        private Button movingButton;
        private Point _startPoint;
        private string FilePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"), "tables.xml");

        public Window1()
        {
            InitializeComponent();
            
            AddExistingTables();
        }

        private void AddExistingTables()
        {
            var tables = GetTables();
            foreach (var table in tables)
            {
                Debug.WriteLine($"Table loaded at X: {table.X}, Y: {table.Y}");
                Button newButton = new Button
                {
                    Content = table.Tag,
                    Width = 120,
                    Height = 50,
                    Tag = table.Tag
                };

                StyleButton(newButton);

                newButton.PreviewMouseLeftButtonDown += Img_MouseLeftButtonDown;

                MainCanvas.Children.Add(newButton);

                Canvas.SetLeft(newButton, table.X);
                Canvas.SetTop(newButton, table.Y);
            }
        }

        private void StyleButton(Button newButton)
        {

            var imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/wood.png")),
                Stretch = Stretch.Fill
            };

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, imageBrush);
            borderFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));

            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetValue(TextBlock.TextProperty, new Binding("Content") { Source = newButton });
            textBlockFactory.SetValue(TextBlock.FontSizeProperty, 17.0);
            textBlockFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textBlockFactory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(textBlockFactory);

            newButton.Template = new ControlTemplate(typeof(Button))
            {
                VisualTree = borderFactory
            };
        }
        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movingButton = (Button)sender;
            e.Handled = true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = fileNameTextBox.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please enter a file name.");
            }
            else
            {
                Button newButton = new Button
                {
                    Content = fileName,
                    Width = 120,
                    Height = 50,
                    Tag = fileName,
                };

                StyleButton(newButton);

                newButton.PreviewMouseLeftButtonDown += Img_MouseLeftButtonDown;

                MainCanvas.Children.Add(newButton);
                Canvas.SetLeft(newButton, 0);
                Canvas.SetTop(newButton, 0);
            }
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string buttonId = clickedButton.Tag.ToString();
                MessageBox.Show($"Button ID: {buttonId}");
            }
        }


        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (movingButton != null && movingButton.CaptureMouse())
            {
                _startPoint = e.GetPosition(MainCanvas);
                _dragging = true;
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newPoint = e.GetPosition(MainCanvas);
                double dx = newPoint.X - _startPoint.X;
                double dy = newPoint.Y - _startPoint.Y;

                Canvas.SetLeft(movingButton, Canvas.GetLeft(movingButton) + dx);
                Canvas.SetTop(movingButton, Canvas.GetTop(movingButton) + dy);
                _startPoint = newPoint;
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragging)
            {
                var table = new Table();
                table.X = Canvas.GetLeft(movingButton);
                table.Y = Canvas.GetTop(movingButton);
                Debug.WriteLine($"Dropped at X: {table.X}, Y: {table.Y}");
                table.Tag = (string?)movingButton.Tag;
                AddTable(table);
                _dragging = false;
                movingButton.ReleaseMouseCapture();
            }
        }

        private List<Table> GetTables()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Table>));
                using (var fileStream = new FileStream(FilePath, FileMode.Open))
                {
                    return (List<Table>)serializer.Deserialize(fileStream);
                }
            }catch (Exception ex)
            {
                // dodacu bolje logovanje ili prepraviti ovaj slucaj
                Debug.WriteLine($"Exception: {ex}");
                return new List<Table>();
            }
        }

        private Table AddTable(Table table)
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
            }
            var tables = GetTables();
            var oldTable = tables.Find(t => t.Tag == table.Tag);
            if (oldTable != null)
            {
                oldTable.X = table.X;
                oldTable.Y = table.Y;
            }
            else
            {
                tables.Add(table);
            }
            SaveTables(tables);
            return table;
        }

        private void SaveTables(List<Table> tables)
        {
            var serializer = new XmlSerializer(tables.GetType());
            using (var writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, tables);
            }
        }
    }
}
