using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Erp.CustomControls
{
    /// <summary>
    /// Interaction logic for FlatSearchWindow.xaml
    /// </summary>
    public partial class FlatSearchWindow : Window
    {
        #region Settings
        public object Selected_FlatSearchViewModel { get; private set; }

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

        #endregion


        #region Window,Buttons
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
                //MaximizeButton.Content = "❐";
                //MaximizeButton.ToolTip = "Restore Down";
            }
            else
            {
                this.WindowState = WindowState.Normal;
                //MaximizeButton.Content = "□";
                //MaximizeButton.ToolTip = "Maximize";
            }
        }
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        }
        #endregion

        public FlatSearchWindow(UserControl contentControl)
        {
            InitializeComponent();
            // Place the UserControl inside the ContentControl
            FlatSearchContentHost.Content = contentControl;

            // Optional: set its DataContext if you want to access its ViewModel
            // Example: contentControl.DataContext = someViewModel;

            TitleTextBlock.Text = "Filter your Data";

            SourceInitialized += Window_SourceInitialized;

            // Allow dragging by title bar
            TitleTextBlock.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    this.DragMove();
            };

            // Dynamically subscribe to a RequestClose event if it exists in the DataContext
            var viewModel = contentControl.DataContext;
            if (viewModel != null)
            {
                var eventInfo = viewModel.GetType().GetEvent("RequestClose");
                if (eventInfo != null)
                {
                    // Use reflection to attach a handler
                    var handler = new Action<bool?>(ClosePopup);
                    eventInfo.AddEventHandler(viewModel, handler);
                }
            }
        }
        private void ClosePopup(bool? dialogResult)
        {
            try
            {
                this.DialogResult = dialogResult;
            }
            catch (InvalidOperationException)
            {
                // Not opened with ShowDialog(), ignore
            }
            this.Close();
        }


    }
}
