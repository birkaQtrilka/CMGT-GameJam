using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject bulletInstance;
    public float cooldownMax;
    float cooldown;
    public float shootingSpeed;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown < 0)
        {
            cooldown = cooldownMax;
            Shoot();
        }
    }
    void Shoot()
    {
        Vector3 dir = transform.rotation * Vector3.forward;
        GameObject instance = Instantiate(bulletInstance);
        instance.transform.position = transform.position;
        if (instance.GetComponent<Projectile>() == null)
            instance.AddComponent<Projectile>();

        Projectile projectile = instance.GetComponent<Projectile>();
        projectile.velocity = dir * shootingSpeed;
    }
}
