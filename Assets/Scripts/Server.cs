using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Server
{
    private Dictionary<byte, ServerClient> _clientListById;
    private bool _needsToStop = false;
    private bool _isRunning = false;
    private IdsManager _idsManager;

    private Queue<Packet> _packetsQueue;

    public Server()
    {
        _packetsQueue = new Queue<Packet>();
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
                _clientListById[id].ReadNextPacket(_packetsQueue);
            }

            while (_packetsQueue.Count > 0)
            {
                Packet packet = _packetsQueue.Dequeue();
                foreach (byte id in ids)
                {
                    _clientListById[id].SendPacket(packet);
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
