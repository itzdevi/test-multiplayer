using UnityEngine;

public class SceneManager : MonoBehaviour {
    private static SceneManager _singleton;
    public static SceneManager Singleton {
        get => _singleton;
        set {
            if (_singleton == null)
                _singleton = value;
            else
                Destroy(value);
        }
    }

    private void Awake() {
        Singleton = this;
    }

    public void SetScene(Scene scene) {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)scene);
    }

    public enum Scene {
        mainMenu,
        lobby,
    }
}
