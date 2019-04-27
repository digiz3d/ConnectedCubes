using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Server
{
    public const byte SERVER_ID = 254;

    private Dictionary<byte, ServerClient> clientListById;
    private bool _needsToStop = false;
    private bool _isRunning = false;
    private IdsManager idsManager;

    public void Process()
    {
        _isRunning = true;
        _needsToStop = false;

        idsManager = new IdsManager();
        clientListById = new Dictionary<byte, ServerClient>();
        TcpListener listener = new TcpListener(IPAddress.Any, 8000);
        listener.Start();

        while (true)
        {
            if (_needsToStop)
            {
                Debug.Log("[SERVERTHREAD] Is stopping.");
                break;
            }

            if (listener.Pending())
            {
                Debug.Log("[SERVERTHREAD] Accepting new connection.");
                TcpClient tcpClient = listener.AcceptTcpClient();
                try
                {
                    byte newClientid = idsManager.CreateId();
                    clientListById.Add(newClientid, new ServerClient(tcpClient, newClientid));
                    Debug.Log("[SERVERTHREAD] New client has the id :" + newClientid);
                }
                catch (Exception e)
                {
                    Debug.Log("[SERVERTHREAD] Exception :" + e.Message);
                }

            }

            Debug.Log("[SERVERTHREAD] Processing...");
            Thread.Sleep(1);
        }
        _isRunning = false;
    }

    public void PleaseStop()
    {
        _needsToStop = true;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }
}
