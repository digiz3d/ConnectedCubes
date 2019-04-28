using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PacketFactory
{
    public static Packet GetPacket(byte[] buffer)
    {
        switch ((PacketType)buffer[0])
        {
            case PacketType.MOVE:
                return GetMovePacket(ref buffer);
            case PacketType.TEST:
                return GetTestPacket(ref buffer);
        }
        return null;
    }

    public static MovePacket GetMovePacket(ref byte[] buffer)
    {
        return new MovePacket(GetHeader(ref buffer), GetVector3(ref buffer), GetVector3(ref buffer));
    }

    public static TestPacket GetTestPacket(ref byte[] buffer)
    {
        return new TestPacket(GetHeader(ref buffer), GetFloat(ref buffer));
    }

    public static Vector3 GetVector3(ref byte[] buffer)
    {
        return new Vector3(GetFloat(ref buffer), GetFloat(ref buffer), GetFloat(ref buffer));
    }

    /*
     * technical methods
     */
    public static ushort GetUShort(ref byte[] buffer)
    {
        ushort ret = (ushort)BitConverter.ToInt16(buffer, 0);
        ResizeArrayShifting(ref buffer, sizeof(ushort));
        return ret;
    }

    public static float GetFloat(ref byte[] buffer)
    {
        float ret = BitConverter.ToSingle(buffer, 0);
        ResizeArrayShifting(ref buffer, sizeof(float));
        return ret;
    }

    public static byte GetByte(ref byte[] buffer)
    {
        byte ret = buffer[0];
        ResizeArrayShifting(ref buffer, sizeof(byte));
        return ret;
    }

    public static PacketHeader GetHeader(ref byte[] buffer)
    {
        PacketHeader header = new PacketHeader(GetByte(ref buffer), GetByte(ref buffer), GetUShort(ref buffer));
        return header;
    }

    private static void ResizeArrayShifting(ref byte[] buffer, int bytesToRemove) {
        byte[] replacement = new byte[buffer.Length - bytesToRemove];
        Array.Copy(buffer, bytesToRemove, replacement, 0, buffer.Length - bytesToRemove);
        buffer = replacement;
    }

}

public struct PacketHeader
{
    public byte type;
    public byte sender;
    public ushort length;

    public PacketHeader(byte type, byte sender, ushort length)
    {
        this.type = type;
        this.sender = sender;
        this.length = length;
    }
}

public class Packet
{
    public PacketHeader header;

    public Packet(PacketHeader header)
    {
        this.header = header;
    }

    public NetworkPacket ToNetworkPacket()
    {
        return new NetworkPacket(header.type, header.sender);
    }
}

public class MovePacket : Packet
{
    public Vector3 position;
    public Vector3 rotation;

    public MovePacket(PacketHeader header, Vector3 position, Vector3 rotation) : base(header)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public NetworkPacket ToNetworkPacket(Vector3 position, Vector3 rotation)
    {
        return ToNetworkPacket().Add(position).Add(rotation);
    }
}

public class TestPacket : Packet
{
    public float testValue;

    public TestPacket(PacketHeader header, float testValue) : base(header)
    {
        this.testValue = testValue;
    }

    public NetworkPacket ToNetworkPacket(float testValue)
    {
        return ToNetworkPacket().Add(testValue);
    }
}