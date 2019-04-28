using System;
using UnityEngine;

public class NetworkPacket
{
    public const ushort HEADER_SIZE = 4;

    private byte[] _data = new byte[HEADER_SIZE];
    private ushort _actualLength = HEADER_SIZE;

    public NetworkPacket(byte[] bytes)
    {
        Array.Resize(ref _data, bytes.Length);
        Buffer.BlockCopy(bytes, 0, _data, 0, bytes.Length);
        _actualLength = (byte)bytes.Length;
    }

    public NetworkPacket(byte type, byte sender)
    {
        _data[0] = type;
        _data[1] = sender;
        byte[] packetLength = BitConverter.GetBytes(_actualLength);
        _data[2] = packetLength[0];
        _data[3] = packetLength[1];
        _actualLength = 4;
    }

    public NetworkPacket Add(Vector3 v)
    {
        return Add(v.x).Add(v.y).Add(v.z);
    }

    public NetworkPacket Add(float f)
    {
        byte[] buffer = BitConverter.GetBytes(f);
        return Add(buffer); ;
    }

    private NetworkPacket Add(byte[] buffer)
    {
        ushort newLength = (ushort)(_actualLength + buffer.Length);
        Array.Resize(ref _data, newLength);
        Buffer.BlockCopy(buffer, 0, _data, _actualLength, buffer.Length);
        _actualLength = newLength;
        return this;
    }

    public byte[] GetBytes()
    {
        byte[] len = BitConverter.GetBytes(GetLength());
        _data[2] = len[0];
        _data[3] = len[1];
        return _data;
    }

    public ushort GetLength()
    {
        return _actualLength;
    }
}
