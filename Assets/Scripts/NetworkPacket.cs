using System;
using UnityEngine;

public class NetworkPacket
{
    public const ushort HEADER_SIZE = 4;

    private byte[] _bytes = new byte[HEADER_SIZE];
    private ushort _length = HEADER_SIZE;

    public NetworkPacket(byte[] header, byte[] data)
    {
        int newLength = header.Length + data.Length;

        Array.Resize(ref _bytes, newLength);
        Buffer.BlockCopy(header, 0, _bytes, 0, HEADER_SIZE);
        Buffer.BlockCopy(data, 0, _bytes, HEADER_SIZE, data.Length - HEADER_SIZE);
        _length = (ushort)newLength;
    }

    public NetworkPacket(byte type, byte sender)
    {
        _bytes[0] = type;
        _bytes[1] = sender;
        /*
        byte[] packetLength = BitConverter.GetBytes(_length);
        _bytes[2] = packetLength[0];
        _bytes[3] = packetLength[1];
        */
        _length = 4;
    }

    public NetworkPacket Add(Vector3 v)
    {
        return Add(v.x).Add(v.y).Add(v.z);
    }

    public NetworkPacket Add(float f)
    {
        byte[] buffer = BitConverter.GetBytes(f);
        return Add(buffer);
    }

    private NetworkPacket Add(byte[] buffer)
    {
        ushort newLength = (ushort)(_length + buffer.Length);
        Array.Resize(ref _bytes, newLength);
        Buffer.BlockCopy(buffer, 0, _bytes, _length, buffer.Length);
        _length = newLength;
        Debug.Log("length: " + _length);
        return this;
    }

    public byte[] GetBytes()
    {
        byte[] len = BitConverter.GetBytes(GetLength());
        _bytes[2] = len[0];
        _bytes[3] = len[1];
        return _bytes;
    }

    public ushort GetLength()
    {
        return _length;
    }

    public Packet ToPacket()
    {
        return PacketFactory.GetPacket(_bytes);
    }
}
