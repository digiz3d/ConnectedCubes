using UnityEngine;

public static class NetworkPacketFactory
{
    public static NetworkPacket CreateFromBytes(byte[] header, byte[] data)
    {
        return new NetworkPacket(header, data);
    }

    public static NetworkPacket CreateFromBytes(byte[] bytes)
    {
        return new NetworkPacket(bytes);
    }

    public static NetworkPacket CreateMovePacket(byte sender, Vector3 pos, Vector3 rot)
    {
        return new NetworkPacket((byte)PacketType.MOVE, sender)
            .Add(pos)
            .Add(rot);
    }

    public static NetworkPacket CreateTestPacket(byte sender, float f)
    {
        return new NetworkPacket((byte)PacketType.TEST, sender)
            .Add(f);
    }
}

public enum PacketType : byte
{
    GIVE_ID,
    TEST,
    MOVE,
    LOGOUT,
}