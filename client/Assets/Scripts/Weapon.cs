using UnityEngine;
using Riptide;
using System.Collections;

public enum GunIndex {
    beck,
    carbon,
    lightbreaker,
    screech,
    skullSmasher
}
public class Weapon : MonoBehaviour {
    [SerializeField] WeaponSO settings;
    [SerializeField] Transform tip;

    public WeaponSO GetSettings() {
        return settings;
    }


    [MessageHandler((ushort)ServerToClient.gunShoot)]
    private static void GunShoot(Message msg) {
        ushort id = msg.GetUShort();
        Vector3 hitPosition = msg.GetVector3();
        Vector3 hitNormal = msg.GetVector3();
        bool hitWall = msg.GetBool();

        Player player = Player.players[id];
        
        player.Weapon.StartCoroutine(player.Weapon.SpawnShootVFX(hitPosition, hitNormal, 0.17f, hitWall));
    }

    IEnumerator SpawnShootVFX(Vector3 hitPosition, Vector3 hitNormal, float speed, bool hitWall) {
        TrailRenderer trail = Instantiate(Prefabs.Singleton.bullet, tip.position, Quaternion.identity);
        while (trail.transform.position != hitPosition) {
            trail.transform.position = Vector3.Lerp(trail.transform.position, hitPosition, speed / Vector3.Distance(trail.transform.position, hitPosition));
            yield return null;
        }

        if (hitWall) {
            Renderer bulletHole = Instantiate(Prefabs.Singleton.bulletHole, hitPosition, Quaternion.LookRotation(hitNormal));
            yield return new WaitForSeconds(2);
            
            Color currentColor = bulletHole.material.GetColor("_Color");
            while (currentColor.a > 0) {
                currentColor.a -= 0.01f;
                bulletHole.material.SetColor("_Color", currentColor);
                yield return null;
            }
            Destroy(bulletHole.gameObject);
        }
    }
}