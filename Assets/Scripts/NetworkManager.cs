using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private bool IsServerRunning = false;
    private Thread _serverThread = null;
    private Server _server = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!IsServerRunning)
            {
                StartServer();
            }
            else
            {
                StopServer();
            }
        }
    }

    void FixedUpdate()
    {
        if (IsServerRunning)
        {
            if (_server.IsFinished())
            {
                IsServerRunning = false;
            }
        }
    }

    private void StartServer()
    {
        Debug.Log("[SERVER] Starting server.");
        IsServerRunning = true;
        _server = new Server();
        ThreadStart threadStart = new ThreadStart(_server.Process);
        _serverThread = new Thread(threadStart);
        _serverThread.Start();
    }

    private void StopServer()
    {
        Debug.Log("[SERVER] Stopping server.");
        _server.PleaseStop();
    }
}

