using Erp.Model;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Erp.CustomControls
{
    /// <summary>
    /// Interaction logic for F7PopupWindow.xaml
    /// </summary>
    public partial class F7PopupWindow : Window
    {
        public F7PopupViewModel ViewModel { get; private set; }
        public object SelectedItem { get; private set; }

        private readonly Action<object> changeCanExecuteCallback;

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;

        private const int WM_NCHITTEST = 0x0084;

        private const int resizeBorderThickness = 6; // pixels for resize border thickness

        public F7PopupWindow(F7Data f7Data, Action<object> changeCanExecute)
        {
            InitializeComponent();

            changeCanExecuteCallback = changeCanExecute;

            TitleTextBlock.Text = string.IsNullOrEmpty(f7Data.F7Title) ? "Select Item" : f7Data.F7Title;

            ViewModel = new F7PopupViewModel(f7Data);
            this.F7PopupControl.DataContext = ViewModel;

            ViewModel.ItemSelected += item =>
            {
                SelectedItem = item;
                changeCanExecuteCallback?.Invoke(item);
                this.DialogResult = true;
            };

            TitleTextBlock.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    this.DragMove();
            };

            SourceInitialized += Window_SourceInitialized;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                handled = true;
                Point pos = GetMousePosition(lParam);

                // Convert to window coordinates
                var relativePos = this.PointFromScreen(pos);

                // Determine if mouse is within resize border zones
                if (relativePos.Y <= resizeBorderThickness)
                {
                    if (relativePos.X <= resizeBorderThickness)
                        return new IntPtr(HTTOPLEFT);
                    else if (relativePos.X >= ActualWidth - resizeBorderThickness)
                        return new IntPtr(HTTOPRIGHT);
                    else
                        return new IntPtr(HTTOP);
                }
                else if (relativePos.Y >= ActualHeight - resizeBorderThickness)
                {
                    if (relativePos.X <= resizeBorderThickness)
                        return new IntPtr(HTBOTTOMLEFT);
                    else if (relativePos.X >= ActualWidth - resizeBorderThickness)
                        return new IntPtr(HTBOTTOMRIGHT);
                    else
                        return new IntPtr(HTBOTTOM);
                }
                else if (relativePos.X <= resizeBorderThickness)
                {
                    return new IntPtr(HTLEFT);
                }
                else if (relativePos.X >= ActualWidth - resizeBorderThickness)
                {
                    return new IntPtr(HTRIGHT);
                }
                else
                {
                    // Allow dragging from title bar area (height 30 px)
                    if (relativePos.Y <= 30)
                        return new IntPtr(HTCAPTION);

                    return new IntPtr(HTCLIENT);
                }
            }
            return IntPtr.Zero;
        }

        private Point GetMousePosition(IntPtr lParam)
        {
            int x = lParam.ToInt32() & 0xFFFF;
            int y = (lParam.ToInt32() >> 16) & 0xFFFF;

            // Handle negative values for high word (y)
            if (y > 32767) y -= 65536;
            if (x > 32767) x -= 65536;

            return new Point(x, y);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                MaximizeButton.Content = "❐";
                MaximizeButton.ToolTip = "Restore Down";
            }
            else
            {
                this.WindowState = WindowState.Normal;
                MaximizeButton.Content = "□";
                MaximizeButton.ToolTip = "Maximize";
            }
        }
    }
}