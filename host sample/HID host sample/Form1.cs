using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HID_host_sample {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private OaktreeLab.USBDevice.HIDSimple device =null;
        private Timer timer = null;

        private void buttonOpen_Click( object sender, EventArgs e ) {
            // HIDデバイスをオープンする
            device = new OaktreeLab.USBDevice.HIDSimple();
            if ( device.Open( 0x04d8, 0x003f ) ) {
                // オープン成功
                // ボタンを切り替え
                buttonOpen.Enabled = false;
                buttonLEDOn.Enabled = true;
                buttonLEDOff.Enabled = true;
                // タイマー起動
                timer = new Timer();
                timer.Interval = 100;
                timer.Tick += new EventHandler( timer_Tick );
                timer.Start();

            } else {
                // オープン失敗（デバイスが見つからない）
                MessageBox.Show( "デバイスが見つかりません" );
            }
        }

        private void Form1_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( device != null ) {
                device.Dispose();
            }
            if ( timer != null ) {
                timer.Dispose();
            }
        }

        private void timer_Tick( object sender, EventArgs e ) {
            // 先頭1バイトに0x01を入れてパケットを送信
            device.Send( 0x01 );
            // デバイスからパケットを受信
            byte[] res = device.Receive();
            if ( res[0] == 0 ) {
                textBoxSwitch.Text = "OFF";
            } else {
                textBoxSwitch.Text = "ON";
            }

            // 先頭1バイトに0x02を入れてパケットを送信
            device.Send( 0x02 );
            // デバイスからパケットを受信
            res = device.Receive();
            uint value = (uint)res[0] * 256 + res[1];
            textBoxADC.Text = value.ToString();
        }

        private void buttonLEDOn_Click( object sender, EventArgs e ) {
            // 先頭2バイトに0x00,0x01を入れてパケットを送信
            device.Send( 0x00, 0x01 );
            // デバイスからパケットを受信
            device.Receive();
        }

        private void buttonLEDOff_Click( object sender, EventArgs e ) {
            // 先頭2バイトに0x00,0x00を入れてパケットを送信
            device.Send( 0x00, 0x00 );
            // デバイスからパケットを受信
            device.Receive();
        }

    }
}
