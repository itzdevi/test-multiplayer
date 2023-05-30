using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    private static Prefabs _singleton;
    public static Prefabs Singleton
    {
        get => _singleton;
        set
        {
            if (_singleton == null)
                _singleton = value;
            else
                Destroy(value);
        }
    }

    public Player player;
    public Player localPlayer;

    [Header("Guns")]
    public WeaponSO beck;
    public WeaponSO carbon;
    public WeaponSO lightbreaker;
    public WeaponSO screech;
    public WeaponSO skullSmasher;

    [Header("Visuals")]
    public TrailRenderer bullet;
    public Renderer bulletHole;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(this);
    }
}
