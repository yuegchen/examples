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
    

    class NSCorrected{
        // private PerformanceCounter theCPUCounter = 
   // new PerformanceCounter("Processor", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        static void Main(string[] args){
            // NSUtilities.ASKey = (NSUtilities.getKey(32));
            // NSUtilities.BSKey = (NSUtilities.getKey(32));
            
            //Console.WriteLine(ASKey.Length);
            // var B = new NSBob(BSKey);
            // var A = new NSAlice(ASKey);
            // var S = new NSServer(ASKey, BSKey);
    
            // Task t1 = new Task(S.run);

            // Task t2 = new Task(B.start);
            
            // t1.Start();
            // t2.Start();
            // A.start();

            Process ks_proc = new Process();
            Process bob_proc = new Process();
            Process alice_proc = new Process();
            ks_proc.StartInfo.UseShellExecute = false;
                
                ks_proc.StartInfo.FileName = "NSServer.exe";
                // ks_proc.StartInfo.Arguments = ASKey+" "+BSKey;
                ks_proc.StartInfo.CreateNoWindow = true;
                ks_proc.StartInfo.UseShellExecute = false;
                
                bob_proc.StartInfo.FileName = "NSBob.exe";
                // bob_proc.StartInfo.Arguments = BSKey;
                bob_proc.StartInfo.CreateNoWindow = true;
                bob_proc.StartInfo.UseShellExecute = false;
                
                alice_proc.StartInfo.FileName = "NSAlice.exe";
                // alice_proc.StartInfo.Arguments = ASKey;
                alice_proc.StartInfo.CreateNoWindow = true;
                alice_proc.StartInfo.UseShellExecute = false;
            ks_proc.Start(); 
            bob_proc.Start();     
            alice_proc.Start();
            
        }
    }
}
