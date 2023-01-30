using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinFormsApp1
{
    public partial class Uc3dPanel : UserControl
    {
        Process process;
        /// <summary>
        /// U3d的运行文件
        /// </summary>
        public static string U3dFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "Game2.exe");
        public Uc3dPanel()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.Selectable, true);
            this.Resize += Uc3dPanel_Resize;
            //在为控件创建句柄时发生
            HandleCreated += (_, __) => OpenU3d();
            //在控件的句柄处于销毁过程中时发生
            //特别注意，其父控件（或者祖先控件）需要在适当的位置显式调用Dispose()函数
            HandleDestroyed += (_, __) => CloseU3d();
        }

        private void Uc3dPanel_Resize(object? sender, EventArgs e)
        {
            if (process == null || process.MainWindowHandle == IntPtr.Zero) return;
            MoveWindow(process.MainWindowHandle, 0, 0, this.Width, this.Height, true);
        }

        private void OpenU3d()
        {
            FileInfo fileInfo = new FileInfo(U3dFile);
            var U3dName = fileInfo.Name.Replace(".exe", "");//事先清理额外的进程
            foreach (Process process in Process.GetProcesses().Where(p => p.ProcessName == U3dName))
            {
                Debug.WriteLine("清理额外的U3d进程，进程编号：{0}", process.Id);
                process.Kill();
            }
            if (fileInfo.Exists)
            {
                process = new Process();
                process.StartInfo.FileName = U3dFile;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForInputIdle();
                Thread.Sleep(100);//稍微等一下句柄
                SetParent(process.MainWindowHandle, this.Handle);
                ShowWindow(process.MainWindowHandle, (int)ProcessWindowStyle.Maximized);
            }
        }

        /// <summary>
        /// 关闭的Unity3d的程序
        /// </summary>
        /// <param name="guid"></param>
        private void CloseU3d()
        {
            try
            {
                if (process != null)
                {
                    process.CloseMainWindow();
                    Thread.Sleep(1000);
                    while (!process.HasExited)
                        process.Kill();
                }
            }
            catch (Exception)
            {

            }
        }


        [DllImport("User32.dll", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);
    }
}
