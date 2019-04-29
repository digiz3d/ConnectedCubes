using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client
{
    private bool _needsToStop = false;

    private bool _isConnected = false;

    private Queue<Packet> packetsQueue;
    private string _serverIp;
    private int _serverPort;

    public Client(string serverIp, int serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
        packetsQueue = new Queue<Packet>();
    }

    public void Process()
    {
        _needsToStop = false;

        Debug.Log("[CLIENT] Started processing.");

        TcpClient _tcpClient = new TcpClient(_serverIp, _serverPort);

        while (!_tcpClient.Connected)
        {
            Debug.Log("[CLIENT] Waiting for the server to approve connection.");
            Thread.Sleep(1);
        }

        _isConnected = true;

        NetworkStream networkStream = _tcpClient.GetStream();
        byte[] headerBuffer = new byte[NetworkPacket.HEADER_SIZE];
        byte[] dataBuffer;
        while (!_needsToStop)
        {
            if (networkStream.CanRead && networkStream.DataAvailable)
            {
                networkStream.Read(headerBuffer, 0, NetworkPacket.HEADER_SIZE);
                ushort packetLength = BitConverter.ToUInt16(headerBuffer, 2);
                dataBuffer = new byte[packetLength - NetworkPacket.HEADER_SIZE];
                networkStream.Read(dataBuffer, 0, packetLength - NetworkPacket.HEADER_SIZE);
                NetworkPacket p = new NetworkPacket(headerBuffer, dataBuffer);
                Packet packet = p.ToPacket();
                switch ((PacketType)packet.header.type)
                {
                    case PacketType.MOVE:
                        MovePacket mv = (MovePacket)packet;
                        Debug.Log("position = " + mv.position + ", rotation = " + mv.rotation);
                        break;

                    case PacketType.TEST:
                        TestPacket test = (TestPacket)packet;
                        Debug.Log("test packet ok");
                        break;

                    default:
                        Debug.Log("empty packet received");
                        break;
                }
                Debug.Log(packet);
            }

            if (networkStream.CanWrite && packetsQueue.Count > 0)
            {
                Packet packet = packetsQueue.Dequeue();
                NetworkPacket toSendPacket = packet.ToNetworkPacket();
                byte[] bytes = toSendPacket.ToBytes();
                networkStream.Write(bytes, 0, bytes.Length);
            }

            Debug.Log("[CLIENTTHREAD] Processing...");
            Thread.Sleep(1);
        }

        Debug.Log("[CLIENTTHREAD] Is stopping.");
        _tcpClient.Close();
        _isConnected = false;
    }

    public void PleaseDisconnect()
    {
        _needsToStop = true;
    }

    public void Disconnect()
    {
        _isConnected = false;
    }

    public bool IsConnected()
    {
        return _isConnected;
    }

    public void SendNetworkTestMessage()
    {
        packetsQueue.Enqueue(new TestPacket(69, 0f));
    }

    public void SendMovePacket(Vector3 pos, Vector3 rot)
    {
        Debug.Log("[CLIENTTHREAD] sending pos=" + pos + ", rot=" + rot);
        packetsQueue.Enqueue(new MovePacket(69, pos, rot));
    }
}
