using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace serverUdp
{
    public class serverUdp
    {
        //puerto de escucha
        private const int sampleUdpPort = 5555;
        //hilo
        public Thread sampleUdpThread;
        /// <summary>
        /// Constructor que inicializa un nuevo hilo
        /// </summary>
        public serverUdp()
        {
            try
            {
                //Inicia el nuevo hilo
                sampleUdpThread = new Thread(new ThreadStart(StartReceiveFrom));
                sampleUdpThread.Start();
                Console.WriteLine("Started SampleTcpUdpServer's UDP Receiver Thread!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("An UDP Exception has occurred!" + e.ToString());
                sampleUdpThread.Abort();
            }
        }
        /// <summary>
        /// Función que inicia un servidor UDP en un nuevo hilo
        /// </summary>
        public void StartReceiveFrom()
        {
            //dirección IP local
            IPAddress ipLocal = IPAddress.Loopback;
            try
            {
                //Create a UDP socket.
                Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //configura la dirección IP
                IPEndPoint localIpEndPoint = new IPEndPoint(ipLocal, sampleUdpPort);
                soUdp.Bind(localIpEndPoint);

                //configura el puerto serie
                SerialPort sp = new SerialPort();
                sp.PortName = "COM1";
                sp.BaudRate = 9600 ;
                sp.Parity = System.IO.Ports.Parity.None;
                sp.StopBits = System.IO.Ports.StopBits.One;
                sp.DataBits = 8;

                //recibe valores
                Byte[] received = new Byte[256];
                IPEndPoint tmpIpEndPoint = new IPEndPoint(ipLocal, sampleUdpPort);
                EndPoint remoteEP = (tmpIpEndPoint);

                //bucle infinito del servidor UDP
                while (true)
                {
                    //lee datos recibidos
                    int bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);
                    if (bytesReceived > 0)
                    {
                        //Si hay datos se envía por puerto serie
                        try
                        {
                            if (!sp.IsOpen)
                            {
                                sp.Open();
                            }
                            Byte[] datos = new Byte[bytesReceived];
                            for (int i = 0; i < bytesReceived; i++)
                            {
                                datos[i] = received[i];
                            }

                            //se envían los datos por el puerto COM1
                            sp.Write(datos, 0, datos.Length);
                            Console.WriteLine("datos enviados: " + Encoding.UTF8.GetString(datos));
                            sp.Close();
                            bytesReceived = 0;
                        }
                        catch (UnauthorizedAccessException uae)
                        {
                            Console.WriteLine("Error: " + uae.Message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        
                    }
                   
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            serverUdp sU = new serverUdp();
        }
    }
}
