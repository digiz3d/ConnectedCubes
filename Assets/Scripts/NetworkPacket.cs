using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacket
{
    public const ushort MAX_SIZE = 1500;
    public const ushort HEADER_SIZE = 4;

    private byte[] _buffer = new byte[MAX_SIZE];
    private byte[] _data = new byte[HEADER_SIZE];
    private ushort _actualLength = HEADER_SIZE;

    public NetworkPacket(byte[] bytes)
    {
        _data = new byte[bytes.Length];
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
        return this.Add(v.x).Add(v.y).Add(v.z);
    }

    public NetworkPacket Add(float f)
    {
        ushort newLength = (ushort)(_actualLength + sizeof(float));
        if (newLength > MAX_SIZE)
        {
            throw new Exception("Paquet would be aboce max allowed size :" + newLength + "/" + MAX_SIZE);
        }
        byte[] buffer = BitConverter.GetBytes(f);
        addData(buffer);
        return this;
    }

    private void addData(byte[] data)
    {
        ushort newLength = (ushort)(_actualLength + data.Length);
        Array.Resize(ref _data, _actualLength + data.Length);
        Buffer.BlockCopy(data, 0, _data, _actualLength, data.Length);
        _actualLength = newLength;
    }

    public byte[] GetBytes()
    {
        byte[] len = BitConverter.GetBytes(GetLength());
        _data[2] = len[0];
        _data[3] = len[1];
        return this._data;
    }

    public ushort GetLength()
    {
        return _actualLength;
    }
}
