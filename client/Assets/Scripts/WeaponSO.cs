using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject {
    [Header("Display Data")]
    public string title;
    public Weapon prefab;

    public Weapon CreatePhysicalWeapon(Player owner) {
        Weapon weapon = Instantiate(prefab, owner.weaponContainer.position, owner.transform.rotation * Quaternion.Euler(0, -90, 0), owner.weaponContainer);
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