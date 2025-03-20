using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CaffeExpert
{
    public partial class Window1 : Window
    {
        private bool _dragging = false;
        private Point clickPosition;
        private Button movingButton;

        public Window1()
        {
            InitializeComponent();

            dragButton.PreviewMouseLeftButtonDown += Img_MouseLeftButtonDown;
        }

        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movingButton = (Button)sender;
            //clickPosition = e.GetPosition(movingButton);
            e.Handled = true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new button
            Button newButton = new Button
            {
                Content = "Drag Me",
                Width = 100,
                Height = 30,
                Tag = $"Button-{MainCanvas.Children.Count + 1}"
            };

            // Attach mouse event handlers for drag-and-drop and double-click
            
            newButton.PreviewMouseLeftButtonDown += Img_MouseLeftButtonDown;
            

            // Add the button to the canvas
            MainCanvas.Children.Add(newButton);
            Canvas.SetLeft(newButton, 250); // Initial position
            Canvas.SetTop(newButton, 250);
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string buttonId = clickedButton.Tag.ToString(); // Retrieve unique ID
                MessageBox.Show($"Button ID: {buttonId}");
            }
        }
        /*
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movingElement = sender as Button;

            if (movingElement.CaptureMouse())
            {
                _dragging = true;
                clickPosition = e.GetPosition(MainCanvas);
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newPoint = e.GetPosition(MainCanvas);
                double dx = newPoint.X - clickPosition.X;
                double dy = newPoint.Y - clickPosition.Y;

                Canvas.SetLeft(movingElement, Canvas.GetLeft(movingElement) + dx);
                Canvas.SetTop(movingElement, Canvas.GetTop(movingElement) + dy);
                clickPosition = newPoint;
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragging)
            {
                _dragging = false;
                movingElement.ReleaseMouseCapture();
                movingElement?.ReleaseMouseCapture();
                movingElement = null;
            }
        }
        */
        private Point _startPoint;

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (movingButton != null && movingButton.CaptureMouse())
            {
                _startPoint = e.GetPosition(null);
                _dragging = true;
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newPoint = e.GetPosition(null);
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
                _dragging = false;
                movingButton.ReleaseMouseCapture();
            }
        }
    }
}
