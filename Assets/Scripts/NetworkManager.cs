using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private Server _server;
    private Client _client;
    public CharacterController character;

    private void Start()
    {
        _server = new Server();
        _client = new Client("localhost", 8000);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (_server.IsRunning() && !_client.IsConnected())
            {
                ConnectToServer();
            }
            else
            {
                DisconnectFromServer();
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_server.IsRunning() && _client.IsConnected())
            {
                //_client.SendMovePacket(character.transform.position);
                _client.SendNetworkTestMessage();
            }
        }
    }

    private void StartServer()
    {
        Debug.Log("[SERVER] Starting server.");
        
        Thread _serverThread = new Thread(_server.Process);
        _serverThread.Start();
    }

    private void StopServer()
    {
        Debug.Log("[SERVER] Stopping server.");

        _server.PleaseStop();
    }

    private void ConnectToServer()
    {
        Debug.Log("[CLIENT] Connecting to server.");

        Thread _clientThread = new Thread(_client.Process);
        _clientThread.Start();
    }

    private void DisconnectFromServer()
    {
        Debug.Log("[CLIENT] Disconnecting from server.");

        _client.PleaseDisconnect();
    }
}

