using UnityEngine;

public enum PacketType : byte
{
    GIVE_ID,
    TEST,
    MOVE,
    LOGOUT,
}

public struct PacketHeader
{
    public byte type;
    public byte sender;

    public PacketHeader(byte type, byte sender)
    {
        this.type = type;
        this.sender = sender;
    }
}

public class Packet
{
    public PacketHeader header;

    public Packet(PacketType packetType, byte sender)
    {
        header = new PacketHeader((byte)packetType, sender);
    }

    public virtual NetworkPacket ToNetworkPacket()
    {
        return new NetworkPacket(header.type, header.sender);
    }
}

public class MovePacket : Packet
{
    public Vector3 position;
    public Vector3 rotation;

    public MovePacket(byte sender, Vector3 position, Vector3 rotation) : base(PacketType.MOVE, sender)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public override NetworkPacket ToNetworkPacket()
    {
        return base.ToNetworkPacket()
            .Add(position)
            .Add(rotation);
    }
}

public class TestPacket : Packet
{
    public float testValue;

    public TestPacket(byte sender, float testValue) : base(PacketType.TEST, sender)
    {
        this.testValue = testValue;
    }

    public override NetworkPacket ToNetworkPacket()
    {
        return base.ToNetworkPacket()
            .Add(testValue);
    }
}