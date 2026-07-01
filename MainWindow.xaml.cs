using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using System.Diagnostics;
using Windows.Media.Audio;
using Microsoft.VisualBasic.Devices;
namespace BAU
{

    
    public partial class MainWindow : Window
    {
        private AudioPlaybackConnection _connection;
        private NotifyIcon HideIcon;
        private bool hide = false;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayICon();
            
        }
        private void InitializeTrayICon()
        {
            HideIcon = new NotifyIcon();
            HideIcon.Icon = System.Drawing.SystemIcons.Application;
            HideIcon.Text = "实用小工具";
            HideIcon.Visible = true;
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem itemShow = new ToolStripMenuItem("显示面板");
            itemShow.Click += (s, e) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
            ToolStripMenuItem itemExit = new ToolStripMenuItem("退出");
            itemExit.Click += (s, e) => { 
                HideIcon.Visible = false;
                _connection.Dispose();
                System.Windows.Application.Current.Shutdown();
            };
            ToolStripMenuItem itemStop = new ToolStripMenuItem("停止");
            itemStop.Click += (s, e) =>
            {
                if (itemStop.Text == "停止")
                {
                    _connection.Dispose();
                    itemStop.Text = "重启";
                }
                else
                {
                    ScanDevices();
                    itemStop.Text = "停止";
                }
                
            };
            menu.Items.Add(itemShow); 
            menu.Items.Add(itemExit);
            menu.Items.Add(itemStop);
            HideIcon.ContextMenuStrip = menu;
            HideIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            hide = true;
            this.ShowInTaskbar = false;
        }
        protected override void OnClosed(EventArgs e)
        {
            if(HideIcon != null)
            {
                HideIcon.Visible = false;
                HideIcon.Dispose();
            }
            base.OnClosed(e);
        }
        private async void ScanDevices()
        {
            string selector = AudioPlaybackConnection.GetDeviceSelector();
            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(selector);
            foreach(var device in devices)
            {
                   Debug.Write($"正在激活音频协议……\n设备名字{device.Name}");
                    _connection = AudioPlaybackConnection.TryCreateFromId(device.Id);
                if (_connection == null)
                {
                    System.Windows.MessageBox.Show("激活失败，要么是手机没连上要么就是你电脑用不了，去检查一下蓝牙连接！");
                }
                    try
                    {
                        _connection.Start();
                        var result = await _connection.OpenAsync();
                    if (hide == false)
                    {
                        System.Windows.MessageBox.Show($"设备{device.Name}的连接结果为{result.Status}\n你可以关掉这个软件的窗口");
                    }
                    
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"连接失败： {ex.Message}");
                    }
                
            }

        }  

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanDevices();
        }
    }
}