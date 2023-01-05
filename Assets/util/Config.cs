using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Config
{
    public string ip = "127.0.0.1";

    public short port = 25565;

    public string description = "A default server";

    public string filePath;

    public int maxPlayers = 20;

    public bool generateCrashReports = true;

    public Config(string filePath)
    {
        this.filePath = filePath;
    }

    public void GenerateConfig()
    {
        if (!File.Exists(filePath + "/config.conf"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Debug.WriteLine("[WARNING] attemped to write new config data while config already generated \n" + new System.Exception().StackTrace);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else
        {
            FileStream fs = File.Create(filePath + "/config.conf");
            fs.Close();

            string[] properties = new string[]
{
                "ip=" + ip,
                "port=" + port,
                "description=" + description,
                "maxPlayers=" + maxPlayers,
                "generateCrashReports=" + generateCrashReports
};

            File.WriteAllLines(filePath + "/config.conf", properties);
        }

    }

    public static Config FromConfigPath(string path)
    {
        Config toSave = new Config(path);

        if (File.Exists(path + "/config.conf"))
        {
            string[] properties = File.ReadAllLines(path + "/config.conf");

            toSave.ip = properties[0].Split("=")[1].Split("\n")[0];
            toSave.port = short.Parse(properties[1].Split("=")[1].Split("\n")[0]);
            toSave.description = properties[2].Split("=")[1].Split("\n")[0];
            toSave.maxPlayers = int.Parse(properties[3].Split("=")[1].Split("\n")[0]);
            toSave.generateCrashReports = bool.Parse(properties[4].Split("=")[1].Split("\n")[0]);
        }

        return toSave;
    }
}
