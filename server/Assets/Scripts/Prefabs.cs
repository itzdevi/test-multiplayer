using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour {
    private static Prefabs _singleton;
    public static Prefabs Singleton {
        get => _singleton;
        set {
            if (_singleton == null)
                _singleton = value;
            else
                Destroy(value);
        }
    }

    public Player player;

    [Header("Guns")]
    public WeaponSO beck;
    public WeaponSO carbon;
    public WeaponSO lightbreaker;
    public WeaponSO screech;
    public WeaponSO skullSmasher;

    private void Awake() {
        Singleton = this;
        DontDestroyOnLoad(this);
    }
}
