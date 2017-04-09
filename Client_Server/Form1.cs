using CryptoWizard.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_Server
{
  public partial class Form1 : Form
  {
    private ElectronicDigitalSignature EDS = new ElectronicDigitalSignature();
    private TcpClient client;
    private StreamReader STR;
    private StreamWriter STW;
    private string receive;
    private string text_to_send;
    private int private_key = 12;

    public Form1()
    {
      InitializeComponent();

      var localIP = Dns.GetHostAddresses(Dns.GetHostName()); //get my own IP
      foreach(IPAddress address in localIP)
      {
        if(address.AddressFamily == AddressFamily.InterNetwork)
        {
          textBox3.Text = address.ToString();
        }
      }
    }

    private void button2_Click(object sender, EventArgs e) //Start Server
    {
      var listener = new TcpListener(IPAddress.Any, int.Parse(textBox4.Text));
      listener.Start();
      client = listener.AcceptTcpClient();
      STR = new StreamReader(client.GetStream());
      STW = new StreamWriter(client.GetStream());
      STW.AutoFlush = true;

      backgroundWorker1.RunWorkerAsync(); //Start receiving data to background
      backgroundWorker2.WorkerSupportsCancellation = true; //Ability to cancel this thread
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //receive data
    {
      while(client.Connected)
      {
        try
        {
          receive = STR.ReadLine();
          var result = receive.Split(',');
          var sr = new StreamReader(result[2]);
          var data = sr.ReadToEnd();
          sr.Close();
          var r = receive.Split(',').Select(item => int.Parse(item));
          var res = EDS.CheckEDS(416, 55, r.ElementAt(0), r.ElementAt(1), Encoding.UTF8.GetBytes(data), private_key, 728, -1, 751);
          if (res)
          {
            receive = "EDS is confirmed!";
          }
          else
          {
            receive = "EDS is not confirmed!";
          }
          this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("you: " + receive + "\n"); }));                                 

          receive = "";
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message.ToString());
        }
      }
    }

    private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) //send data
    {
      if(client.Connected)
      {
        STW.WriteLine(text_to_send);
        this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("me: " + text_to_send + "\n"); }));

      }
      else
      {
        MessageBox.Show("Send failed!");
      }
      backgroundWorker2.CancelAsync();
    }

    private void button3_Click(object sender, EventArgs e) //connect to server
    {
      client = new TcpClient();
      var IP_End = new IPEndPoint(IPAddress.Parse(textBox5.Text), int.Parse(textBox6.Text));

      try
      {
        client.Connect(IP_End);
        if(client.Connected)
        {
          textBox2.AppendText("Connected to server" + "\n");
          STW = new StreamWriter(client.GetStream());
          STR = new StreamReader(client.GetStream());
          STW.AutoFlush = true;

          backgroundWorker1.RunWorkerAsync(); //Start receiving data to background
          backgroundWorker2.WorkerSupportsCancellation = true; //Ability to cancel this thread
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message.ToString());
      }
    }

    private void button1_Click(object sender, EventArgs e) //Send button
    {
      if(textBox1.Text != "")
      {
        text_to_send = textBox1.Text;
      }
      backgroundWorker2.RunWorkerAsync();
    }

    private void button4_Click(object sender, EventArgs e)
    {
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        var sr = new StreamReader(openFileDialog1.FileName);
        var data = sr.ReadToEnd();
        sr.Close();
        var add = new ElectronicDigitalSignature();
        var rand = new Random();
        var result = EDS.GenerateEDS(416, 55, Encoding.UTF8.GetBytes(data), private_key, 728, 5, -1, 751); 

        text_to_send = string.Join(",", result.ToArray());
        text_to_send += "," + openFileDialog1.FileName; //we have to send data, not file Name!!!
        MessageBox.Show("File was loaded!");
      }
    }
  }
}
