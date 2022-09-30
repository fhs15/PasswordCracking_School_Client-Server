// See https://aka.ms/new-console-template for more information
using CrackerClientV2;
using CrackerClientV2.Models;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

TcpClient socket = new TcpClient("10.200.130.20", 6969);

NetworkStream ns = socket.GetStream();
StreamReader reader = new StreamReader(ns);
StreamWriter writer = new StreamWriter(ns);

string userJson = reader.ReadLine();

List<UserInfo> userInfo = JsonSerializer.Deserialize<List<UserInfo>>(userJson);

Cracking cracker = new Cracking();

while (socket.Connected)
{
    Console.WriteLine("Listening...");
    string message = "";
    try
    {
        message = reader.ReadLine();
        Console.WriteLine("Received...");
        List<string> stringList = JsonSerializer.Deserialize<List<string>>(message);

        Console.WriteLine("Cracking...");
        List<UserInfoClearText> clearText = cracker.RunCracking(userInfo, stringList);

        if (clearText.Any())
        {
            string json = JsonSerializer.Serialize(clearText);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(json);
            Console.ForegroundColor = ConsoleColor.White;
            writer.WriteLine(json);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("None found");
            Console.ForegroundColor = ConsoleColor.White;
            writer.WriteLine("none found");
        }        
        writer.Flush();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        socket.Close();
        return;
    }
}

socket.Close();

