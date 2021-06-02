using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Turret : Vehicle
{
    private Quaternion aimRotation;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if(aiming) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, weaponAimSpeed);
            getLimits();
            //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
            Quaternion inverseAimRotation = new Quaternion(-aimRotation.x, -aimRotation.y, -aimRotation.z, -aimRotation.w);
            if(transform.rotation == aimRotation || transform.rotation == inverseAimRotation) {
                aiming = false;
            }
        }
    }

    protected override void UseWeapon () {
        base.UseWeapon();
        Vector3 spawnPoint = transform.position;
        spawnPoint.x += (2.1f * transform.forward.x);
        spawnPoint.y += 3.5f;
        spawnPoint.z += (2.1f * transform.forward.z);
        //Debug.Log("creez proiectil");
        GameObject gameObject = (GameObject)Instantiate(GameService.extractWorldObject("Glont"), spawnPoint, transform.rotation);
        Proiectil projectile = gameObject.GetComponentInChildren< Proiectil >();
        projectile.SetRange(0.9f * weaponRange);
        projectile.SetTarget(target);
    }

    public override bool CanAttack() {
        //tureta poate ataca
        return true;
    }

    protected override void AimAtTarget () {
        base.AimAtTarget();
        aimRotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }
    
}
