using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class IdsManager
{
    public const byte SERVER_ID = 254;
    public const byte NOBODY_ID = 255;

    private List<byte> _ids;
    private byte _maxPlayers = 253;

    public IdsManager()
    {
        _ids = new List<byte>();
    }

    public byte CreateId()
    {
        if (_ids.Count >= _maxPlayers)
        {
            throw new Exception("[IdsManager] Too many players: " + _ids.Count + ">=" + _maxPlayers);
        }

        for (byte i = 1; i <= _maxPlayers; i++)
        {
            if (!_ids.Contains(i))
            {
                // Debug.Log("[IdsManager] Generated id :" + i);
                _ids.Add(i);
                return i;
            }
        }

        throw new Exception("[IdsManager] No free Id available. Too many players.");
    }

    public void FreeId(byte id)
    {
        _ids.Remove(id);
    }

    public List<byte> GetIds()
    {
        return _ids;
    }

    public void SetMaxPlayers(byte maxPlayers)
    {
        _maxPlayers = maxPlayers;
    }
}
