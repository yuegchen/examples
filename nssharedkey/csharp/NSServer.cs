using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace NS_SK
{
   public class NSServer
    {
        
   //      private PerformanceCounter theCPUCounter = 
   // new PerformanceCounter("Processor", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        // public NSServer(byte[] Key1,byte[] Key2){
        //     ASKey=Key1;
        //     BSKey=Key2;
        //     remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NSUtilities.server_port); 
        //     udpServer = new UdpClient(NSUtilities.server_port);
        // }
        static void Main(string[] args){
            byte[] ASKey;
            byte[] BSKey;
            IPEndPoint remoteEP;
            UdpClient udpServer;
            remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NSUtilities.server_port); 
            udpServer = new UdpClient(NSUtilities.server_port);
            // string[] keys = args[0].Split(new string[]{" "}, StringSplitOptions.None);
            // BSKey=NSUtilities.getBytes(args[1]);
            // ASKey=NSUtilities.getBytes(args[0]);
            // ASKey=NSUtilities.ASKey;
            // BSKey=NSUtilities.BSKey;
            ASKey=new byte[] { 0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 
                                        0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            BSKey=new byte[] { 0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 
                                        0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            string nonceA = null;
            string nonceB = null;
            //Console.WriteLine("Start Server at port 11000");

            int i=0;
            Console.WriteLine("  RoleS start processor time: {0}",
                            Process.GetCurrentProcess().TotalProcessorTime);
            while(true) {
                
                byte[] data = udpServer.Receive(ref remoteEP); // listen on port 11000

                //Console.WriteLine("Server: receive key request from Alice. ");
                string dataString = NSUtilities.getString(data);

                string[] msgs = dataString.Split(new string[]{" "}, StringSplitOptions.None);
               
                if(String.Compare(msgs[0],"msg3:") == 0 && int.Parse(msgs[1]) == NSUtilities.Alice_port &&
                   int.Parse(msgs[2]) == NSUtilities.Bob_port)
                {
                    nonceA = msgs[3];
                    string payload_B = NSUtilities.getString(NSUtilities.Decrypt(NSUtilities.getBytes(msgs[4]), BSKey));
                    nonceB = payload_B.Split(new string[]{" "}, StringSplitOptions.None)[1];

                    // Aes aesAlg = Aes.Create();
                    byte[] keyAB = NSUtilities.getKey(32);
                    string kAB_s = NSUtilities.getString(keyAB);
                    byte[] kAB_A = NSUtilities.getBytes(kAB_s+" "+nonceB+" "+NSUtilities.Alice_port);
                   
                    byte[] enc_kAB_A = NSUtilities.Encrypt(kAB_A, BSKey);
                    
                    string enc_kAB_A_s = NSUtilities.getString(enc_kAB_A);
                    byte[] msg2s = NSUtilities.getBytes(nonceA +" "+NSUtilities.Bob_port+" "+kAB_s+" "+enc_kAB_A_s);
                   
                    byte[] msg2 = NSUtilities.Encrypt(msg2s,ASKey);
                    byte[] msg2combine=NSUtilities.getBytes("msg4: "+NSUtilities.getString(msg2));
                    udpServer.Send(msg2combine, msg2combine.Length, remoteEP);
                    //Console.WriteLine("Server: send Alice Kab. ");
                }
                i++;
                if(i==NSUtilities.loop){
                    Console.WriteLine("  RoleS end processor time: {0}",
                            Process.GetCurrentProcess().TotalProcessorTime);
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
    }
}
