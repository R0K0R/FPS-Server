using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterGun : MultiShot
{
    public TeleporterGun(GameObject prefab) : base(prefab)
    {
        Gun script = prefab.GetComponentInChildren<Gun>();
        model = prefab;
        ((TeleporterGun)script).type = this.type;
        ((TeleporterGun)script).bullet = this.bullet;
        ((TeleporterGun)script).damage = this.damage;
        ((TeleporterGun)script).maxLoadedAmmo = this.maxLoadedAmmo;
        ((TeleporterGun)script).reloadTime = this.reloadTime;
        ((TeleporterGun)script).shotSpeed = this.shotSpeed;
        ((TeleporterGun)script).type = this.type;
    }

    public override bool hit(RaycastHit hitInfo, Player player)
    {
        shooter.Movement.Teleport(hitInfo.point + hitInfo.normal);
        return false;
    }
}
