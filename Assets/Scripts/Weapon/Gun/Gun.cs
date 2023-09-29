using Riptide;
using UnityEngine;

public enum WeaponType : byte
{
    none,
    pistol,
    teleporter,
    laser
}

public abstract class Gun : MonoBehaviour
{
    [SerializeField] protected WeaponType type;
    [SerializeField] protected float shotSpeed;
    [SerializeField] protected byte loadedAmmo;
    [SerializeField] protected byte maxLoadedAmmo;
    [SerializeField] protected ushort totalAmmo = 1000;
    [SerializeField] protected float cooldown;
    [SerializeField] protected float reloadTime;
    [SerializeField] public GameObject bullet;
    [SerializeField] protected float damage;
    [SerializeField] protected GameObject model;

    public Player shooter;
    public string gunType;
    protected ushort maxTotalAmmo;
    protected float timeBeforeNextShot = 0;
    public bool isReloading = false;

    public Gun(GameObject prefab)
    {
        
    }

    private void OnValidate()
    {
        
    }

    protected void Start()
    {
        maxTotalAmmo = totalAmmo;
    }

    public abstract void Shoot();

    public void Reload()
    {
        if (totalAmmo > 0)
        {
            byte amountToReload = (byte)Mathf.Min(maxLoadedAmmo - loadedAmmo, totalAmmo);
            loadedAmmo += amountToReload;
            totalAmmo -= amountToReload;
            SendAmmoUpdated();
        }
    }

    public abstract bool hit(RaycastHit hitInfo, Player player);

    public void ResetAmmo()
    {
        loadedAmmo = maxLoadedAmmo;
        totalAmmo = maxTotalAmmo;
        SendAmmoUpdated();
    }

    #region messages
    protected void SendAmmoUpdated()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.playerAmmoChanged);
        message.AddByte((byte)type);
        message.AddByte(loadedAmmo);
        message.AddUShort(totalAmmo);
        NetworkManager.Singleton.Server.Send(message, shooter.Id);
    }
    #endregion

    protected void FixedUpdate()
    {
        gunCoolDown(shooter.activeGun);
    }

    public static void gunCoolDown(Gun gun)
    {
        gun.timeBeforeNextShot -= 0.025f;
        if (gun.timeBeforeNextShot < 0)
        {
            gun.timeBeforeNextShot = 0;
        }
    }
}
