using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private Server _server;

    private void Start()
    {
        _server = new Server();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if(_server.IsRunning())
            {
                StopServer();
            }
            else
            {
                StartServer();
            }
        }
    }

    private void StartServer()
    {
        Debug.Log("[SERVER] Starting server.");
        
        ThreadStart threadStart = new ThreadStart(_server.Process);
        Thread _serverThread = new Thread(threadStart);
        _serverThread.Start();
    }

    private void StopServer()
    {
        Debug.Log("[SERVER] Stopping server.");
        _server.PleaseStop();
    }
}

