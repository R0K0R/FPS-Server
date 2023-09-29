using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<ushort, Projectile> list = new Dictionary<ushort, Projectile>();

    [SerializeField] private float gravity;
    [SerializeField] private float damage;
    [SerializeField] protected float shotSpeed;

    private ushort id;
    private Player shooter;
    private float gravityAcceleration;
    private Vector3 velocity;

    private void Start()
    {
        gravityAcceleration = gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    private void Awake()
    {
        ushort id = NextId;
        this.name = $"Projectile {id}";
        this.id = id;
        this.velocity = this.gameObject.transform.forward;

        list.Add(id, this);
    }

    public void Setting(Player shooter, float damage)
    {
        this.shooter = shooter;
        this.damage = damage;
    }

    private void FixedUpdate()
    {
        velocity.y += gravityAcceleration;
        Vector3 nextPosition = transform.position + velocity;
        
        if (Physics.Raycast(transform.position, velocity.normalized, out RaycastHit hitInfo, velocity.magnitude))
        {
            Player hitPlayer = hitInfo.collider.GetComponent<Player>();
            if (hitPlayer == null || shooter.team == hitPlayer.team) // Hit a non player or same team
            {
                Hit(hitInfo);
                return;
            }
            else if (hitPlayer.Id != shooter.Id) // Hit another player
            {
                Hit(hitInfo, hitPlayer);
                return;
            }
        }

        transform.position = nextPosition;
        SendMovement();
    }

    private void Collide(Vector3 position)
    {
        transform.position = position;
        SendCollided();
        Destroy(gameObject);
    }

    private void Hit(RaycastHit hitInfo)
    {
        if (shooter.activeGun is TeleporterGun)
            shooter.Movement.Teleport(hitInfo.point + hitInfo.normal);

        Collide(hitInfo.point);
    }
    private void Hit(RaycastHit hitInfo, Player player)
    {
        Collide(hitInfo.point);

        if(shooter.activeGun.hit(hitInfo, player)) SendHitmarker();
    }

    private void OnDestroy()
    {
        list.Remove(id);
    }

    public static void Spawn(Player shooter, float damage, Vector3 position, Vector3 initialVelocity)
    {
        Projectile projectile;
        projectile = Instantiate(shooter.activeGun.bullet, position, Quaternion.LookRotation(initialVelocity)).GetComponent<Projectile>();

        ushort id = NextId;
        projectile.name = $"Projectile {id}";
        projectile.id = id;
        projectile.shooter = shooter;
        projectile.velocity = initialVelocity;
        projectile.damage= damage;

        projectile.SendSpawned();
        list.Add(id, projectile);
    }

    private static ushort _nextId;
    private static ushort NextId
    {
        get => _nextId++;
    }

    #region Messages
    private void SendSpawned()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.projectileSpawned);
        message.AddUShort(id);
        message.AddInt(Guns.getGunIndex(shooter.activeGun));
        message.AddUShort(shooter.Id);
        message.AddVector3(transform.position);
        message.AddVector3(transform.forward);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void SendMovement()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.projectileMovement);
        message.AddUShort(id);
        message.AddUShort(NetworkManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void SendCollided()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.projectileCollided);
        message.AddUShort(id);
        message.AddVector3(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void SendHitmarker()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.projectileHitmarker);
        NetworkManager.Singleton.Server.Send(message, shooter.Id);
    }
    #endregion
}
