using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPack : MonoBehaviourPun, IItem
{
    public float health = 50f;

    public void Use(GameObject target)
    {
        LivingEntity life = target.GetComponent<LivingEntity>();
        if (life != null)
        {
            life.RestoreHealth(health);
        }
        //Destroy(gameObject);
        //Debug.Log("체력을 회복 했다. : " + health);
        PhotonNetwork.Destroy(gameObject);
    }
}
