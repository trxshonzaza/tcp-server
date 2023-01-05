using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class Sender
{
    private string senderName;

    public Sender(string senderName)
    {
        this.senderName = senderName;
    }

    public Client GetClient()
    {
        Server server = Server.currentInstance;

        foreach (Client client in server.clients)
        {
            if(client.getPlayerName() == senderName)
            {
                return client;
            }
        }

        return null;
    }

    public string GetSenderName()
    {
        return senderName;
    }
}