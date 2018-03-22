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
    public class NSBob
    {   
        
        // public NSBob(){
        //     // BSKey=Key;
        // }
        static byte[] KeyAB = null;
        static void Main(string[] args){
            // var B=new NSBob(); 
            // B.BSKey=NSUtilities.getBytes(args[0]);
            // B.BSKey=NSUtilities.BSKey;
            byte[] BSKey;
            UdpClient Bob;
            IPEndPoint Alice;
            
            BSKey=new byte[] { 0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 
                                        0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            Bob = new UdpClient(NSUtilities.Bob_port);
            Alice = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NSUtilities.Alice_port); 
            int i=0;          
            Console.WriteLine("  RoleB start processor time: {0}",
                            Process.GetCurrentProcess().TotalProcessorTime);
            while(i<NSUtilities.loop){
                byte[] receivedData_init = Bob.Receive(ref Alice);
                run(receivedData_init,BSKey,Bob,Alice);
                i++;
                Console.WriteLine(i);

                if(i==NSUtilities.loop){
                    Console.WriteLine("  RoleB end processor time: {0}",
                            Process.GetCurrentProcess().TotalProcessorTime);
                    Process.GetCurrentProcess().Kill();
                }
            }
            

        }
        public static void run(byte[] receivedData_init,byte[] BSKey,UdpClient Bob,IPEndPoint Alice)
        {
            //Console.WriteLine("Bob: listens on port 11010.");

            Int64 nonceB0 = NSUtilities.getNonce();
            Int64 nonceB = NSUtilities.getNonce();

            
            string dataString_init = NSUtilities.getString(receivedData_init);
            //Console.WriteLine("Bob: receive info from Alice.");

            string[] splits_init = dataString_init.Split(new string[]{" "}, StringSplitOptions.None);
            if(String.Compare(splits_init[0],"msg1:") != 0 || String.Compare(splits_init[1],NSUtilities.Alice_port+"")!=0){
                //Console.WriteLine("Bob: does not recognize message.");
                return;
            }
            byte[] msg0_payload = NSUtilities.getBytes(NSUtilities.Alice_port + " " + nonceB0);
            byte[] msg0 = NSUtilities.getBytes("msg2: " + NSUtilities.getString(NSUtilities.Encrypt(msg0_payload, BSKey)));
            Bob.Send(msg0,msg0.Length, Alice);
            //Console.WriteLine("Bob: send first nonceB to Alice.");

            byte[] receivedData = Bob.Receive(ref Alice);
            string dataString = NSUtilities.getString(receivedData);
            //Console.WriteLine("Bob: receive Kab from Alice.");

            string[] splits = dataString.Split(new string[]{" "}, StringSplitOptions.None);
            if(String.Compare(splits[0],"msg5:") == 0){
                byte[] cipher3= NSUtilities.getBytes(dataString.Substring(6,dataString.Length-6));
                
                string msg3 = NSUtilities.getString(NSUtilities.Decrypt(cipher3,BSKey));
                string[] msg3s = msg3.Split(new string[]{" "}, StringSplitOptions.None);
                if( int.Parse(msg3s[2]) == NSUtilities.Alice_port && Int64.Parse(msg3s[1]) == nonceB0 )
                {
                    //Console.WriteLine("Bob: verified the first nonceB.");
                    KeyAB=NSUtilities.getBytes(msg3s[0]);

                    byte[] msg4combine=NSUtilities.getBytes("msg6: "+NSUtilities.getString(NSUtilities.Encrypt(BitConverter.GetBytes(nonceB),KeyAB)));
                   
                    Bob.Send(msg4combine,msg4combine.Length, Alice);
                    //Console.WriteLine("Bob: send second nonceB to Alice.");
                    byte[] receivedData2 = Bob.Receive(ref Alice);
                    string dataString2 = NSUtilities.getString(receivedData2);

                    string[] splits2 = dataString2.Split(new string[]{" "}, StringSplitOptions.None);
                    if(String.Compare(splits2[0],"msg7:") == 0){
                        byte[] cipher5= NSUtilities.getBytes(dataString2.Substring(6,dataString2.Length-6));
                        Int64 nonceBminus;
                        //parse nonceB-1
                        nonceBminus = BitConverter.ToInt64(NSUtilities.Decrypt(cipher5,KeyAB), 0);

                        if(nonceBminus + 1 != nonceB)
                        {
                            return;
                        }
                        //Console.WriteLine("Bob: verified nonceB-1.");
                        //Console.WriteLine("Bob: successfully finished key negotiation.");
                    }
                }
            }
        }
    }
}
