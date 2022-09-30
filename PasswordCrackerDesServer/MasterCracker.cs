using PasswordCrackerDesServer.model;
using PasswordCrackerDesServer.util;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Diagnostics;

namespace PasswordCrackerDesServer
{
    public class MasterCracker
    {
        Stopwatch stopwatch = new();
        int DicDepth = 0;
        List<UserInfo> userInfos = new();
        List<string> dictionary = new();
        List<UserInfoClearText> Cracked = new();
        int workload = 10000;
        public void start()
        {
            Console.WriteLine("Server");

            userInfos = PasswordFileHandler.ReadPasswordFile("passwords.txt");

            using (FileStream fs = new FileStream("webster-dictionary.txt", FileMode.Open, FileAccess.Read))

            using (StreamReader dictionaryStream = new StreamReader(fs))
            {
                while (!dictionaryStream.EndOfStream)
                {
                    dictionary.Add(dictionaryStream.ReadLine());
                }
            }

            TcpListener listener = new(IPAddress.Any, 6969);
            listener.Start();
            stopwatch.Start();
            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Task.Run(() => HandleClient(socket));
            }


            listener.Stop();
        }

        private void HandleClient(TcpClient socket)
        {
            NetworkStream stream = socket.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            Console.WriteLine("New Connection");
            string message = null;
            message = JsonSerializer.Serialize(userInfos);
            writer.WriteLine(message);
            writer.Flush();
            string result;
            List<string> NextWork = dictionary.Skip(workload * DicDepth).Take(workload).ToList();
            DicDepth++;
            while (NextWork.Any() && socket.Connected)
            {
                message = JsonSerializer.Serialize(NextWork);
                writer.WriteLine(message);
                writer.Flush();
                result = reader.ReadLine();
                Console.WriteLine("Work done");
                if (result != "none found")
                {
                    List<UserInfoClearText> userInfoClearTexts = JsonSerializer.Deserialize<List<UserInfoClearText>>(result);
                    foreach(var i in userInfoClearTexts)
                    {
                        Cracked.Add(i);
                    }
                }
                NextWork = dictionary.Skip(workload * DicDepth).Take(workload).ToList();
                DicDepth++;
                Console.WriteLine(DicDepth);
            }

            socket.Close();
            foreach(var item in Cracked)
            {
                Console.WriteLine(item);
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        }
    }
}
