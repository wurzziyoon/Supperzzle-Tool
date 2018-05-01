using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SupperzzleTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32", EntryPoint = "FindWindow")]
        private static extern int FindWindow(
                string lpClassName,
                string lpWindowName
        );
        [DllImport("user32", EntryPoint = "GetWindowRect")]
        private static extern int GetWindowRect(
                int hwnd,
                ref RECT lpRect
        );
        [DllImport("user32", EntryPoint = "SetCursorPos")]
        private static extern int SetCursorPos(
                int x,
                int y
        );
        [DllImport("user32", EntryPoint = "mouse_event")]
        private static extern void mouse_event(
                int dwFlags,
                int dx,
                int dy,
                int cButtons,
                int dwExtraInfo
        );
        [DllImport("user32", EntryPoint = "RegisterHotKey")]
        private static extern int RegisterHotKey(
                int hwnd,
                int id,
                int fsModifiers,
                int vk
        );
        [DllImport("user32", EntryPoint = "UnregisterHotKey")]
        private static extern int UnregisterHotKey(
                int hwnd,
                int id
        );
        [DllImport("user32", EntryPoint = "BringWindowToTop")]
        private static extern int BringWindowToTop(
                int hwnd
        );
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;
        private const int MOUSEEVENTF_LEFTUP = 0x4;
        bool ProcessIsStart = true,IsStart=true;
        byte[,] GameRect1=new byte[8,8];
        byte[,] GameRect2 = new byte[8, 8];
        int Hwnd = 0,BestAdd=-1;
        const int GAMERECTADD = 0x189F78,GAMERECTADD2=0x48bb68, SCOREADD = 0x48edc0;
        RECT Rect=new RECT();
        Tools.Tools.GameHelper gh = new Tools.Tools.GameHelper();
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ProcessIsStart) { MessageBox.Show("游戏未启动"); return; }
                while (true)
                {
                    
                    getGameRect(ref GameRect1, GAMERECTADD);
                    clearChar(GameRect1);
                    label2.Text = "正在扫描请稍后";
                    System.Threading.Thread.Sleep(5000);
                    getGameRect(ref GameRect2, GAMERECTADD2);
                    if (getGameRect(GAMERECTADD) != GameRect1)
                    {
                        while (true)
                        {
                            BestAdd = 2;
                            IsStart = true;
                            if (gh.ReadInt(SCOREADD) >= 5000 || gh.ReadInt(SCOREADD) == 0) { IsStart = false; return; }
                            getGameRect(ref GameRect2, GAMERECTADD2);
                            clearChar(GameRect2);
                            System.Threading.Thread.Sleep(trackBar1.Value);
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            BestAdd = 1;
                            IsStart = true;
                            if (gh.ReadInt(SCOREADD) >= 5000 || gh.ReadInt(SCOREADD) == 0) { IsStart = false; return; }
                            getGameRect(ref GameRect1, GAMERECTADD);
                            clearChar(GameRect1);
                            System.Threading.Thread.Sleep(trackBar1.Value);
                        }
                    }
                    //if (gh.ReadInt(SCOREADD) >= 5000 || gh.ReadInt(SCOREADD) == 0) { IsStart = false; return; }
                    //IsStart=true;
                    //getGameRect(ref GameRect2, GAMERECTADD2);
                    //clearChar(GameRect2);
                    //System.Threading.Thread.Sleep(trackBar1.Value);
                    //getGameRect(ref GameRect1, GAMERECTADD);
                    //clearChar(GameRect1);
                   
            
                
             //   clearChar(GameRect2);
               
            }
            
            
        }
        private byte[,] getGameRect(int add)
        {
            byte[,] temp = new byte[8, 8];
            int addtemp = add;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8 + 17; y++)
                {
                    if (y < 8)
                    {
                        temp[x, y] = gh.ReadByte(addtemp);
                    }
                    addtemp++;
                }
            }
            return temp;
        }
        private void getGameRect(ref byte[,] GameRect,int Add)
        {
            int addtemp = Add;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8 + 17; y++)
                {
                    if (y < 8)
                    {
                        GameRect[x, y] = gh.ReadByte(addtemp);
                    }
                    addtemp++;
                }
            }
        }
        
        private void clearChar(byte[,] GameRect)
        {
           
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (x + 2 <= 7)
                    {
                        if (GameRect[x + 2, y] == GameRect[x, y]) 
                        {
                            if (y - 1 >= 0)//中上
                            {
                                if (GameRect[x + 1, y - 1] == GameRect[x, y])
                                {
                                    clickGameChar(x + 1, y);
                                    clickGameChar(x + 1, y - 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "中上";
                                    return;
                                }
                            }
                            if (y + 1 <= 7) //中下
                            {
                                if (GameRect[x + 1, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x + 1, y);
                                    clickGameChar(x + 1, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "中下";
                                    return;
                                }
                            }
                        }

                    }
                    if (x + 1 <= 7)
                    {
                        if (GameRect[x, y] == GameRect[x + 1, y])
                        {
                            if (x - 2 >= 0) //左左
                            {
                                if (GameRect[x - 2, y] == GameRect[x, y])
                                {
                                    clickGameChar(x - 2, y);
                                    clickGameChar(x - 1, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "左左";
                                    return;
                                    //  return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x - 1 >= 0 && y - 1 >= 0) //左上
                            {
                                if (GameRect[x, y] == GameRect[x - 1, y - 1])
                                {
                                    clickGameChar(x - 1, y - 1);
                                    clickGameChar(x - 1, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "左上";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x - 1 >= 0 && y + 1 <= 7) //左下
                            {
                                if (GameRect[x, y] == GameRect[x - 1, y + 1])
                                {
                                    clickGameChar(x - 1, y);
                                    clickGameChar(x - 1, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "左下";
                                    return;

                                    //    return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x + 3 <= 7)  //右右
                            {
                                if (GameRect[x, y] == GameRect[x + 3, y])
                                {
                                    clickGameChar(x + 3, y);
                                    clickGameChar(x + 2, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "右右";
                                    return;

                                    //    return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x + 2 <= 7 && y - 1 >= 0) //右上
                            {
                                if (GameRect[x, y] == GameRect[x + 2, y - 1])
                                {
                                    clickGameChar(x + 2, y - 1);
                                    clickGameChar(x + 2, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "右上";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x + 2 <= 7 && y + 1 <= 7) //右下
                            {
                                if (GameRect[x, y] == GameRect[x + 2, y + 1])
                                {
                                    clickGameChar(x + 2, y + 1);
                                    clickGameChar(x + 2, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "右下";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                        }
                    }
                    if(x-1>=0){
                        if (GameRect[x, y] == GameRect[x - 1, y])
                        {
                            if (x - 3 >= 0)
                            {
                                if (GameRect[x - 3, y] == GameRect[x, y]) //左左
                                {
                                    clickGameChar(x - 3, y);
                                    clickGameChar(x - 2, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "左左";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x - 2 >= 0 && y - 1 >= 0) //左上
                            {
                                if (GameRect[x - 2, y - 1] == GameRect[x, y])
                                {
                                    clickGameChar(x - 2, y - 1);
                                    clickGameChar(x - 2, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "左上";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x - 2 >= 0 && y + 1 <= 7)   //左下
                            {
                                if (GameRect[x - 2, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x - 2, y + 1);
                                    clickGameChar(x - 2, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "左下";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x + 2 <= 7)  //右右
                            {
                                if (GameRect[x + 2, y] == GameRect[x, y])
                                {
                                    clickGameChar(x + 2, y);
                                    clickGameChar(x + 1, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "右右";
                                    return;

                                    //    return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x + 1 <= 7 && y - 1 >= 0)  //右上
                            {
                                if (GameRect[x + 1, y - 1] == GameRect[x, y])
                                {
                                    clickGameChar(x + 1, y - 1);
                                    clickGameChar(x + 1, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "右上";
                                    return;

                                    //    return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (x + 1 <= 7 && y + 1 <= 7)  //右下
                            {
                                if (GameRect[x + 1, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x + 1, y + 1);
                                    clickGameChar(x + 1, y);
                                    this.Text = x.ToString() + "  " + y.ToString() + "右下";
                                    return;

                                    //   return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (y + 2 <= 7)
                    {
                        if (GameRect[x, y] == GameRect[x, y + 2])
                        {
                            if (x - 1 >= 0) //中左
                            {
                                if (GameRect[x - 1, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x - 1, y + 1);
                                    clickGameChar(x, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "中左";
                                    return;
                                }
                            }
                            if (x + 1 <= 7) //中右
                            {
                                if (GameRect[x + 1, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x, y + 1);
                                    clickGameChar(x + 1, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "中右";
                                    return;
                                }
                            }
                        }
                    }
                    if (y + 1 <= 7)
                    {
                        if (GameRect[x, y] == GameRect[x, y + 1])
                        {
                            if (y - 2 >= 0)//上上
                            {
                                if (GameRect[x, y - 2] == GameRect[x, y])
                                {
                                    clickGameChar(x, y - 2);
                                    clickGameChar(x, y - 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "上上";
                                    return;

                                    //  return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y - 1 >= 0 && x - 1 >= 0) //上左
                            {
                                if (GameRect[x - 1, y - 1] == GameRect[x, y])
                                {
                                    clickGameChar(x - 1, y - 1);
                                    clickGameChar(x, y - 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "上左";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y - 1 >= 0 && x + 1 <= 7) //上右
                            {
                                if (GameRect[x + 1, y - 1] == GameRect[x, y])
                                {
                                    clickGameChar(x + 1, y - 1);
                                    clickGameChar(x, y - 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "上右";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y + 3 <= 7)//下下
                            {
                                if (GameRect[x, y] == GameRect[x, y + 3])
                                {
                                    clickGameChar(x, y + 3);
                                    clickGameChar(x, y + 2);
                                    this.Text = x.ToString() + "  " + y.ToString() + "下下";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y + 2 <= 7 && x - 1 >= 0) //下左
                            {
                                if (GameRect[x, y] == GameRect[x - 1, y + 2])
                                {
                                    clickGameChar(x - 1, y + 2);
                                    clickGameChar(x, y + 2);
                                    this.Text = x.ToString() + "  " + y.ToString() + "下左";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y + 2 <= 7 && x + 1 <= 7) //下右边
                            {
                                if (GameRect[x, y] == GameRect[x + 1, y + 2])
                                {
                                    clickGameChar(x + 1, y + 2);
                                    clickGameChar(x, y + 2);
                                    this.Text = x.ToString() + "  " + y.ToString() + "下右";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                        }
                    }
                    if(y-1>=0){
                        if (GameRect[x, y] == GameRect[x, y - 1])
                        {
                            if (y - 3 >= 0) //上上
                            {
                                if (GameRect[x, y - 3] == GameRect[x, y])
                                {
                                    clickGameChar(x, y - 3);
                                    clickGameChar(x, y - 2);
                                    this.Text = x.ToString() + "  " + y.ToString() + "上上";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y - 2 >= 0 && x + 1 <= 7)   //上右
                            {
                                if (GameRect[x + 1, y - 2] == GameRect[x, y])
                                {
                                    clickGameChar(x, y - 2);
                                    clickGameChar(x + 1, y - 2);
                                    this.Text = x.ToString() + "  " + y.ToString() + "上右边";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y - 2 >= 0 && x - 1 >= 0)   //上左
                            {
                                if (GameRect[x - 1, y - 2] == GameRect[x, y])
                                {
                                    clickGameChar(x, y - 2);
                                    clickGameChar(x - 1, y - 2);
                                    this.Text = x.ToString() + "  " + y.ToString() + "上左";
                                    return;

                                    //    return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y + 2 <= 7) //下下
                            {
                                if (GameRect[x, y + 2] == GameRect[x, y])
                                {
                                    clickGameChar(x, y + 2);
                                    clickGameChar(x, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "下下";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y + 1 <= 7 && x - 1 >= 0)  //下左
                            {
                                if (GameRect[x - 1, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x, y + 1);
                                    clickGameChar(x - 1, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "下左";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                            if (y + 1 <= 7 && x + 1 <= 7)   //下右
                            {
                                if (GameRect[x + 1, y + 1] == GameRect[x, y])
                                {
                                    clickGameChar(x, y + 1);
                                    clickGameChar(x + 1, y + 1);
                                    this.Text = x.ToString() + "  " + y.ToString() + "下右";
                                    return;

                                    //return clearChar(gh.ReadInt(SCOREADD));
                                }
                            }
                        }
                    }
                }
            }

              //  return false;
            
        }
        private void clickGameChar(int x,int y)
        {
            BringWindowToTop(Hwnd);
            GetWindowRect(Hwnd, ref Rect);
            int xpot =Rect.Left+ 288 + (x * 47);
            int ypot =Rect.Top+ 115 + (y * 47);
            SetCursorPos(xpot, ypot);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            System.Threading.Thread.Sleep(50);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "点击延迟(" + trackBar1.Value.ToString() + ")";
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "kyodaiRPG")
                {
                    
                    if (p != gh.ReadProcess)
                    {
                        Hwnd = FindWindow(null, "QQ游戏 - 连连看角色版");
                        ProcessIsStart = true;
                        gh.ReadProcess = p;
                        label1.ForeColor = Color.Black;
                        label1.Text = "程序已启动";
                        break;
                    }
                }
                else {
                    ProcessIsStart = true;
                    label1.ForeColor = Color.Red;
                    label1.Text = "程序未启动";
                }
            }
            if (gh.ReadProcess != null && checkBox1.Checked == true && IsStart == false && ProcessIsStart == true)
            {

               // IsStart = true;
                button1.PerformClick();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = "点击延迟(" + trackBar1.Value.ToString() + ")";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                gh.CloseHandle();
            }
            catch { }
        }
    }
}
