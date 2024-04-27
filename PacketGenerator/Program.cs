using System.Xml;

namespace PacketGenerator;

class Program
{
    static string clientRegister;
    static string serverRegister;


    static void Main(string[] args)
    {
        string file = "../../Common/protoc-3.12.3-win64/bin/Protocol.proto";

        // 인자가 있음
        if (args.Length >= 1)
            file = args[0];

        bool startParsing = false;
        foreach(string line in File.ReadAllLines(file))
        {
            if(!startParsing && line.Contains("enum MsgId"))
            {
                startParsing = true;
                continue;
            }

            if (!startParsing)
                continue;

            if (line.Contains("}"))
                break;

            string[] names = line.Trim().Split(" =");
            if (names.Length == 0)
                continue;

            string name = names[0];
            if (name.StartsWith("SC_"))
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"SC_{msgName.Substring(2)}";
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("CS_"))
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"CS_{msgName.Substring(2)}";
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
        }

        string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
        File.WriteAllText("ClientPacketManager.cs", clientManagerText);
        string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
        File.WriteAllText("ServerPacketManager.cs", serverManagerText);
    }

    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }
        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
    }
}

