using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour
{

    //This class defines and controls individual weapon behaviours and states. It is triggered and activated by its mech's WeaponsController script.

    //Is the projectile attached to the muzzle transform (lasers, raybullets)?
    public bool isDiscreteProjectile;

    //Control References
    public WeaponController myController;
    Transform muzzlePoint;
    public GameObject myProjectile;

    //Audio stuff
    AudioSource fireSound;
    public float fireSoundPitchVariance = 0.05f;

    //4-character name for display
    public string weaponName;

    //Variables for determining weapon state and for HUD to leech info off of.
    public bool isActive { get; set; }
    public bool isStillFiring { get; set; }
    public bool triggered { get; set; }
    public bool isOverheating { get; protected set; }
    public float weaponRange;		//maximum range at which the weapon can be fired
    public float weaponSafetyRange; //Minimum range at which the weapon would be safely fired, so that AI doesn't blow itself up.
    public bool canFire { get; protected set; }


    //General refire variables
    public float refireTime;
    float nextFireTime;
    public float reloadPercentage { get; set; }

    //Accuracy
    public float fireConeAngle;

    //Per-weapon heat variables
    float weaponHeat;
    public float maxHeat;
    public float heatPerShot;
    public float heatSinkRate;
    public float heatRatio { get; protected set; }

    //How many projectiles are to be spawned per shot?
    public int projectilesPerShot;
    public float delayBetweenShots;
    private float delayCounter;
    private int delayedShots;

    //Weapon ammo levels
    public int ammoLevel;
    public int ammoMaxLevel;
    public int ammoPerShot = 1;

    //Energy cost
    public float energyPerShot;

    //Variables to determine whether the weapon requires a lock before, and control variables to handle this and pass to HUD/AI.
    public bool requiresLock;
    public float lockOnDuration;
    float lockOnTime;
    public float lockOnRatio { get; protected set; }

    Ray lockRay;
    int lockMask = ~((1 << 9) | (1 << 13)); //set up locking coll mask to ignore weapon-permeable and HUD meshes.

    // Use this for initialization
    void Awake()
    {
        muzzlePoint = transform.FindChild("muzzlePoint");
        fireSound = transform.GetComponent<AudioSource>();
        nextFireTime = Time.time;
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        canFire = CheckRefireVariables();

        if (isActive)
        {
            if (triggered && canFire)
            {
                if (requiresLock)
                {
                    if ((myController.lockedTarget != null) && (lockOnTime == 0))
                    {
                        lockOnTime = Time.time + lockOnDuration;
                    }
                    LockOnTargetAndFire();
                }
                else
                {
                    FireWeapon();
                }
            }
            else
                lockOnTime = 0;
        }

        if (isStillFiring)
        {
            delayCounter += Time.deltaTime;
            if (delayCounter >= delayBetweenShots)
                ContinueFiring();
        }

        ApplyHeat();

        ApplySound();


        reloadPercentage = (nextFireTime - Time.time) / refireTime;
        reloadPercentage = Mathf.Clamp(reloadPercentage, 0, 1);
    }

    private void ContinueFiring()
    {
        delayedShots -= 1;
        FireSingleShot();

        if (delayedShots == 0)
        {
            isStillFiring = false;
            delayCounter = 0.0f;
        }
        else
        {
            isStillFiring = true;
            delayCounter = 0.0f;
        }
    }

    private void FireWeapon()
    {
        if (delayBetweenShots <= 0.0f)
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
                FireSingleShot();
            }
        }
        else
        {
            delayedShots = projectilesPerShot - 1;
            FireSingleShot();
            isStillFiring = true;
            delayCounter = 0.0f;
        }

        weaponHeat += heatPerShot;
        myController.heatLevel += heatPerShot;

        myController.energyLevel -= energyPerShot;

        if (ammoMaxLevel > 0)
        {
            ammoLevel -= ammoPerShot;
        }

        if ((ammoMaxLevel > 0) && (ammoLevel == 0))
        {
            nextFireTime = 0;
            weaponHeat = 0;
        }
        else
            nextFireTime = Time.time + refireTime;

    }

    private void FireSingleShot()
    {
        Quaternion fireDirection = transform.rotation;
        if (fireConeAngle > 0f)
        {
            fireDirection.eulerAngles += new Vector3(Random.Range(-fireConeAngle, fireConeAngle), Random.Range(-fireConeAngle, fireConeAngle), 0f);

        }

        GameObject pew = (GameObject)Instantiate(myProjectile, muzzlePoint.position, fireDirection);

        //Attach the projectile to the muzzle transform (for lasers), otherwise it can go its own way.
        if (!isDiscreteProjectile)
        {
            pew.transform.parent = muzzlePoint;
        }
        else
        {
            Physics.IgnoreCollision(pew.collider, myController.collider);
        }

        if (requiresLock)
        {
            pew.GetComponent<MissileSpawnerScript>().lockedTarget = myController.lockedTarget;
        }
    }

    private bool CheckRefireVariables()
    {
        if ((!isOverheating) && (!myController.isOverheating))
        {
            if (Time.time >= nextFireTime)
            {
                if ((ammoMaxLevel == 0) || ((ammoMaxLevel > 0) && (ammoLevel > 0)))
                {
                    if ((energyPerShot == 0) || ((energyPerShot > 0) && (myController.energyLevel > 0)))
                    {
                        return true;
                    }
                }
            }


        }

        return false;

    }

    void ApplyHeat()
    {

        weaponHeat = Mathf.Clamp(weaponHeat - (heatSinkRate * Time.deltaTime), 0, maxHeat * 1.5f);

        if (myController.isOverheating)
        {
            isOverheating = true;
        }
        else
        {
            if (weaponHeat > maxHeat) { isOverheating = true; }

            if ((isOverheating) && (weaponHeat == 0)) { isOverheating = false; }
        }
        heatRatio = weaponHeat / maxHeat;
    }


    void LockOnTargetAndFire()
    {

        if (myController.lockedTarget != null)
        {
            if (lockOnTime <= Time.time)
            {
                Debug.Log("FIRE MISSILES!");
                FireWeapon();
                lockOnTime = 0;
            }
        }

        lockOnRatio = ((lockOnTime - Time.time) / lockOnDuration);
    }

    void ApplySound()
    {
        if (fireSound)
        {
            if (isActive && triggered && canFire && !fireSound.isPlaying)
            {
                if (fireSoundPitchVariance > 0)
                    fireSound.pitch = Random.Range(1 - fireSoundPitchVariance, 1 + fireSoundPitchVariance);

                fireSound.Play();

            }
            if ((!triggered || isOverheating) && (fireSound.loop))
                fireSound.Stop();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(lockRay);

    }


}
