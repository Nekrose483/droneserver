// This program is fully open source, although please give us some sort of credit :)
// Your free to re-write or change the strings of the server. But leave the copyrights in the code.
// Copyright of xsDevelopment & sniperX
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
//using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ChatServer
{
    /*public partial class Form1 : Form
	{
        private delegate void UpdateStatusCallback(string strMessage);
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }
		
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
        
        private void btnListen_Click(object sender, EventArgs e)
        {
            IPAddress ipAddr = IPAddress.Parse(txtIp.Text);
            ChatServer mainServer = new ChatServer(ipAddr);
            ChatServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);
            mainServer.StartListening();
            btnListen.Enabled = false;
            txtIp.Enabled = false;
            Name1.Start();
            MessageBox.Show("Note, Default Administrative Username and password is. Username: Admin, Password: 123987", "Default Admin Username/Pass");
        }
        public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
        }
        private void UpdateStatus(string strMessage)
        {
            txtLog.AppendText(strMessage + "\r\n");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Chat Server";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChatServer.SendAdminMessage(textBox1.Text);
            textBox1.Clear();
            textBox1.Text = "Message here";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminTools.tempDelAdmin(textBox2.Text);
            textBox2.Clear();
            textBox2.Text = "Admin Nick Here";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdminTools.muteUser(textBox3.Text, "Server");
            textBox3.Clear();textBox3.Text = "Nick here";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AdminTools.unMuteUser(textBox4.Text, "Server");
            textBox4.Clear();textBox4.Text = "Nick here";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AdminTools.disconnectUser(textBox5.Text,null);
            textBox5.Clear();textBox5.Text = "Nick here";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AdminTools.mimicUser(textBox6.Text, textBox7.Text);
            textBox6.Clear();textBox7.Clear();textBox6.Text = "Nick";textBox7.Text = "Message";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AdminTools.addTempAdmin(textBox8.Text, textBox9.Text);
            textBox8.Clear();textBox9.Clear();textBox8.Text = ""; textBox9.Text = "";
        }

        private void Name1_Tick(object sender, EventArgs e)
        {
            this.Text = "Running...";
            Name2.Start();
            Name1.Stop();
        }

        private void Name2_Tick(object sender, EventArgs e)
        {
            this.Text = "Services Hosted By: " + txtIp.Text;
            Name3.Start();
            Name2.Stop();
        }

        private void Name3_Tick(object sender, EventArgs e)
        {
            this.Text = "Running..." + txtIp.Text;
            Name1.Start();
            Name3.Stop();
        }

    }
*/
}
