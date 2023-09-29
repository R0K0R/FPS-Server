using Riptide;
using System;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject pistol;
    [SerializeField] private Gun teleporter;
    [SerializeField] private Gun laser;
    
    private WeaponType activeType;

    public void SetActiveWeapon(GameObject gun, Player player)
    {
        if (gun != null && player != null)
        {
            player.activeGun = gun.GetComponentsInChildren<Gun>()[0];
            player.activeGun.shooter = player;
            SendActiveWeaponUpdate(player);
        }
        else
        {
            Debug.LogError("Something is wrong");
        }
    }

    #region Messages

    private void SendActiveWeaponUpdate(Player player)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientId.playerActiveWeaponUpdated);
        message.AddUShort(player.Id);
        message.AddInt(Guns.getGunIndex(player.activeGun));
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    #endregion
}
