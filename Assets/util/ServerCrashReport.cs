using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class ServerCrashReport
{
    public ServerCrashReport(StackTrace trace, string whatWasRunning, string fullError, string message, string stackTrace, Thread runningThread)
    {
        StackFrame frame = trace.GetFrame(0);

        int fileLine = frame.GetFileLineNumber();
        int fileColumn = frame.GetFileColumnNumber();

        string fileName = frame.GetFileName();
        string methodName = frame.GetMethod().Name;

        Directory.CreateDirectory(Application.dataPath + "/crashreport");

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("---- Crash Report ----");

        sb.AppendLine("// " + new string[] {

            "who turned the lights off?",
            "what zaza was this server smoking?",
            "rip server, died before developing plot",
            "andrew tate is top g",
            "cancel kanye",
            "it's not a bug, its just an undocumented feature",
            "“Experience is the name everyone gives to their mistakes.” – Oscar Wilde"

        }[new Random().Next(0, 6)] + "\n");

        sb.AppendLine("time of error: " + DateTime.Now);

        sb.AppendLine("description: " + message + "\n");

        sb.AppendLine(fullError + "\n");

        sb.AppendLine("a full detailed walkthrough of the error, its code path and all the details is as follows:");
        sb.AppendLine("------------------------------------------------------------------------------------------ \n");

        sb.AppendLine("what was running: " + whatWasRunning + "\n");

        sb.AppendLine("running thread: " + runningThread.Name);

        sb.AppendLine("stacktrace:");
        sb.AppendLine(methodName + "(" + fileName + ":line " + fileLine + " column " + fileColumn + ")\n");

        if(Server.currentInstance != null)
        {
            sb.AppendLine("all players connected at the time the server crashed:");
            sb.AppendLine("----------------------------------------------------- \n");

            foreach (Client client in Server.currentInstance.clients)
            {
                sb.AppendLine("ip: " + client.GetClient().Client.RemoteEndPoint + ", username: " + client.getPlayerName() + ", guid: " + client.getPlayerGuid() + ", coordinates as of the server crashing: [" + client.GetXPos() + "," + client.GetYPos() + "," + client.GetZPos() + "]");
            }
        }

        sb.AppendLine();

        sb.AppendLine("end report");

        File.WriteAllText(Application.dataPath + "/crashreport/recentcrash.txt", sb.ToString());

        Debug.Log("generated crash report for crash at " + DateTime.Now);
    }
}
