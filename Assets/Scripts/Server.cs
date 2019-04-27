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

    private Dictionary<byte, ServerClient> _clientListById;
    private bool _needsToStop = false;
    private bool _isRunning = false;
    private IdsManager _idsManager;

    private Queue<NetworkPacket> packetsQueue;

    public Server()
    {
        packetsQueue = new Queue<NetworkPacket>();
    }

    public void Process()
    {
        _isRunning = true;
        _needsToStop = false;

        _idsManager = new IdsManager();
        _clientListById = new Dictionary<byte, ServerClient>();
        TcpListener listener = new TcpListener(IPAddress.Any, 8000);
        listener.Start();

        while (!_needsToStop)
        {
            if (listener.Pending())
            {
                Debug.Log("[SERVERTHREAD] Accepting new connection.");
                TcpClient tcpClient = listener.AcceptTcpClient();
                try
                {
                    byte newClientid = _idsManager.CreateId();
                    _clientListById.Add(newClientid, new ServerClient(tcpClient, newClientid));
                    Debug.Log("[SERVERTHREAD] New client has the id :" + newClientid);
                }
                catch (Exception e)
                {
                    Debug.Log("[SERVERTHREAD] Exception :" + e.Message);
                }
            }

            List<byte> ids = _idsManager.GetIds();
            foreach (byte id in ids)
            {
                _clientListById[id].ReadNextPacket(packetsQueue);
            }

            if (packetsQueue.Count > 0)
            {
                NetworkPacket networkPacket = packetsQueue.Dequeue();
                foreach (byte id in ids)
                {
                    _clientListById[id].SendPacket(networkPacket);
                }
            }

            Debug.Log("[SERVERTHREAD] Processing...");
            Thread.Sleep(1);
        }

        Debug.Log("[SERVERTHREAD] Is stopping.");

        listener.Stop();
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
