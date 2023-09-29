using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerHealthRegenerator : MonoBehaviour
{
    [SerializeField] private float amount = 5;

    public void Start()
    {
        InvokeRepeating("regenerate", 0f, 0.5f);
    }

    void regenerate()
    {
        foreach (Player player in Player.list.Values)
        {
            if (player.timeFromLastHurt > 2f && player.health <= player.maxHealth - amount && player.IsAlive)
            {
                player.health += amount;
                Debug.Log("Health Generated For: " + player.name);
                player.SendHealthChanged();
            }
        }
    }
}
