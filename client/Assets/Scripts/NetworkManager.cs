using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;

public enum ClientToServer {
    connect,
    input
}
public enum ServerToClient {
    join,
    playerUpdate,
    gunShoot
}
public class NetworkManager : MonoBehaviour {
    private static NetworkManager _singleton;
    public static NetworkManager Singleton {
        get => _singleton;
        set {
            if (_singleton == null)
                _singleton = value;
            else
                Destroy(value);
        }
    }

    public Client Client { get; private set; }

    [SerializeField] string host = "127.0.0.1";
    [SerializeField] ushort port = 5635;

    private void Awake() {
        Singleton = this;
        DontDestroyOnLoad(this);
    }

    private void Start() {
        RiptideLogger.Initialize(Debug.Log, false);

        Client = new Client();
        Client.Disconnected += Disconnected;
        Client.ConnectionFailed += ConnectionFailed;
        Client.ClientDisconnected += ClientDisconnected;
    }

    private void FixedUpdate() {
        Client.Update();
    }

    private void OnApplicationQuit() {
        Client.Disconnect();
    }

    public void Connect() {
        Client.Connect($"{host}:{port}");
    }

    private void Disconnected(object sender, DisconnectedEventArgs args) {
        SceneManager.Singleton.SetScene(SceneManager.Scene.mainMenu);
        Player.ClearPlayersList();
    }

    private void ConnectionFailed(object sender, ConnectionFailedEventArgs args) {
        SceneManager.Singleton.SetScene(SceneManager.Scene.mainMenu);
    }

    private void ClientDisconnected(object sender, ClientDisconnectedEventArgs args) {
        Player.RemovePlayer(args.Id);
    }
}
