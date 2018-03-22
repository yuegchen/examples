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
    public class NSAlice
    {	
    	static byte[] ASKey;
    	static byte[] KeyAB = null;
    	static UdpClient Alice;
    	static IPEndPoint Bob;
    	static IPEndPoint server;
    	// private PerformanceCounter theCPUCounter = 
   // new PerformanceCounter("Processor", "% Processor Time", Process.GetCurrentProcess().ProcessName);
    	public NSAlice(){
    		// ASKey=Key;
    	}
    	static void Main(string[] args){
    		// var A = new NSAlice();
    		// A.ASKey=NSUtilities.getBytes(args[0]);
    		// A.ASKey=NSUtilities.ASKey;
    		ASKey=new byte[] { 0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 
                                        0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x8, 0x8, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
       
    		Alice = new UdpClient(NSUtilities.Alice_port);
    		Bob = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NSUtilities.Bob_port);
    		server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NSUtilities.server_port); 
    		
    		Console.WriteLine("  RoleA start processor time: {0}",
                            Process.GetCurrentProcess().TotalProcessorTime);
    		for(int i=1;i<=NSUtilities.loop;i++){
    			run();
    		}
    		Console.WriteLine("  RoleA end processor time: {0}",
                            Process.GetCurrentProcess().TotalProcessorTime);
    		Process.GetCurrentProcess().Kill();
    		
    	}
        public static void run()
        {
            

            //Console.WriteLine("Alice: Sends its identity to Bob");
            Alice.Connect(Bob);

            byte[] msg_init = NSUtilities.getBytes("msg1: "+NSUtilities.Alice_port);
            Alice.Send(msg_init,msg_init.Length);

            byte[] receivedData_init = Alice.Receive(ref Bob);
            string dataString_init = NSUtilities.getString(receivedData_init);

            //Console.WriteLine("Alice: receive first nonce from Bob.");

            string[] splits_init = dataString_init.Split(new string[]{" "}, StringSplitOptions.None);
            if(String.Compare(splits_init[0],"msg2:") != 0){
                //Console.WriteLine("Alice: does not recognize message.");
                return;
            }
            //Console.WriteLine("Alice: Send key request to server");
			
			Alice.Connect(server);
		
			Int64 nonceA = NSUtilities.getNonce();
			Int64 nonceB;

			byte[] msg = NSUtilities.getBytes("msg3: "+NSUtilities.Alice_port+" "+NSUtilities.Bob_port+" "+nonceA+" "+splits_init[1]);
			// send to server
			Alice.Send(msg,msg.Length);

			// then receive data
			byte[] receivedData = Alice.Receive(ref server);
			string dataString = NSUtilities.getString(receivedData);

			//Console.WriteLine("Alice: receive key info from Server.");

			string[] splits = dataString.Split(new string[]{" "}, StringSplitOptions.None);
			if(String.Compare(splits[0],"msg4:") == 0){
				byte[] cipher2= NSUtilities.getBytes(dataString.Substring(6,dataString.Length-6));
				string msg2 = NSUtilities.getString(NSUtilities.Decrypt(cipher2,ASKey));

				string[] msg2s = msg2.Split(new string[]{" "}, StringSplitOptions.None);
				if( Int64.Parse(msg2s[0]) == nonceA && int.Parse(msg2s[1]) == NSUtilities.Bob_port )
                {
					KeyAB=NSUtilities.getBytes(msg2s[2]);
					byte[] msg3combine=NSUtilities.getBytes("msg5: "+msg2s[3]);
					
					// IPEndPoint Bob = new IPEndPoint(IPAddress.Parse("127.0.0.1"), NSUtilities.Bob_port);
					Alice.Connect(Bob);
					Alice.Send(msg3combine,msg3combine.Length);
					//Console.WriteLine("Alice: send Kab to Bob.");
					byte[] receivedData2 = Alice.Receive(ref Bob);
					string dataString2 = NSUtilities.getString(receivedData2);

					string[] splits2 = dataString2.Split(new string[]{" "}, StringSplitOptions.None);
					if(String.Compare(splits2[0],"msg6:") == 0){
						byte[] cipher4= NSUtilities.getBytes(dataString2.Substring(6,dataString2.Length-6));
						// parse nounceB
						nonceB = BitConverter.ToInt64(NSUtilities.Decrypt(cipher4,KeyAB), 0);
						//Console.WriteLine("Alice: decrypted nonceB with Kab.");
						nonceB--;
						byte[] msg5combine=NSUtilities.getBytes("msg7: "+NSUtilities.getString(NSUtilities.Encrypt(BitConverter.GetBytes(nonceB),KeyAB)));
						Alice.Send(msg5combine,msg5combine.Length);
						//Console.WriteLine("Alice: successfully finished key negotiation.");
					}
				}
			}
		}
    }
}
