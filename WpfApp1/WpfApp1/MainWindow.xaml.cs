using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool flag_on_off = false;
        Lan_data lan = new Lan_data();
        //ApplicationState seiv = new ApplicationState();

        public MainWindow()
        {
            InitializeComponent();
            edtId.Text = FREQUENCY_TRANSFER_DEVICES_30_37.Properties.Settings.Default.ID_con;
            edtPORT.Text = FREQUENCY_TRANSFER_DEVICES_30_37.Properties.Settings.Default.PORT_con; //Извлекаем содержимое 
            FREQUENCY_TRANSFER_DEVICES_30_37.Properties.Settings.Default.Save(); // Сохраняем переменные.
            if ((edtId.Text == "") && (edtPORT.Text == ""))
            {
                edtId.Text = "192.168.1.77";
                edtPORT.Text = "5025";
            }
        }

        private void btnE8257D_Click(object sender, RoutedEventArgs e)
        {
            if ((edtPORT.Text != "") && (edtId.Text != ""))
            {
                if ((flag_on_off == false))
                {
                    if (lan.connect(int.Parse(edtPORT.Text), edtId.Text.ToString()) == 0)
                    {
                        FREQUENCY_TRANSFER_DEVICES_30_37.Properties.Settings.Default.ID_con = edtId.Text; // Записываем содержимое 
                        FREQUENCY_TRANSFER_DEVICES_30_37.Properties.Settings.Default.PORT_con = edtPORT.Text;
                        FREQUENCY_TRANSFER_DEVICES_30_37.Properties.Settings.Default.Save(); // Сохраняем переменные.
                        btnE8257D.Content = "Disconnect";
                        flag_on_off = true;
                        up.IsEnabled = true;
                        down.IsEnabled = true;
                        Send.IsEnabled = true;
                        edtId.IsEnabled = false;
                        edtPORT.IsEnabled = false;
                        //connect_time();
                    }
                }
                else
                {
                    lan.discount(/*int.Parse(edtPORT.Text), edtId.Text.ToString()*/);
                    btnE8257D.Content = "Connect";
                    flag_on_off = false;
                    up.IsEnabled = false;
                    down.IsEnabled = false;
                    Send.IsEnabled = false;
                    edtId.IsEnabled = true;
                    edtPORT.IsEnabled = true;
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Нет данных!");
            }
        }

        async void connect_time()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            if (lan.SocketConnected() == false)
            {
                lan.discount(/*int.Parse(edtPORT.Text), edtId.Text.ToString()*/);
                flag_on_off = false;
                up.IsEnabled = false;
                down.IsEnabled = false;
                Send.IsEnabled = false;
                edtId.IsEnabled = true;
                edtPORT.IsEnabled = true;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(10));

        }

        //*************************************************************************************************
        //*************************************************************************************************
        //*************************************************************************************************

        private void Freq_UP_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Freq_UP.Text.ToString() == "")
            {
                lan.send("UP", "FREQ", "0", Freq_UP_cmb.Text.ToString());
            }
            else
            {
                lan.send("UP", "FREQ", Freq_UP.Text.ToString(), Freq_UP_cmb.Text.ToString());
            }
        }

        private void AMP_UP_btn_Click(object sender, RoutedEventArgs e)
        {
            if (AMP_UP.Text.ToString() == "")
            {
                lan.send("UP", "POWER", "0", AMP_UP_cmb.Text.ToString());
            }
            else
            {
                lan.send("UP", "POWER", AMP_UP.Text.ToString(), AMP_UP_cmb.Text.ToString());
            }
        }

        private void UP_RF_Checked(object sender, RoutedEventArgs e)
        {
            lan.sendON_OFF("UP", "RF", "ON");
        }

        private void UP_RF_Unchecked(object sender, RoutedEventArgs e)
        {
            lan.sendON_OFF("UP", "RF", "OFF");
        }

        //*************************************************************************************************
        //*************************************************************************************************
        //*************************************************************************************************

        private void Freq_DOWN_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Freq_DOWN.Text.ToString() == "")
            {
                lan.send("DOWN", "FREQ", "0", Freq_DOWN_cmb.Text.ToString());
            }
            else
            {
                lan.send("DOWN", "FREQ", Freq_DOWN.Text.ToString(), Freq_DOWN_cmb.Text.ToString());
            }
        }

        private void AMP_DOWN_btn_Click(object sender, RoutedEventArgs e)
        {
            if (AMP_DOWN.Text.ToString() == "")
            {
                lan.send("DOWN", "POWER", "0", AMP_DOWN_cmb.Text.ToString());
            }
            else
            {
                lan.send("DOWN", "POWER", AMP_DOWN.Text.ToString(), AMP_DOWN_cmb.Text.ToString());
            }
        }

        private void DOWN_RF_Checked(object sender, RoutedEventArgs e)
        {
            lan.sendON_OFF("DOWN", "RF", "ON");
        }

        private void DOWN_RF_Unchecked(object sender, RoutedEventArgs e)
        {
            lan.sendON_OFF("DOWN", "RF", "OFF");
        }

        //*************************************************************************************************
        //*************************************************************************************************
        //*************************************************************************************************

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            if ((send_Synt.Text.ToString() == "") || (send_Synt.Text.ToString() == " "))
            {

            }
            else
            {
                lan.send_string(send_Synt.Text.ToString());
            }
        }
        public void Resiv_txt(string R)
        {
            Return_Synt.Text = "\0";
            Return_Synt.Text = "\0";
            Return_Synt.Text = "\0";
            Return_Synt.Text = R;
        }
    }

    public class Lan_data
    {
        public Socket socket;

        public int connect(int port, string address)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                socket.Connect(ipPoint);
                return 0;

            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message);
                return 1;
            }

        }

        public void discount()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                //socket.Disconnect(true);
                socket.Disconnect(true);
                //((MainWindow)System.Windows.Application.Current.MainWindow).discounct_Expanded();
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message);
            }

        }

        public bool SocketConnected()
        {
            if (socket.Connected == true) return true;
            else return false;
            /*if (socket.Available != 0) return false;
            else return true;*/
        }

        public void send(string port, string type, string value, string size)
        {
            string s = port + ":" + type + ":" + value + ":" + size;
            byte[] data = Encoding.ASCII.GetBytes(s + "\n");
            try
            {
                socket.Send(data);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message);
                discount();
            }
        }

        public void sendON_OFF(string port, string type, string ON_OFF)
        {
            byte[] data = Encoding.ASCII.GetBytes(port + " " + type + " " + ON_OFF + "\n");
            try
            {
                socket.Send(data);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message);
                discount();
            }
        }

        public void send_string(string strv)
        {
            byte[] data = Encoding.ASCII.GetBytes(strv + "\n");
            try
            {
                socket.Send(data);
                if (strv.EndsWith("?"))
                {
                    Receive_string();
                }
            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show(ex.Message);
                discount();
            }

        }
        public void Receive_string()
        {
            byte[] data = new byte[255]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт

            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);

            ((MainWindow)System.Windows.Application.Current.MainWindow).Resiv_txt(builder.ToString());
        }
    }

    
}
