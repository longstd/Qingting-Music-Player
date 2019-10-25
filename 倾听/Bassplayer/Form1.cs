using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Un4seen.Bass;


namespace Bassplayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool p = false;
        private void Form1_Load(object sender, EventArgs e)//加载窗口时自动设置字体
        {
            FontFamily songti = new FontFamily("宋体");
            this.Font = new Font(songti, 9);
            radioButton1.Font = new Font(songti, 11);
            radioButton2.Font = new Font(songti, 11);
            button1.Font = new Font(songti, 10);
            button2.Font = new Font(songti, 10);
            listBox1.Font = new Font(songti, 12);
            groupBox1.Font = new Font(songti, 11);
            // this.TransparencyKey = Color.Green;
            // this.BackColor = Color.Green;
            this.Opacity = 0.65;
           // listBox1.BackColor = Color.Green;
           // listBox1.BackColor = TransparencyKey;
            try
            {
                trackBar1.Value = 20;//初始化赋予音量                
                string flac = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "bassflac.dll";//从软件运行目录加载音频库
                string ape = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase+ "bass_ape.dll";
                string tta = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "bass_tta.dll";
                string wv = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "basswv.dll";
                int handle = Bass.BASS_PluginLoad(flac);
                int handle2 = Bass.BASS_PluginLoad(ape);
                int handle3 = Bass.BASS_PluginLoad(tta);
                int handle4 = Bass.BASS_PluginLoad(wv);
                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_CPSPEAKERS, this.Handle))//如果bass音频库加载出错
                {
                    MessageBox.Show("bass初始化出错" + Bass.BASS_ErrorGetCode().ToString());
                }
                stream = Bass.BASS_StreamCreateFile(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "启动音频.wav", 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);//软件启动时播放选中的歌曲
                Bass.BASS_ChannelPlay(stream, true);
            }
            catch { }

        }
        int stream;                  
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//关闭软件后释放资源
        {
            try
            {
                Bass.BASS_ChannelStop(stream);
                Bass.BASS_StreamFree(stream);
                Bass.BASS_Stop();
                Bass.BASS_Free();
            }
            catch { }
        }
        
        private void 打开音乐文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

        }
        List<string> listpath = new List<string>();//路径的集合
        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)//打开音乐文件对话框的编写
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"F:\";
            ofd.Filter = "flac文件|*.flac|ape文件|*.ape|mp3文件|*.mp3|所有文件|*.*";
            ofd.Title = "您好，请选择你的音乐文件";
            ofd.Multiselect = true;
            ofd.ShowDialog();//打开文件对话框
            string[] path = ofd.FileNames;
            for (int i = 0; i < path.Length; i++)
            {
                listpath.Add(path[i]);
                listBox1.Items.Add(Path.GetFileName(path[i]));//listbox获得文件的路径
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)//listbox双击歌曲播放
        {
            try { 
            string songname = listpath[listBox1.SelectedIndex];//获得受点击歌曲的路径
            if (listBox1.Items.Count<= 0)
            {
                MessageBox.Show("请选择要播放的音乐文件");//在还没选择选择歌曲时双击listbox时的代码
                return;
            }
            Bass.BASS_ChannelStop(stream);   //停止前一首的歌曲      
            stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);//播放选中的歌曲
                Bass.BASS_ChannelPlay(stream, true);
                label1.Text = "  " + songname;
                p = true;
                button1.Enabled = true;
                暂停ToolStripMenuItem.Text = "暂停";
                button1.Text = "暂停";
            }

            catch { }



        }


        private void 从列表中删除ToolStripMenuItem_Click(object sender, EventArgs e)//从列表中移除歌曲
        {
            int count = listBox1.SelectedItems.Count;//首先要获得要删除歌曲的数量
            for (int i = 0; i < count; i++)
            {
                listpath.RemoveAt(listBox1.SelectedIndex);//先删集合
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);//再删列表
            }


        }

        private void timer1_Tick(object sender, EventArgs e)//自动播放下一曲
        {
            
           double c= Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));//获取歌曲总长度
            double a = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream));//获取当前正播放歌曲的进度
            
            label1.Text = label1.Text.Substring(1) + label1.Text.Substring(0, 1);
          
       if (c==a&&c>=0)//判断歌曲是否播放完了
       {
           if (radioButton1.Checked)//判断歌曲的播放模式是否为“顺序播放”
           {
               try
               {
                   int index = listBox1.SelectedIndex;//获得当前选中的索引
                   listBox1.SelectedIndices.Clear();//清空当前所有选中的索引

                   index++;
                   if (index == listBox1.Items.Count)//如果歌曲为最后一曲
                   { index = 0; }//索引从头开始
                   listBox1.SelectedIndex = index;
                   Bass.BASS_ChannelStop(stream);//停止上一曲
                   string songname = listpath[listBox1.SelectedIndex];//获得当前的歌曲路径
                   stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);//播放当前选中的歌曲
                   Bass.BASS_ChannelPlay(stream, true);
                   label1.Text = "   " + songname;
                   button1.Enabled = true;
                   暂停ToolStripMenuItem.Text = "暂停";
                   button1.Text = "暂停";
                   p = true;
                  
               }
               catch { }
           }
           else if(radioButton2.Checked)//当播放模式为随机播放
           {

               try
               {
                   Bass.BASS_ChannelStop(stream);
                   listBox1.SelectedIndices.Clear();//清空当前所有选中的索引
                   Random next = new Random();
                   int nextsong = next.Next(1, listBox1.Items.Count - 1);
                   listBox1.SelectedIndex = nextsong;
                   string songname = listpath[listBox1.SelectedIndex];
                   stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
                   Bass.BASS_ChannelPlay(stream, true);
                   label1.Text = "   " + songname;
                   暂停ToolStripMenuItem.Text = "暂停";
                   button1.Text = "暂停";
                   p = true;
               }
               catch { }
           }
                else  //单曲循环播放模式
                {
                    try
                    {
                        int index = listBox1.SelectedIndex;//获得当前选中的索引
                        listBox1.SelectedIndices.Clear();//清空当前所有选中的索引

                        // index++;
                        // if (index == listBox1.Items.Count)
                        // { index = 0; }
                        listBox1.SelectedIndex = index;
                        Bass.BASS_ChannelStop(stream);
                        string songname = listpath[listBox1.SelectedIndex];
                        stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
                        Bass.BASS_ChannelPlay(stream, true);
                        label1.Text = "   " + songname;
                        button1.Enabled = true;
                        暂停ToolStripMenuItem.Text = "暂停";
                        button1.Text = "暂停";
                        p = true;
                    }
                    catch { }
                }

            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            float volume = Bass.BASS_GetVolume();//获得当前音量
            volume =(float)trackBar1.Value/100;//将获得的音量除以100的到音频庫所支持的音量值
            Bass.BASS_SetVolume(volume);//将音量输出
            volume = volume * 100;//乘上100后让label显示
            label4.Text = "音量："+volume.ToString();
            double c = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)) - Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream));  //歌曲总长度减去已经播放歌曲长度           
            double min = c / 60; int min1 = (int)min;//转为分和秒让label显示
            double sec = c % 60; int sec1 = (int)sec;
            label2.Text = min1.ToString()+":"+sec1.ToString();
         
         
        }

        private void button1_Click(object sender, EventArgs e)  //播放按钮
        {
            if(p==true)//判断是否在播放
            {
                Bass.BASS_ChannelPause(stream);
                button1.Text = "继续";
                暂停ToolStripMenuItem.Text = "继续";
                p = false;
            }
            else
            {
                Bass.BASS_ChannelPlay(stream,false);
                p =true;
                暂停ToolStripMenuItem.Text = "暂停";
                button1.Text = "暂停";
            }
        }

        private void button2_Click(object sender, EventArgs e)//下一曲
        {
            if (radioButton2.Checked)//判断播放模式,如果是随机播放，则执行以下代码
            {
                try
                {
                    Bass.BASS_ChannelStop(stream);
                    listBox1.SelectedIndices.Clear();//清空当前所有选中的索引
                    Random next = new Random();
                    int nextsong = next.Next(1, listBox1.Items.Count - 1);
                    listBox1.SelectedIndex = nextsong;
                    string songname = listpath[listBox1.SelectedIndex];
                    stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
                    Bass.BASS_ChannelPlay(stream, true);
                    label1.Text = "   " + songname;
                    暂停ToolStripMenuItem.Text = "暂停";
                    button1.Text = "暂停";
                    p = true;
                }
                catch { }
            }
            else if(radioButton1.Checked )//如果是按列表顺序模式
            {
                try
                {
                    int index = listBox1.SelectedIndex;//获得当前选中的索引
                    listBox1.SelectedIndices.Clear();//清空当前所有选中的索引

                    index++;
                    if (index == listBox1.Items.Count)
                    { index = 0; }
                    listBox1.SelectedIndex = index;
                    Bass.BASS_ChannelStop(stream);
                    string songname = listpath[listBox1.SelectedIndex];
                    stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
                    Bass.BASS_ChannelPlay(stream, true);
                    label1.Text = "   " + songname;
                    button1.Enabled = true;
                    暂停ToolStripMenuItem.Text = "暂停";
                    button1.Text = "暂停";
                    p = true;
                }
                catch { }          
            }
            else  //单曲循环播放模式
            {
                try {
                    int index = listBox1.SelectedIndex;//获得当前选中的索引
                    listBox1.SelectedIndices.Clear();//清空当前所有选中的索引

                    // index++;
                    // if (index == listBox1.Items.Count)
                    // { index = 0; }
                    listBox1.SelectedIndex = index;
                    Bass.BASS_ChannelStop(stream);
                    string songname = listpath[listBox1.SelectedIndex];
                    stream = Bass.BASS_StreamCreateFile(songname, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT);
                    Bass.BASS_ChannelPlay(stream, true);
                    label1.Text = "   " + songname;
                    button1.Enabled = true;
                    暂停ToolStripMenuItem.Text = "暂停";
                    button1.Text = "暂停";
                    p = true;
                }
                catch { }
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)//右键退出
        {
            this.Close();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)//窗体最小化
        {
            if(WindowState==FormWindowState.Minimized)
            {
               
                 //this.Visible = false;
                //this.ShowInTaskbar = true;
            }
            
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)//双击恢复窗体
        {

        }

        private void 暂停ToolStripMenuItem_Click(object sender, EventArgs e)//最小化时控制播放暂停代码
        {
            if (p == true)
            {
                Bass.BASS_ChannelPause(stream);
                button1.Text = "继续";
                暂停ToolStripMenuItem.Text = "继续";
                p = false;
            }
            else
            {
                Bass.BASS_ChannelPlay(stream, false);
                p = true;
                暂停ToolStripMenuItem.Text = "暂停";
                button1.Text = "暂停";
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
        
  /*  void IsExistlrc(string songPath)
        {
            songPath += ".lrc";
            //songPath.Replace(".mp3", ".lrc");
            if (File.Exists(songPath))
        {
            string[] lrcText = File.ReadAllLines(songPath, Encoding.Default);
             Formatlrc(lrcText);
        }
     else
        {
            label5.Text = "<-----歌词未找到----->";

        }
        }

        List<double> listTime=new List<double>();
        List<string> listLrcText=new List<string>();
    void Formatlrc(string[] lrcText)
    {
        for (int i = 0; i <lrcText.Length; i++)
        {
    
            string[] lrcTemp = lrcText[i].Split(new char[] {'[',']'}, StringSplitOptions.RemoveEmptyEntries);
            string[] lrcNewTemp = lrcTemp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            double time = double.Parse(lrcNewTemp[0])*60+double.Parse(lrcNewTemp[1]);
               listTime.Add(time);
              listLrcText.Add(lrcTemp[1]);
           
        }


    }
    */
    private void timer3_Tick(object sender, EventArgs e)
    {
       
        
    }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_Click(object sender, EventArgs e)//窗口恢复正常代码
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.TopMost = true;
                this.Visible = true;
                WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Activate();
            }
            }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)//打开“关于文本”
        {   
            System.Diagnostics.Process.Start("notepad.exe","关于");
        }

        private void 透明ToolStripMenuItem_Click(object sender, EventArgs e)//设置透明主题
        {
            this.TransparencyKey = Color.DeepSkyBlue;
            this.BackColor = Color.DeepSkyBlue;
            listBox1.BackColor = TransparencyKey;
        }

        private void 默认ToolStripMenuItem_Click(object sender, EventArgs e)//设置默认主题
        {
            this.BackColor = Color.White;
            listBox1.BackColor = Color.White;
            this.Opacity = 0.65;
        }

        private void 楷体ToolStripMenuItem_Click(object sender, EventArgs e)//设置字体为楷体代码
        {
            FontFamily kaiti = new FontFamily("楷体");
            this.Font = new Font(kaiti,9);
            radioButton1.Font = new Font(kaiti,11);
            radioButton2.Font = new Font(kaiti,11);
            button1.Font = new Font(kaiti, 10);
            button2.Font = new Font(kaiti, 10);
            // menuStrip1.Font = new Font(kaiti, 9);
            listBox1.Font = new Font(kaiti,12);
            groupBox1.Font = new Font(kaiti, 11);
        }

        private void 默认ToolStripMenuItem1_Click(object sender, EventArgs e)//设置字体为默认宋体代码
        {
            FontFamily songti = new FontFamily("宋体");
            this.Font = new Font(songti, 9);
            radioButton1.Font = new Font(songti, 11);
            radioButton2.Font = new Font(songti, 11);
            button1.Font = new Font(songti,10);
            button2.Font = new Font(songti,10);
            listBox1.Font = new Font(songti,12);
            groupBox1.Font = new Font(songti, 11);
        }

        private void monotypeCorsivaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //2 FontFamily weiruan = new FontFamily("微软雅黑");
            // this.Font = new Font(weiruan, 9);
            // radioButton1.Font = new Font(weiruan, 10);
            // radioButton2.Font = new Font(weiruan, 10);
            // button1.Font = new Font(weiruan, 10);
            // button2.Font = new Font(weiruan,10);
            //groupBox1.Font = new Font(weiruan, 10);
            try
            {
                FontFamily MC = new FontFamily("CommercialScript BT");
                listBox1.Font = new Font(MC, 11);
            }

            catch { }
        }
    }
}
