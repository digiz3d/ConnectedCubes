using UnityEngine;

public static class NetworkPacketFactory
{
    public static NetworkPacket CreateFromBytes(byte[] bytes)
    {
        return new NetworkPacket(bytes);
    }

    public static NetworkPacket CreateMovePacket(byte sender, Vector3 pos, Vector3 rot)
    {
        return new NetworkPacket(sender, (byte)NetworkPacketType.MOVE)
            .Add(pos)
            .Add(rot);
    }

    public static NetworkPacket CreateTestPacket(byte sender, float f)
    {
        return new NetworkPacket(sender, (byte)NetworkPacketType.TEST)
            .Add(f);
    }
}

public enum NetworkPacketType : short
{
    GIVE_ID,
    TEST,
    MOVE,
    LOGOUT,
}