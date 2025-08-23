using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using ZedGraph;
using System.Management;

namespace DC
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            SerialPort_Initialize();
            ZedGraph_Initialize();
            check = true;
            rbtVelocity.Checked = true;
            txtPosition.Enabled = false;
            txtVelocity.Enabled = true;
            
            //setup timergraph
            TimerGraph.Enabled = false;
            TimerGraph.Tick += new EventHandler(TimerGraph_Tick);
            TimerGraph.Interval = 100;
            TimerGraph.Stop();
            //setup timerport
            TimerPort.Enabled = true;
            TimerPort.Tick += new EventHandler(TimerPort_Tick);
            TimerPort.Interval = 100;
            TimerPort.Start();
        }
   
        private void SerialPort_Initialize()
        {
            //Baud Rate
            cbBaudRate.Items.Add(19200);
            cbBaudRate.Items.Add(38400);
            cbBaudRate.Items.Add(57600);
            cbBaudRate.Items.Add(115200);
            cbBaudRate.Items.Add(128000);
            cbBaudRate.Items.Add(256000);
            cbBaudRate.Items.ToString();
            //get first item print in text
            cbBaudRate.Text = cbBaudRate.Items[3].ToString();
        }

        int SumPort = 0;
        private void TimerPort_Tick(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            if (SumPort != ports.Length)
            {
                SumPort = ports.Length;
                cbSeclectCom.Items.Clear();
                foreach (COMPortInfo PortCOMs in COMPortInfo.GetCOMPortsInfo())
                {
                    cbSeclectCom.Items.Add(string.Format("{0} – {1}", PortCOMs.Name, PortCOMs.Description));
                    cbComName.Items.Add(PortCOMs.Name);
                }
                cbSeclectCom.SelectedIndex = 0;
                cbComName.SelectedIndex = 0;
            }
        }
        /* khoi tao do thi */
        private void ZedGraph_Initialize()
        {
            GraphPane PosPane = GraphPos.GraphPane;
            GraphPane VelPane = GraphVel.GraphPane;

            PosPane.Title.FontSpec.Size = 12;
            PosPane.XAxis.Title.FontSpec.Size = 12;
            PosPane.YAxis.Title.FontSpec.Size = 12;
            PosPane.Title.Text = "Position Graph";
            PosPane.XAxis.Title.Text = "Time (s)";
            PosPane.YAxis.Title.Text = "Position (mm)";
            PosPane.XAxis.MajorGrid.IsVisible = true;
            PosPane.YAxis.MajorGrid.IsVisible = true;
            PosPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(139, 195, 74), 45.0f);
            PosPane.Fill = new Fill(Color.White, Color.White, 45.0f);

            VelPane.Title.FontSpec.Size = 12;
            VelPane.XAxis.Title.FontSpec.Size = 12;
            VelPane.Title.Text = "Velocity Graph";
            VelPane.XAxis.Title.Text = "Time (s)";
            VelPane.YAxis.Title.Text = "Velocity (rad/s)";
            VelPane.XAxis.MajorGrid.IsVisible = true;
            VelPane.YAxis.MajorGrid.IsVisible = true;
            VelPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(139, 195, 74), 45.0f);
            VelPane.Fill = new Fill(Color.White, Color.White, 45.0f);
            
            RollingPointPairList list1 = new RollingPointPairList(60000);
            RollingPointPairList list2 = new RollingPointPairList(60000);

            RollingPointPairList list3 = new RollingPointPairList(60000);
            RollingPointPairList list4 = new RollingPointPairList(60000);

            LineItem curve1 = PosPane.AddCurve("Set Point", list1, Color.Red, SymbolType.None);
            LineItem curve2 = PosPane.AddCurve("Position", list2, Color.Blue, SymbolType.None);

            LineItem curve3 = VelPane.AddCurve("Set Point", list3, Color.Red, SymbolType.None);
            LineItem curve4 = VelPane.AddCurve("Velocity", list4, Color.Blue, SymbolType.None);

            // Chỉnh độ dày của các đường vẽ
            curve1.Line.Width = 1.5f; // Độ dày của đường "Set Point" trên Position Graph
            curve2.Line.Width = 1.0f; // Độ dày của đường "Position" trên Position Graph
            curve3.Line.Width = 1.5f; // Độ dày của đường "Set Point" trên Velocity Graph
            curve4.Line.Width = 1.0f; // Độ dày của đường "Velocity" trên Velocity Graph

            //  TimerGraph.Interval = 50;
            //   PosPane.XAxis.Scale.Min = 0;
            PosPane.XAxis.Scale.Max = 40;
            PosPane.XAxis.Scale.MinorStep = 1;
            PosPane.XAxis.Scale.MajorStep = 5;
            PosPane.YAxis.Scale.MaxAuto = false;
            PosPane.YAxis.Scale.MinAuto = false;
            PosPane.YAxis.Scale.Min = -0.5;
            PosPane.YAxis.Scale.Max = 300;
            GraphPos.AxisChange();




            VelPane.XAxis.Scale.Min = 0;
            VelPane.XAxis.Scale.Max = 40;
            VelPane.XAxis.Scale.MinorStep = 1;
            VelPane.XAxis.Scale.MajorStep = 5;
            VelPane.YAxis.Scale.MinAuto = false; // Tắt tự động điều chỉnh Min
            VelPane.YAxis.Scale.MaxAuto = false; // Tắt tự động điều chỉnh Max
            VelPane.YAxis.Scale.Min = -0.2;       // Giá trị Min cố định
            VelPane.YAxis.Scale.Max = 140;        // Giá trị Max cố định
            GraphVel.AxisChange();
        }

        /* -------------------------------- Vẽ đồ thị---------------------------------- */
        int tickStart = 0;
        double pos, vel, set_pos, set_vel, mv;
        bool scroll = true, check = true;

        public void ZedGraph_Draw()
        {
            if (GraphPos.GraphPane.CurveList.Count <= 0) return;
            if (GraphVel.GraphPane.CurveList.Count <= 0) return;

            LineItem curve1 = GraphPos.GraphPane.CurveList[0] as LineItem;
            LineItem curve2 = GraphPos.GraphPane.CurveList[1] as LineItem;
            LineItem curve3 = GraphVel.GraphPane.CurveList[0] as LineItem;
            LineItem curve4 = GraphVel.GraphPane.CurveList[1] as LineItem;

            if (curve1 == null) return;
            if (curve2 == null) return;
            if (curve3 == null) return;
            if (curve4 == null) return;

            IPointListEdit list1 = curve1.Points as IPointListEdit;
            IPointListEdit list2 = curve2.Points as IPointListEdit;
            IPointListEdit list3 = curve3.Points as IPointListEdit;
            IPointListEdit list4 = curve4.Points as IPointListEdit;

            if (list1 == null) return;
            if (list2 == null) return;
            if (list3 == null) return;
            if (list4 == null) return;

            double time = (Environment.TickCount - tickStart) / 1000.0;

            if (check)
            {
                list1.Add(time, mv);
                list3.Add(time, set_vel);
                list4.Add(time, vel);
            }
            else
            {
                list1.Add(time, set_pos);
                list2.Add(time, pos);
                list3.Add(time, set_vel);
                list4.Add(time, vel);
            }


            Scale PosScale = GraphPos.GraphPane.XAxis.Scale;
            Scale VelScale = GraphVel.GraphPane.XAxis.Scale;

            if (time > PosScale.Max - PosScale.MajorStep)
            {
                if (scroll)
                {
                    PosScale.Max = time + PosScale.MajorStep;
                    PosScale.Min = PosScale.Max - 30.0;
                    VelScale.Max = time + VelScale.MajorStep;
                    VelScale.Min = VelScale.Max - 30.0;
                }
                else
                {
                    PosScale.Max = time + PosScale.MajorStep;
                    PosScale.Min = 0;
                    VelScale.Max = time + PosScale.MajorStep;
                    VelScale.Min = 0;
                }
            }

            GraphPos.AxisChange();
            GraphPos.Invalidate();

            GraphVel.AxisChange();
            GraphVel.Invalidate();
        }   

        private void btConnect_Click(object sender, EventArgs e)
        {
            try
            {
                SerialPort.PortName = Convert.ToString(cbComName.Text);
                SerialPort.BaudRate = Convert.ToInt32(cbBaudRate.Text);
                SerialPort.DataBits = 8;
                SerialPort.StopBits = StopBits.One;
                SerialPort.Handshake = Handshake.None;
                SerialPort.Parity = Parity.None;
                SerialPort.Open();
                gbConnect.Text = cbSeclectCom.SelectedItem.ToString();
                TimerPort.Enabled = false;
                //Run BackgroundWorker
                if (!bkgdWorker.IsBusy) { bkgdWorker.RunWorkerAsync(); }

                System.Threading.Thread.Sleep(1000);
                tickStart = Environment.TickCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            DialogResult msg;
            msg = MessageBox.Show("Do you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (msg == DialogResult.Yes)
            {
                TimerGraph.Stop();
                SerialPort.Close();
                //this.Close();
                Application.Exit();
            }

        }

        private void btScale_Click(object sender, EventArgs e)
        {
            if (btScale.Text == "Compact")
            {
                btScale.Text = "Sroll";
                scroll = true;
            }
            else
            {
                btScale.Text = "Compact";
                scroll = false;
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            try
            {
                //SerialPort.Write(" ");
                //send_data = false;
                //SerialPort.Write("b");
                //if (check == true)  // dieu khien toc do
                //{
                    //set_vel = Convert.ToDouble(txtVelocity.Text);
                    //set_pos = 0;
                    //SerialPort.Write(txtVelocity.Text + "s");
                    //SerialPort.Write(txtKpSet.Text + "p");
                    //SerialPort.Write(txtKiSet.Text + "i");
                    //SerialPort.Write(txtKdSet.Text + "d");
                //}
                //else   // dieu khien vi tri
                //{
                SerialPort.Write("r");
                TimerGraph.Start();
                
                //set_pos = Convert.ToDouble(txtPosition.Text);
                //set_vel = 0;
                //SerialPort.Write(txtPosition.Text );
                //SerialPort.Write("s");
                //SerialPort.Write(txtAcc.Text + "m");
                //SerialPort.Write(txtVelocity.Text + "s");
                //SerialPort.Write(txtKpSet.Text + "p");
                //SerialPort.Write(txtKiSet.Text + "i");
                //SerialPort.Write(txtKdSet.Text + "d");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                //send_data = true;
                //SerialPort.Write("z");
                //SerialPort.Write("e");// cho send data
                //SerialPort.Write("a");//run
                // TimerGraph.Enabled = true;
                // tickStart = Environment.TickCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        string InputData = String.Empty;
        private void bkgdWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            while (!bw.CancellationPending)                // backgroundworker runs continously
            {
                if (SerialPort.IsOpen)
                {
                    InputData = SerialPort.ReadLine();
                    if (InputData != String.Empty)
                    {
                        this.BeginInvoke(new SetTextCallback(SetText), new object[] { InputData });
                    }
                }
            }
        }
        
        delegate void SetTextCallback(string text);
        private void SetText(string data)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            try
            {
                data = data.Trim();
                string strCheck = data.Substring(0,1);
                string strData = data.Substring(1);
                
                switch (strCheck)
                {
                    case "V":
                        txtVel.Text = strData;
                        vel = Convert.ToDouble(strData,provider);
                        break;
                    case "P":
                        txtPos.Text = strData;
                        pos = Convert.ToDouble(strData,provider);
                        errPos = set_pos - pos;
                        /****************************************************/
                        //txtError.Text = Convert.ToString(errPos,provider);
                        break;
                    case "U":
                        txtPos.Text = strData;
                        mv = Convert.ToDouble(strData,provider);
                        break;
                    case "z":
                       // txtKbSet.Text = strData;
                        break;    
                }
                ///if(send_data == true) SerialPort.Write("a");
                //else SerialPort.Write("b");
            }
            catch (Exception ex)
            {
                //txtGetData.Text = ex.Message;
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbtPosition_CheckedChanged(object sender, EventArgs e)
        {
            check = false;
            txtPosition.Enabled = true;
            txtKpSet.Enabled = false;
            txtKiSet.Enabled = false;
            txtKdSet.Enabled = false;
            //txtVelocity.ReadOnly = true;
        }

        private void rbtVelocity_CheckedChanged(object sender, EventArgs e)
        {
            check = true;
            txtPosition.Enabled = false;
            txtKpSet.Enabled = true;
            txtKiSet.Enabled = true;
            txtKdSet.Enabled = true;
            //txtAcc.ReadOnly = true;
            //txtVelocity.ReadOnly = false;
        }

        private void btSetPoint_Click(object sender, EventArgs e)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            if (!check)
            {
                set_pos = Convert.ToDouble(txtPosition.Text,provider);
                set_vel = Convert.ToDouble(txtVelocity.Text,provider);
                SerialPort.Write("a");                          //position mode
                SerialPort.Write(txtPosition.Text + "s");
                SerialPort.Write(txtVelocity.Text + "v");
            }
            else
            {
                set_vel = Convert.ToDouble(txtVelocity.Text,provider);
                SerialPort.Write("b");                          //velocity mode
                SerialPort.Write(txtVelocity.Text + "v");
                SerialPort.Write(txtKpSet.Text + "p");
                SerialPort.Write(txtKiSet.Text + "i");
                SerialPort.Write(txtKdSet.Text + "d");
            }
            //update gui
            ZedGraph_Draw();
            TimerGraph.Start();

            //send_data = true;
            //SerialPort.Write("z");
            //SerialPort.Write("e");// cho send data

            /*           
                       try
                       {
                         //  send_data = false;
                           SerialPort.Write("b");
                           if (check == true)
                               SerialPort.Write("k");
                           else
                               SerialPort.Write("l");

                           SerialPort.Write("a");
                       }

                       catch (Exception ex)
                       {
                           MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       }
             */
        }

        private void btSetPID_Click(object sender, EventArgs e)
        {
            
        }

        private void btPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (btPause.Text == "Pause")
                {
                    btPause.Text = "Resume";
                    SerialPort.Write("f");//tam dung
                }
                else
                {
                    btPause.Text = "Pause";
                    SerialPort.Write("g");// tiep tuc
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btReverse_Click(object sender, EventArgs e)
        {
            try
            {
              //  SerialPort.Write("b");
              //  if (btReverse.Text == "Reverse")
              //  {
              //      btReverse.Text = "Forward";
              //      SerialPort.Write("h");//chay nghich
              //  }
              //  else
              //  {
              //      btReverse.Text = "Reverse";
              //      SerialPort.Write("j");// chay thuan
              //  }
              //  SerialPort.Write("a");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
  
        private void btReSet_Click(object sender, EventArgs e)
        {
            //send_data = false;
           //SerialPort.Write("b");
           // SerialPort.Write("r");
            GraphPos.GraphPane.CurveList.Clear();
            GraphVel.GraphPane.CurveList.Clear();
            GraphPos.GraphPane.GraphObjList.Clear();
            GraphVel.GraphPane.GraphObjList.Clear();
           ZedGraph_Initialize();
           TimerGraph.Enabled = false;
        }

        private void btClearPID_Click(object sender, EventArgs e)
        {

        }

        private void btStop_Click(object sender, EventArgs e)
        {
            try
            {
                SerialPort.Write("e");
                TimerGraph.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void grbSetVel_Enter(object sender, EventArgs e)
        {

        }

       

        private void txtPos_TextChanged(object sender, EventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void txtKpSet_TextChanged(object sender, EventArgs e)
        {

        }
        double errPos = 0;
        private void TimerGraph_Tick(object sender, EventArgs e)
        {
            ZedGraph_Draw();

        }

        private void cbSeclectCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbComName.SelectedIndex = cbSeclectCom.SelectedIndex;
        }

        private void btDisconnect_Click(object sender, EventArgs e)
        {
            gbConnect.Text = "Disconnect";
            TimerGraph.Stop();                           //Stop timer(s), whatever it takes
            TimerPort.Enabled = true;                    // Start timer to scan port   
            
            bkgdWorker.CancelAsync();
            System.Threading.Thread.Sleep(500);         //Wait bkworker to finish
            SerialPort.Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult msg;
            msg = MessageBox.Show("Do you want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (msg == DialogResult.Yes)
            {
                SerialPort.Close();
                TimerGraph.Stop();
                e.Cancel = false;
            }
            else e.Cancel = true;
        }
    }
}
