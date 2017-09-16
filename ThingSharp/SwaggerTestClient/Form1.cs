using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IO.Swagger.Api;
using IO.Swagger.Model;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private int _logIndex = 0;
        
        public Form1()
        {
            InitializeComponent();
            comboBoxPower.SelectedIndex = 0;
            textBoxColor.Text = "91F9FF";
        }

        private string GetSorisAddress()
        {
            string ip = "192.168.1.123";
            string port = "8080";

            if (!String.IsNullOrEmpty(TextBox_AdapterIP.Text))
                ip = TextBox_AdapterIP.Text;

            if (!String.IsNullOrEmpty(textBox_AdapterPort.Text))
                port = textBox_AdapterPort.Text;

            return String.Format("http://{0}:{1}", ip, port);
        }

        private void buttonDiscovery_Click(object sender, EventArgs e)
        {
            DefaultApi  instance = new DefaultApi(GetSorisAddress());
            
            DateTime dt0 = DateTime.Now;
            string strDate = String.Format("{0:MM/dd HH:mm:ss.ffffff}", dt0);
            ListViewItem item = listViewLog.Items.Add(_logIndex.ToString());
            item.SubItems.Add(strDate);
            item.SubItems.Add("Discovery start");

            var bulbs = new Linklist();

            try
            {
                bulbs = instance.RootGet();
            }
            catch (Exception ex)
            {
                string text = String.Format("ERROR: Either the SORIS Adapter Address is wrong, or the SORIS Adapter is not running.");
                SetLog(_logIndex++, text, dt0);
                //MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetLog(_logIndex++, "Discovery finish", dt0);
            
            listBox1.Items.Clear();

            foreach (var bulb in bulbs.Links)
            {
                listBox1.Items.Add(bulb.Uri);
            }            
        }

        private void SetLog(int js, string msg, DateTime dt0)
        {
            DateTime dt = DateTime.Now;
            string strDate = String.Format("{0:MM/dd HH:mm:ss.ffffff}", dt);
            ListViewItem item = listViewLog.Items.Add(js.ToString());
            item.SubItems.Add(strDate);
            item.SubItems.Add(msg);
            dt = new DateTime(dt.Ticks - dt0.Ticks);
            strDate = String.Format("{0:HH:mm:ss.ffffff}", dt);
            item.SubItems.Add(strDate);

            listViewLog.TopItem = listViewLog.Items[listViewLog.Items.Count - 1];
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            listViewLog.Items.Clear();
            _logIndex = 0;
        }

        private void buttonGetPower_Click(object sender, EventArgs e)
        {
            foreach (var light in listBox1.SelectedItems)
            {
                DefaultApi lightInstance = new DefaultApi(light.ToString());

                DateTime dt0 = DateTime.Now;
                string strDate = String.Format("{0:MM/dd HH:mm:ss.ffffff}", dt0);
                ListViewItem item = listViewLog.Items.Add(_logIndex.ToString());
                item.SubItems.Add(strDate);
                item.SubItems.Add("Get Power start - " + light.ToString());

                var pw = lightInstance.PowerGet();

                SetLog(_logIndex++, "Get Power finish. Power = " + pw.ToString(), dt0);
            }
        }

        private void buttonGetColor_Click(object sender, EventArgs e)
        {
            foreach (var light in listBox1.SelectedItems)
            {
                DefaultApi lightInstance = new DefaultApi(light.ToString());

                DateTime dt0 = DateTime.Now;
                string strDate = String.Format("{0:MM/dd HH:mm:ss.ffffff}", dt0);
                ListViewItem item = listViewLog.Items.Add(_logIndex.ToString());
                item.SubItems.Add(strDate);
                item.SubItems.Add("Get Color start" + light.ToString());

                var co = lightInstance.ColorGet();

                SetLog(_logIndex++, "Get Color finish. Color = " + co.ToString(), dt0);
            }
        }

        private void buttonSetPower_Click(object sender, EventArgs e)
        {
            foreach (var light in listBox1.SelectedItems)
            {
                DefaultApi lightInstance = new DefaultApi(light.ToString());

                DateTime dt0 = DateTime.Now;
                string strDate = String.Format("{0:MM/dd HH:mm:ss.ffffff}", dt0);
                ListViewItem item = listViewLog.Items.Add(_logIndex.ToString());
                item.SubItems.Add(strDate);
                item.SubItems.Add("Set Power start - " + light.ToString());

                lightInstance.SetPower(new Power(comboBoxPower.SelectedIndex == 0));

                SetLog(_logIndex++, "Set Power finish. Power = " + (comboBoxPower.SelectedIndex == 0).ToString(), dt0);
            }
        }

        private void buttonSetColor_Click(object sender, EventArgs e)
        {
            foreach (var light in listBox1.SelectedItems)
            {
                DefaultApi lightInstance = new DefaultApi(light.ToString());

                DateTime dt0 = DateTime.Now;
                string strDate = String.Format("{0:MM/dd HH:mm:ss.ffffff}", dt0);
                ListViewItem item = listViewLog.Items.Add(_logIndex.ToString());
                item.SubItems.Add(strDate);
                item.SubItems.Add("Set Color start - " + light.ToString());
                string str = textBoxColor.Text + "000000";
                str = str.Substring(0, 6);

                try
                {
                    lightInstance.SetColor(new IO.Swagger.Model.Color(str));
                }
                catch (Exception ex)
                {
                    string text = String.Format("ERROR: The Color string entered ({0}) is not valid", textBoxColor.Text);
                    SetLog(_logIndex++, text, dt0);
                    //MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SetLog(_logIndex++, "Set Color finish. Color = " + str, dt0);
            }
        }
    }
}
