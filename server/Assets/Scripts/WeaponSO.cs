using UnityEngine;

public enum WeaponType {
    automatic,
    semiAutomatic,
}
public enum BulletType {
    light,
    medium,
    heavy,
    shell
}
[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject {
    [Header("Display Data")]
    public string title;
    public Weapon prefab;

    [Header("Enums")]
    public WeaponType fireMode;
    public BulletType bullet;

    [Header("Gun Data")]
    public int magazineSize;
    public float delayBetweenShots;
    [Tooltip("Amount of damage reduced per meter")] public float damageDropoff;
    [Min(1)] public float bulletsPerShot = 1;
    public int damage;
    public float headshotDamageMultiplier;
    public Vector2 spreadLimit;

    public Weapon CreatePhysicalWeapon(Player owner) {
        Weapon weapon = Instantiate(prefab, owner.weaponContainer.position, owner.transform.rotation * Quaternion.Euler(0, -90, 0), owner.weaponContainer);
        weapon.Owner = owner;
        return weapon;
    }

    public static WeaponSO GetWeaponByIndex(ushort index) {
        switch (index) {
            case (ushort)GunIndex.beck:
                return Prefabs.Singleton.beck;
            case (ushort)GunIndex.carbon:
                return Prefabs.Singleton.carbon;
            case (ushort)GunIndex.lightbreaker:
                return Prefabs.Singleton.lightbreaker;
            case (ushort)GunIndex.screech:
                return Prefabs.Singleton.screech;
            case (ushort)GunIndex.skullSmasher:
                return Prefabs.Singleton.skullSmasher;
            default:
                return null;
        }
    }

    public static ushort GetIndexFromWeapon(Weapon weapon) {
        if (weapon.GetSettings() == Prefabs.Singleton.beck)
            return ((ushort)GunIndex.beck);
        else if (weapon.GetSettings() == Prefabs.Singleton.carbon)
            return ((ushort)GunIndex.carbon);
        else if (weapon.GetSettings() == Prefabs.Singleton.lightbreaker)
            return ((ushort)GunIndex.lightbreaker);
        else if (weapon.GetSettings() == Prefabs.Singleton.screech)
            return ((ushort)GunIndex.screech);
        else if (weapon.GetSettings() == Prefabs.Singleton.skullSmasher)
            return ((ushort)GunIndex.skullSmasher);

        return ushort.MaxValue;
    }
}