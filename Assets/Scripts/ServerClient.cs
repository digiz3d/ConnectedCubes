using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


public class ServerClient
{
    private TcpClient _tcpClient;
    private byte _id;
    private NetworkStream _networkStream;

    public ServerClient(TcpClient tcpClient, byte id)
    {
        _tcpClient = tcpClient;
        _id = id;
        _networkStream = _tcpClient.GetStream();
    }

    public void PrintId()
    {
        Debug.Log("[ServerClient] his id is = " + _id);
    }

    public NetworkStream GetNetworkStream()
    {
        return _networkStream;
    }

    public void ReadNextPacket(Queue<NetworkPacket> packetQueue)
    {
        if (_networkStream.DataAvailable && _networkStream.CanRead)
        {
            byte[] headerBuffer = new byte[NetworkPacket.HEADER_SIZE];
            _networkStream.Read(headerBuffer, 0, NetworkPacket.HEADER_SIZE);
            ushort packetLength = BitConverter.ToUInt16(headerBuffer, 2);
            byte[] dataBuffer = new byte[packetLength - NetworkPacket.HEADER_SIZE];
            _networkStream.Read(dataBuffer, 0, packetLength - NetworkPacket.HEADER_SIZE);
            
            byte[] originalPacket = new byte[packetLength];
            Buffer.BlockCopy(headerBuffer, 0, originalPacket, 0, NetworkPacket.HEADER_SIZE);
            Buffer.BlockCopy(dataBuffer, 0, originalPacket, NetworkPacket.HEADER_SIZE, packetLength - NetworkPacket.HEADER_SIZE);
            packetQueue.Enqueue(NetworkPacketFactory.CreateFromBytes(originalPacket));
        }
    }

    public void SendPacket(NetworkPacket networkPacket)
    {
        if (_networkStream.CanWrite)
        {
            byte[] buffer = networkPacket.GetBytes();
            _networkStream.Write(buffer, 0, buffer.Length);
        }
    }
}
