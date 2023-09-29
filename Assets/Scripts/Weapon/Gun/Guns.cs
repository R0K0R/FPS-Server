using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guns : MonoBehaviour
{
    public GameObject pistol;
    public GameObject laserGun;
    public GameObject teleporterGun;

    public static Dictionary<string, GameObject> guns = new Dictionary<string, GameObject>();

    private void Start()
    {
        register("pistol", pistol);
        register("laser", laserGun);
        register("telepoter", teleporterGun);
    }

    public static int getGunIndex(Gun gun)
    {
        return Array.FindIndex(Guns.guns.Values.ToArray(), item => item != null && item.GetComponentsInChildren<Gun>()[0].gunType == gun.gunType);
    }

    public static GameObject getGunbyIndex(int index) { return Guns.guns.Values.ToArray()[index]; }


    private void register(string name, GameObject gun)
    {
        gun.GetComponentsInChildren<Gun>()[0].gunType = name;
        guns.Add(name, gun);
    }

    public static GameObject getRandomGun()
    {
        System.Random random = new System.Random();

        int randomInt = random.Next(0, Guns.guns.Values.Count);

        return getGunbyIndex(randomInt);
    }

    public static GameObject gunToGameobject(Gun gun)
    {
        return getGunbyIndex(getGunIndex(gun));
    } 
}
