using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Server
{
    private bool _needsToStop = false;
    private bool _isFinished = false;
    public void Process()
    {
        while (true)
        {
            if (_needsToStop)
            {
                Debug.Log("[SERVERTHREAD] Is stopping.");
                break;
            }
            Debug.Log("[SERVERTHREAD] Processing...");
            Thread.Sleep(500);
        }
        _isFinished = true;
    }

    public void PleaseStop()
    {
        _needsToStop = true;
    }
    public bool isFinished()
    {
        return _isFinished;
    }
}
