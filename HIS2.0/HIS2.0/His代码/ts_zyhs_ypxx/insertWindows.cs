using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.Windows.Forms;
namespace ts_zyhs_ypxx
{
    class InsertWindow
    {
        /// <summary>  
        /// 将程序嵌入窗体  
        /// </summary>  
        /// <param name="pW">容器</param>  
        /// <param name="appname">程序名</param>  
        public InsertWindow(Panel pW, string appname)
        {
            this.pan = pW;
            this.LoadEvent(appname);
            pane();
        }
        public InsertWindow(Panel pW, IntPtr inptr)
        {
            this.pan = pW;
            this.LoadEvent(inptr);
            pane();
        }

        ~InsertWindow()
        {
            if (m_innerProcess != null)
            {
                m_innerProcess.Dispose();
            }
        }

        #region  函数和变量声明
        /* 
34.         * 声明 Win32 API 
35.         */

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild,
            IntPtr hWndNewParent
         );

        [DllImport("user32.dll")]
        static extern Int32 GetWindowLong(IntPtr hWnd,
            Int32 nIndex
        );

        [DllImport("user32.dll")]
        static extern Int32 SetWindowLong(IntPtr hWnd,
           Int32 nIndex,
           Int32 dwNewLong
        );

        [DllImport("user32.dll")]
        static extern Int32 SetWindowPos(IntPtr hWnd,
            IntPtr hWndInsertAfter,
            Int32 X,
           Int32 Y,
            Int32 cx,
            Int32 cy,
            UInt32 uFlags
        );

        /* 
    64.          * 定义 Win32 常数 
    65.          */
        const Int32 GWL_STYLE = -16;
        const Int32 WS_BORDER = (Int32)0x00800000L;
        const Int32 WS_THICKFRAME = (Int32)0x00040000L;

        const Int32 SWP_NOMOVE = 0x0002;
        const Int32 SWP_NOSIZE = 0x0001;
        const Int32 SWP_NOZORDER = 0x0004;
        const Int32 SWP_FRAMECHANGED = 0x0020;

        const Int32 SW_MAXIMIZE = 3;
        IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        // 目标应用程序的进程.  
        Process m_innerProcess = null;
        IntPtr _inptr;
        #endregion

        #region  容器
        private Panel pan = null;
        public Panel panel1
        {
            set { pan = value; }
            get { return pan; }
        }
        private void pane()
        {
            panel1.Anchor = AnchorStyles.Left | AnchorStyles.Top |
             AnchorStyles.Right | AnchorStyles.Bottom;
            panel1.Resize += new EventHandler(panel1_Resize);
        }
        private void panel1_Resize(object sender, EventArgs e)
        {
            // 设置目标应用程序的窗体样式.  
            IntPtr innerWnd;
            try
            {
                innerWnd = m_innerProcess.MainWindowHandle;
            }
            catch
            {
                innerWnd = _inptr;

            }
            SetWindowPos(innerWnd, IntPtr.Zero, 0, 0,
                 panel1.ClientSize.Width, panel1.ClientSize.Height,
                SWP_NOZORDER);
        }
        #endregion

        #region  相应事件
        private void LoadEvent(string appFile)
        {

            // 启动目标应用程序.  
            m_innerProcess = Process.Start(appFile);
            m_innerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //隐藏  
            // 等待, 直到那个程序已经完全启动.   
            m_innerProcess.WaitForInputIdle();

            // 目标应用程序的主窗体.  
            IntPtr innerWnd = m_innerProcess.MainWindowHandle;

            // 设置目标应用程序的主窗体的父亲(为我们的窗体).  
            SetParent(innerWnd, panel1.Handle);

            // 除去窗体边框.  
            Int32 wndStyle = GetWindowLong(innerWnd, GWL_STYLE);
            wndStyle &= ~WS_BORDER;
            wndStyle &= ~WS_THICKFRAME;
            SetWindowLong(innerWnd, GWL_STYLE, wndStyle);
            SetWindowPos(innerWnd, IntPtr.Zero, 0, 0, 0, 0,
                 SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
            // MoveWindow(innerWnd, 0, 0, this.Width, this.Height, true);
            // 在Resize事件中更新目标应用程序的窗体尺寸.  
            panel1_Resize(panel1, null);
        }
        #endregion
        private void LoadEvent(IntPtr appFile)
        {

            // 启动目标应用程序.  
            //m_innerProcess = Process.Start(appFile);
            //m_innerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //隐藏  
            //// 等待, 直到那个程序已经完全启动.   
            //m_innerProcess.WaitForInputIdle();

            // 目标应用程序的主窗体.  
            IntPtr innerWnd = appFile;
            _inptr = appFile;
            // 设置目标应用程序的主窗体的父亲(为我们的窗体).  
            SetParent(innerWnd, panel1.Handle);

            // 除去窗体边框.  
            Int32 wndStyle = GetWindowLong(innerWnd, GWL_STYLE);
            wndStyle &= ~WS_BORDER;
            wndStyle &= ~WS_THICKFRAME;
            SetWindowLong(innerWnd, GWL_STYLE, wndStyle);
            SetWindowPos(innerWnd, IntPtr.Zero, 0, 0, 0, 0,
                 SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

            // 在Resize事件中更新目标应用程序的窗体尺寸.  
            panel1_Resize(panel1, null);
        }
    }
}


