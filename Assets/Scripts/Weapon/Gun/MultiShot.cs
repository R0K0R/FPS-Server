using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiShot : Gun
{
    public MultiShot(GameObject prefab) : base(prefab)
    {
        Gun script = prefab.GetComponentInChildren<Gun>();
        model= prefab;
        ((MultiShot)script).type = this.type;
        ((MultiShot)script).bullet = this.bullet;
        ((MultiShot)script).damage = this.damage;
        ((MultiShot)script).maxLoadedAmmo= this.maxLoadedAmmo;
        ((MultiShot)script).reloadTime=this.reloadTime;
        ((MultiShot)script).shotSpeed=this.shotSpeed;
        ((MultiShot)script).type=this.type;
    }

    public override void Shoot()
    {
        if (timeBeforeNextShot != 0) return;

        timeBeforeNextShot = cooldown;

        if (loadedAmmo > 0 && shooter.IsAlive && shooter.activeGun != null)
        {
            Transform camProxy = shooter.transform.Find("CamProxy");
            Vector3 targetPoint = camProxy.position + camProxy.forward.normalized * 50000f;
            Vector3 shootDirection = targetPoint - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(camProxy.position, camProxy.forward, out hit))
            {
                targetPoint = hit.point;
                shootDirection = targetPoint - transform.position;
            }

            Projectile.Spawn(shooter, damage, transform.position, shootDirection * shotSpeed * Time.fixedDeltaTime);
            loadedAmmo--;
            base.SendAmmoUpdated();
        }
    }

    public override bool hit(RaycastHit hitInfo, Player player)
    {
        player.TakeDamage(damage);
        return true;
    }
}
