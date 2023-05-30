using UnityEngine;
using Riptide;
using Riptide.Utils;

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

    public Server Server;

    [SerializeField] ushort port = 5635;
    [SerializeField] ushort maxClients;
    [SerializeField] ushort tickRate = 128;

    private void Awake() {
        Singleton = this;
        DontDestroyOnLoad(this);

        Time.fixedDeltaTime = 1f / tickRate;
    }

    private void Start() {
        RiptideLogger.Initialize(Debug.Log, false);

        Server = new Server();

        Server.ClientDisconnected += ClientDisconnected;

        Server.Start(port, maxClients);
    }

    private void FixedUpdate() {
        Server.Update();
    }

    private void ClientDisconnected(object sender, ServerDisconnectedEventArgs args) {
        Player.RemovePlayer(args.Client.Id);
    }
}
