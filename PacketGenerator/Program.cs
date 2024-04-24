﻿using System.Xml;

namespace PacketGenerator;

class Program
{
    static string genPacket;

    static ushort packetId;
    static string packetEnum;

    static string clientRegister;
    static string serverRegister;


    static void Main(string[] args)
    {
        string pdlPath = "../PDL.xml";

        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true,  // 주석 무시
            IgnoreWhitespace = true, // 스페이스바 무시
        };

        // 인자가 있음
        if (args.Length >= 1)
            pdlPath = args[0];

        // using으로 감싸면 끝날때 r.Dispose() 호출
        using (XmlReader r = XmlReader.Create(pdlPath, settings))
        {
            r.MoveToContent();
            while(r.Read())
            {
                if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                    ParsePacket(r);
                //Console.WriteLine(r.Name + " " + r["name"]);
            }

            string fileText = string.Format(PacketFormat.fileFormat, packetEnum, genPacket);
            File.WriteAllText("GenPackets.cs", fileText);
            string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
            File.WriteAllText("ClientPacketManager.cs", clientManagerText);
            string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
            File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        }
    }

    public static void ParsePacket(XmlReader r)
    {
        if (r.NodeType == XmlNodeType.EndElement)
            return;

        if (r.Name.ToLower() != "packet")
        {
            Console.WriteLine("Invalid Packet node");
            return;
        }

        string packetName = r["name"];
        if (string.IsNullOrEmpty(packetName))
        {
            Console.WriteLine("Packet without name");
            return;
        }

        Tuple<string, string, string> t = ParseMembers(r);
        genPacket += string.Format(PacketFormat.packetFormat,
            packetName, t.Item1, t.Item2, t.Item3);
        packetEnum += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";

        if (packetName.StartsWith("SC_") || packetName.StartsWith("sc_"))
            clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
        else
            serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
    }

    // {0} 패킷 이름
    // {1} 멤버 변수들
    // {2} 멤버 변수 Read
    // {3} 멤버 변수 Write
    public static Tuple<string, string, string> ParseMembers(XmlReader r)
    {
        string packetName = r["name"];

        string memberCode = "";
        string readCode = "";
        string writeCode = "";

        int depth = r.Depth + 1;
        while(r.Read())
        {
            if (r.Depth != depth)
                break;

            string memberName = r["name"];
            if (string.IsNullOrEmpty(memberName))
            {
                Console.WriteLine("Member without name");
                return null;
            }

            if (string.IsNullOrEmpty(memberCode) == false)
                memberCode += Environment.NewLine;
            if (string.IsNullOrEmpty(readCode) == false)
                readCode += Environment.NewLine;
            if (string.IsNullOrEmpty(writeCode) == false)
                writeCode += Environment.NewLine;

            string memberType = r.Name.ToLower();
            switch(memberType)
            {
                case "byte":
                case "sbyte":
                    memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                    readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                    writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                    break;
                case "bool":
                case "short":
                case "ushort":
                case "int":
                case "long":
                case "float":
                case "double":
                    memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                    readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                    writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                    break;
                case "string":
                    memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                    readCode += string.Format(PacketFormat.readStringFormat, memberName);
                    writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                    break;
                case "list":
                    Tuple<string, string, string> t = ParseList(r);
                    memberCode += t.Item1;
                    readCode += t.Item2;
                    writeCode += t.Item3;
                    break;
                default:
                    break;
            }
        }

        memberCode = memberCode.Replace("\n", "\n\t");
        readCode = readCode.Replace("\n", "\n\t\t");
        writeCode = writeCode.Replace("\n", "\n\t\t");
        return new Tuple<string, string, string>(memberCode, readCode, writeCode);
    }

    public static Tuple<string, string, string> ParseList(XmlReader r)
    {
        string listName = r["name"];
        if (string.IsNullOrEmpty(listName))
        {
            Console.WriteLine("List without name");
            return null;
        }

        Tuple<string, string, string> t = ParseMembers(r);

        string memberCode = string.Format(PacketFormat.memListFormat,
            FirstCharToUpper(listName),
            FirstCharToLower(listName),
            t.Item1,
            t.Item2,
            t.Item3);

        string readCode = string.Format(PacketFormat.readListFormat,
            FirstCharToUpper(listName),
            FirstCharToLower(listName));

        string writeCode = string.Format(PacketFormat.writeListFormat,
            FirstCharToUpper(listName),
            FirstCharToLower(listName));

        return new Tuple<string, string, string>(memberCode, readCode, writeCode);
    }

    public static string ToMemberType(string memberType)
    {
        switch(memberType)
        {
            case "bool":
                return "ToBoolean";
            case "short":
                return "ToInt16";
            case "ushort":
                return "ToUInt16";
            case "int":
                return "ToInt32";
            case "long":
                return "ToInt64";
            case "float":
                return "ToSingle";
            case "double":
                return "ToDouble";
            default:
                return "";
        }
    }

    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }
        return input[0].ToString().ToUpper() + input.Substring(1);
    }

    public static string FirstCharToLower(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }
        return input[0].ToString().ToLower() + input.Substring(1);
    }
}

