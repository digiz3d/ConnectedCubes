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
        return new MovePacket(GetHeader(ref buffer).sender, GetVector3(ref buffer), GetVector3(ref buffer));
    }

    public static TestPacket GetTestPacket(ref byte[] buffer)
    {
        return new TestPacket(GetHeader(ref buffer).sender, GetFloat(ref buffer));
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
        PacketHeader header = new PacketHeader(GetByte(ref buffer), GetByte(ref buffer));
        GetUShort(ref buffer); // yeah... we lost this useless information
        return header;
    }

    private static void ResizeArrayShifting(ref byte[] buffer, int bytesToRemove)
    {
        byte[] replacement = new byte[buffer.Length - bytesToRemove];
        Array.Copy(buffer, bytesToRemove, replacement, 0, buffer.Length - bytesToRemove);
        buffer = replacement;
    }

}