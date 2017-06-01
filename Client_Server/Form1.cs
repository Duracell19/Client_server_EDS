using CryptoWizard.Services;
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
using System.Numerics;
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
          var data = receive.Split(',');

          var privateKey = data.ElementAt(2);

          long numOfBytes = data.Last().Length / 8;
          var bytes = new byte[numOfBytes];
          for (int i = 0; i < numOfBytes; i++)
          {
            bytes[i] = Convert.ToByte(data.Last().Substring(8 * i, 8), 2);
          }

          var res = EDS.CheckEDS(416, 55, Convert.ToInt64(data.ElementAt(0)), Convert.ToInt64(data.ElementAt(1)), bytes, Convert.ToInt64(privateKey), 728, -1, 751);

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
        this.textBox2.Invoke(new MethodInvoker(delegate() { textBox2.AppendText("me: Data was sending" + "\n"); }));

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
    //  var sWatch = new Stopwatch();
    //  sWatch.Start();

    //  var res = new Addition().AdditionResult(41, 55, 416, 55, -1, 751);

    //  sWatch.Stop();
    //  var time = sWatch.ElapsedMilliseconds.ToString();

      if (textBox1.Text != "")
      {
        text_to_send = textBox1.Text;
      }
      backgroundWorker2.RunWorkerAsync();
    }

    private void button4_Click(object sender, EventArgs e)
    {      
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        var bytes = File.ReadAllBytes(openFileDialog1.FileName);
        var data = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        var k = new Random().Next(1, 11);
        var privateKey = 12;
        var result = EDS.GenerateEDS(416, 55, bytes, privateKey, 728, k, -1, 751);
        text_to_send = string.Format("{0},{1},{2},{3}", result.ElementAt(0), result.ElementAt(1), privateKey, data);
        MessageBox.Show("File was loaded!");        
      }
    }
  }
}
