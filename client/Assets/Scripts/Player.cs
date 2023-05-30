using UnityEngine;
using Riptide;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    public Weapon Weapon { get; private set; }

    public Transform weaponContainer;

    public static Player LocalPlayer { get; private set; }

    [SerializeField] private float sensitivity = 50f;


    private Transform head;
    private float xRotation = 0;

    private static PlayerInputActions _inputActions;
    public static PlayerInputActions InputActions {
        get => _inputActions;
        set {
            if (_inputActions == null)
                _inputActions = value;
                _inputActions.Player.Enable();
        }
    }

    private void Update() {
        if (LocalPlayer == this) {
            SendInputsMessage();
            Rotate();
        }
    }

    public static void ClearPlayersList() {
        foreach (Player player in Player.players.Values)
            Destroy(player.gameObject);
        Player.players.Clear();
    }

    public static void RemovePlayer(ushort id) {
        Player player = players[id];
        Destroy(player.gameObject);
        players.Remove(id);
    }

    private static void SpawnPlayer(ushort id, string username, Vector3 position, Quaternion rotation, ushort weaponIndex) {
        Player player;

        if (id == NetworkManager.Singleton.Client.Id) {
            player = Instantiate(Prefabs.Singleton.localPlayer, position, rotation);
            LocalPlayer = player;
            InputActions = new PlayerInputActions();
        }
        else {
            player = Instantiate(Prefabs.Singleton.player, position, rotation);
        }

        player.Id = id;
        player.Username = username;
        player.head = player.transform.Find("Head");

        player.name = $"({id}) {username}";

        if (player == LocalPlayer) {
            Camera.main.transform.parent = player.head;
            Camera.main.transform.localPosition = Vector3.zero;
        }

        player.Weapon = WeaponSO.GetWeaponByIndex(weaponIndex).CreatePhysicalWeapon(player);

        players.Add(id, player);
    }

    private void SendInputsMessage() {
        Message msg = Message.Create(MessageSendMode.Unreliable, (ushort)ClientToServer.input);
        msg.AddVector2(InputActions.Player.Move.ReadValue<Vector2>());
        msg.AddQuaternion(transform.rotation);
        msg.AddQuaternion(head.localRotation);
        msg.AddBool(System.Convert.ToBoolean(InputActions.Player.Shoot.ReadValue<float>()));

        NetworkManager.Singleton.Client.Send(msg);
    }

    private void Rotate() {
        Vector2 input = InputActions.Player.Rotate.ReadValue<Vector2>();

        xRotation -= input.y * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.Rotate(transform.up * input.x * sensitivity * Time.deltaTime);
        head.localRotation = Quaternion.Euler(Vector3.right * xRotation);
    }

    private static void UpdatePosition(Player player, Vector3 position) {
        player.transform.position = position;
    }

    private static void UpdateRotation(Player player, Quaternion bodyRotation, Quaternion headRotation) {
        if (player == LocalPlayer) return;

        player.transform.rotation = bodyRotation;
        player.head.localRotation = headRotation;
    }

    [MessageHandler((ushort)ServerToClient.join)]
    private static void Join(Message msg) {
        ushort id = msg.GetUShort();
        string username = msg.GetString();
        Vector3 position = msg.GetVector3();
        Quaternion rotation = msg.GetQuaternion();
        ushort weaponIndex = msg.GetUShort();

        SpawnPlayer(id, username, position, rotation, weaponIndex);
    }

    [MessageHandler((ushort)ServerToClient.playerUpdate)]
    private static void PlayerUpdate(Message msg) {        
        ushort id = msg.GetUShort();
        Vector3 position = msg.GetVector3();
        Quaternion playerRotation = msg.GetQuaternion();
        Quaternion headRotation = msg.GetQuaternion();

        if (!players.ContainsKey(id)) return;
        UpdatePosition(players[id], position);
        UpdateRotation(players[id], playerRotation, headRotation);
    }
}
