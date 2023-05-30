using UnityEngine;
using Riptide;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour, IDamagable {
    public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    public Transform Head { get; private set; }

    public Transform weaponContainer;

    [SerializeField] private float moveSpeed = 2;
    
    private Rigidbody rb;
    private Weapon weapon;
    private HealthSystem healthSystem;

    public static void RemovePlayer(ushort id) {
        Player player = players[id];
        Destroy(player.gameObject);
        players.Remove(id);
    }

    public int GetHealth() {
        return healthSystem.GetHealth();
    }

    public void Heal(int amount) {
        healthSystem.Heal(amount);
    }

    public void Damage(int amount) {
        healthSystem.Damage(amount);
    }

    private void FixedUpdate() {
        foreach (Player player in players.Values)
            player.SendUpdateMessage();
    }

    private void OnPlayerDead(object sender, EventArgs args) {
        NetworkManager.Singleton.Server.DisconnectClient(Id);
    }

    private static void SpawnPlayer(ushort id, string username, ushort weaponIndex) {
        Player player = Instantiate(Prefabs.Singleton.player, Vector3.zero, Quaternion.identity);
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? "Guest" : username;
        player.rb = player.GetComponent<Rigidbody>();
        player.Head = player.transform.Find("Head");
        player.healthSystem = new HealthSystem(150);
        player.healthSystem.OnDead += player.OnPlayerDead;

        player.name = $"({player.Id}) {player.Username}";

        player.weapon = WeaponSO.GetWeaponByIndex(weaponIndex).CreatePhysicalWeapon(player);

        player.SendJoinMessage();

        foreach (Player other in players.Values) {
            other.SendJoinMessage(player);
        }

        players.Add(id, player);
    }

    private void Move(Vector2 input) {
        Vector3 velocity = (transform.forward * input.y + transform.right * input.x) * moveSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void SendJoinMessage() {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.join);
        msg.AddUShort(Id);
        msg.AddString(Username);
        msg.AddVector3(transform.position);
        msg.AddQuaternion(transform.rotation);
        msg.AddUShort(WeaponSO.GetIndexFromWeapon(weapon));

        NetworkManager.Singleton.Server.SendToAll(msg);
    }

    private void SendJoinMessage(Player to) {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.join);
        msg.AddUShort(Id);
        msg.AddString(Username);
        msg.AddVector3(transform.position);
        msg.AddQuaternion(transform.rotation);
        msg.AddUShort(WeaponSO.GetIndexFromWeapon(weapon));

        NetworkManager.Singleton.Server.Send(msg, to.Id);
    }
    
    private void SendUpdateMessage() {
        Message msg = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClient.playerUpdate);
        msg.AddUShort(Id);
        msg.AddVector3(transform.position);
        msg.AddQuaternion(transform.rotation);
        msg.AddQuaternion(Head.localRotation);

        NetworkManager.Singleton.Server.SendToAll(msg);
    }

    [MessageHandler((ushort)ClientToServer.connect)]
    private static void Connect(ushort id, Message msg) {
        string username = msg.GetString();
        ushort weaponIndex = msg.GetUShort();
        SpawnPlayer(id, username, weaponIndex);
    }

    [MessageHandler((ushort)ClientToServer.input)]
    private static void Input(ushort id, Message msg) {
        Player player = players[id];

        Vector2 moveInput = msg.GetVector2();
        Quaternion playerRotation = msg.GetQuaternion();
        Quaternion headRotation = msg.GetQuaternion();
        bool isShooting = msg.GetBool();

        player.Move(moveInput);
        player.transform.rotation = playerRotation;
        player.Head.transform.localRotation = headRotation;

        if (isShooting == true) {
            player.weapon.HandleShoot();
        }
        else {
            player.weapon.ResetShot();
        }
    }
}
