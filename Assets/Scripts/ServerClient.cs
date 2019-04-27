using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;


public class ServerClient
{
    private TcpClient _tcpClient;
    private byte _id;

    public ServerClient(TcpClient tcpClient, byte id)
    {
        _tcpClient = tcpClient;
        _id = id;
    }
}
