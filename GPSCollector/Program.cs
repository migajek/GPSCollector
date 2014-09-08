using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GPSCollector
{

    class MyBinder : SerializationBinder 
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == "CommunicationModel.DataReading")
                return typeof(CommunicationModel.DataReading);
            Console.WriteLine("unknown type {0} ", typeName);
            return null;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient listener = new UdpClient(5432);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 5432);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Binder = new MyBinder();
                while (true)
                {
                    using (var ms = new MemoryStream())
                    {
                        Console.WriteLine("Waiting for broadcast");
                        byte[] bytes = listener.Receive(ref groupEP);
                        ms.Write(bytes, 0, bytes.Length);
                        Console.WriteLine("Received {0} bytes!!", bytes.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        var reading = (CommunicationModel.DataReading)bf.Deserialize(ms);
                        reading.TimeReceived = DateTime.Now;                        
                        Console.WriteLine("recv {0} sent {1} delay {2}", reading.TimeReceived, reading.TimeSent,
                             reading.TimeReceived - reading.TimeSent);
                        var reply = new byte[1] { 10 };
                        listener.Send(reply, reply.Length, groupEP);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
