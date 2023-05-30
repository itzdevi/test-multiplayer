using UnityEngine;
using Riptide;
using TMPro;

public class UIManager : MonoBehaviour {
    private static UIManager _singleton;
    public static UIManager Singleton {
        get => _singleton;
        set {
            if (_singleton == null)
                _singleton = value;
            else
                Destroy(value);
        }
    }

    [SerializeField] TMP_InputField hostInputField;
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_Dropdown weaponSelectionDropdown;

    private void Awake() {
        Singleton = this;
    }

    public void Connect() {
        NetworkManager.Singleton.Connect(hostInputField.text);
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServer.connect);
        msg.AddString(usernameInputField.text);
        msg.AddInt(weaponSelectionDropdown.value);

        NetworkManager.Singleton.Client.Send(msg);
        SceneManager.Singleton.SetScene(SceneManager.Scene.lobby);
    }
}
