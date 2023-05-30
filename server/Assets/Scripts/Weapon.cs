using UnityEngine;
using Riptide;

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

    private bool isReloading;
    private bool shotSemiAuto;
    private float shotDelay;

    public Player Owner { get; set; }

    public WeaponSO GetSettings() {
        return settings;
    }

    public void HandleShoot() {
        if (isReloading) return;
        if (settings.fireMode == WeaponType.semiAutomatic && shotSemiAuto == true) return;
        if (shotDelay > 0) return;

        for (int i = 0; i < settings.bulletsPerShot; i++)
            Shoot();

        if (settings.fireMode == WeaponType.semiAutomatic)
            shotSemiAuto = true;
    }

    private void Update() {
        shotDelay -= Time.deltaTime;
        shotDelay = Mathf.Clamp(shotDelay, 0, float.MaxValue);
    }

    public void StopReloadingState() {
        isReloading = false;
    }

    public void ResetShot() {
        shotSemiAuto = false;
    }
    
    private void Shoot() {
        shotDelay = settings.delayBetweenShots;

        float randomX = Random.Range(-settings.spreadLimit.x, settings.spreadLimit.x);
        float randomY = Random.Range(-settings.spreadLimit.y, settings.spreadLimit.y);
        Vector3 direction = Owner.Head.forward + new Vector3(randomX, 0, randomY);
        RaycastHit[] data = Physics.RaycastAll(Owner.Head.position, direction, Mathf.Infinity);
        if (data.Length == 0) {
            SendShootMessage(direction.normalized * 100, Vector3.zero, false);
            return;
        }

        foreach (RaycastHit hit in data) {
            if (hit.transform == Owner.transform) continue;
            else if (hit.transform.TryGetComponent(out IDamagable damagable)) { 
                bool didHeadshot = hit.collider.transform.CompareTag("Head");

                float distance = Vector3.Distance(tip.position, data[0].point);
                float damageAmount = settings.damage - (distance * settings.damageDropoff);
                damageAmount = didHeadshot ? damageAmount * settings.headshotDamageMultiplier : damageAmount;

                damageAmount = Mathf.Clamp(damageAmount, 0, float.MaxValue);
                damagable.Damage((int)damageAmount);

                SendShootMessage(hit.point, hit.normal, false);
                return;
            }
            else {
                SendShootMessage(hit.point, hit.normal, true);
                return;
            }
        }
    }

    private void SendShootMessage(Vector3 hitPosition, Vector3 hitNormal, bool hitWall) {
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClient.gunShoot);
        message.AddUShort(Owner.Id);
        message.AddVector3(hitPosition);
        message.AddVector3(hitNormal);
        message.AddBool(hitWall);

        NetworkManager.Singleton.Server.SendToAll(message);
    }
}