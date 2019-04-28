using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client
{
    private bool _needsToStop = false;

    private bool _isConnected = false;

    private Thread _clientProcessThread;
    private TcpClient _tcpClient;
    private Queue<NetworkPacket> packetsQueue;
    private string _serverIp;
    private int _serverPort;

    public Client(string serverIp, int serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
        packetsQueue = new Queue<NetworkPacket>();
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
                NetworkPacket p = NetworkPacketFactory.CreateFromBytes(headerBuffer, dataBuffer);
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
                NetworkPacket toSendPacket = packetsQueue.Dequeue();
                networkStream.Write(toSendPacket.GetBytes(), 0, toSendPacket.GetLength());
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
        packetsQueue.Enqueue(NetworkPacketFactory.CreateTestPacket(69, 0f));
    }

    public void SendMovePacket(Vector3 pos, Vector3 rot)
    {
        Debug.Log("[CLIENTTHREAD] sending pos=" + pos + ", rot=" + rot);
        packetsQueue.Enqueue(NetworkPacketFactory.CreateMovePacket(69, pos, rot));
    }
}
