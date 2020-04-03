using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HRUpnp
{
    public partial class frmHRUpnp : Form
    {
        BackgroundWorker bgw = new BackgroundWorker();
        public frmHRUpnp()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
          
                }//end button




        private void startHover()
        {
            this.Hide();
            
            string strWorkingDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
   
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = strWorkingDir;
            startInfo.RedirectStandardError = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "HRace.exe";       
            process.StartInfo = startInfo;


            process.Start();
            //  string output = process.StandardOutput.ReadToEnd();
            // MessageBox.Show(output);
            process.WaitForExit();
          
            
            //Little bit of cleaning up after they quit the game
            execCmd("-d 9350 TCP");

            execCmd("-d 9351 TCP");
  
            execCmd("-d 9351 UDP");
            Application.Exit();
        }




        private void execCmd(string strArgs)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
           startInfo.WindowStyle= ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "miniupnp\\upnpc-shared.exe";
            startInfo.Arguments = strArgs;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
          

            process.Start();
          //  string output = process.StandardOutput.ReadToEnd();
           // MessageBox.Show(output);
            process.WaitForExit();
        }

        private void frmHRUpnp_Load(object sender, EventArgs e)
        {
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();

        }//end load void


        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {

            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            int intProgress = 0;
            //Delete existing rules
            execCmd("-d 9350 TCP");
            bgw.ReportProgress(intProgress+=17);
            execCmd("-d 9351 TCP");
            bgw.ReportProgress(intProgress += 17);
            execCmd("-d 9351 UDP");
            bgw.ReportProgress(intProgress += 17);
            //Create new rules for this ip
            execCmd("-a " + localIP + " 9350 9350 TCP");
            bgw.ReportProgress(intProgress += 17);
            execCmd("-a " + localIP + " 9351 9351 TCP");
            bgw.ReportProgress(intProgress += 17);
            execCmd("-a " + localIP + " 9351 9351 UDP");
           
            bgw.ReportProgress(intProgress+=15);
        }


        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgProgress.Value = e.ProgressPercentage;
  
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //do the code when bgv completes its work
            startHover();
        }

      
    }//end class
}//end namespace
