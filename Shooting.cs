using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{   
    //This code will only work for 2D game, edit it to make it work for 3D
    //Check out my Itch.io page as well
    //check out the README file if you have any problems
    [Header("Fire Modes:")]
    public int fireMode =1;
    public bool Single = false;
    public bool Automatic = false;
    public bool Burst = false;
    public bool Shotgun = false;

    //Recoil will not work properly unless you have a gameobject that works as your Cross Hair
    [Header("Recoil Config:")]
    public bool upwardsRecoil;
    public bool downwardsRecoil;
    public bool leftRecoil;
    public bool rightRecoil;

    [Header("Basic Config:")]
    public GameObject CrossHair;
    public GameObject bullet;
    public float bulletForce = 10f;
    public Transform firePoint;
    public float recoilMultiplier;
    public int maxAmmo = 7;
    public int ammo;
    public AudioSource shootSound;
    public float reloadTime = 2f;
    [HideInInspector] public bool isReloading = false;
    public ParticleSystem muzzleFlash;

    [Header("Auto Config:")]
    float nextTimeToFire = 0f;
    public float fireRate =4f;
    [Header("Burst Config:")]
    public int bulletsToShoot;
    public float burstFireRate;
    [Header("Shotgun Config:")]
    public Transform[] shotgunFirePoints;
    public float bulletsToFire;

    //public 
    // Start is called before the first frame update
    void Start()
    {
        ammo = maxAmmo;
        CheckFireMode();
    }

    // Update is called once per frame
    void Update()
    {
        if(isReloading)
            return;
        if(Single) {
            SingleFire();
        } else if(Automatic) {
            AutoFire();
        } else if(Burst) {
            BurstFire();
        } else if(Shotgun){
            ShotgunFire();
        }
        if(ammo<=0) {
            StartCoroutine(Reload());
            return;
        }
    }

    void CheckFireMode() {
        if(fireMode==1){
            Single=true;
            Automatic=false;
            Burst=false;
            Shotgun=false;
        } else if(fireMode==2){
            Single=false;
            Automatic=true;
            Burst=false;
            Shotgun=false;
        } else if(fireMode==3){
            Single=false;
            Automatic=false;
            Burst=true;
            Shotgun=false;
        } else if(fireMode==4){
            Single=false;
            Automatic=false;
            Burst=false;
            Shotgun=true;

        } else {
            Single=false;
            Automatic=false;
            Burst=false;
            Shotgun=false;
        }
    }

    void SingleFire() {
        if(Input.GetButtonDown("Fire1")&&ammo>0) {
            ammo--;
            Shoot();
        }
    }

    void AutoFire() {
        if(Input.GetButton("Fire1")&&ammo>0&& Time.time>=nextTimeToFire) {
            ammo--;
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }

    void BurstFire() {
        if(Input.GetButtonDown("Fire1")&&ammo>0) {
            ammo-=bulletsToShoot;
            StartCoroutine(burstFire());
        }
    }

    void ShotgunFire() {
        if(Input.GetButtonDown("Fire1")){
            for (int i = 0; i < shotgunFirePoints.Length; i++)
            {
                shootSound.Play();
                ammo--;
                GameObject bulletGO = Instantiate(bullet,shotgunFirePoints[i].position,shotgunFirePoints[i].rotation);
                bulletGO.GetComponent<Rigidbody2D>().AddForce(-shotgunFirePoints[i].right * bulletForce, ForceMode2D.Impulse);
                Destroy(bulletGO, 6f);
            }
            Recoil();
        }
    }

    void Recoil() {
        GameObject aim = CrossHair;
        if(upwardsRecoil&&downwardsRecoil&&leftRecoil&&rightRecoil){
            aim.GetComponent<Rigidbody2D>().position+= new Vector2(Random.Range(-1f,1)*recoilMultiplier,Random.Range(-1f,1f)*recoilMultiplier);
        } else if(upwardsRecoil&&downwardsRecoil) {
            aim.GetComponent<Rigidbody2D>().position+= new Vector2(0f,Random.Range(-1f,1f)*recoilMultiplier);
        } else if(leftRecoil&&rightRecoil) {
            aim.GetComponent<Rigidbody2D>().position+= new Vector2(Random.Range(-1f,1)*recoilMultiplier,0f);
        } else if(upwardsRecoil) {
            aim.GetComponent<Rigidbody2D>().position+= new Vector2(0,Random.Range(0.01f,1f)*recoilMultiplier);
        }  else if(downwardsRecoil) {
            aim.GetComponent<Rigidbody2D>().position+= new Vector2(0,Random.Range(-1f,-0.01f)*recoilMultiplier);
        }
    }

    void Shoot() {
        shootSound.Play();
        GameObject bulletGO = Instantiate(bullet,firePoint.position,firePoint.rotation);
        bulletGO.GetComponent<Rigidbody2D>().AddForce(-firePoint.right * bulletForce, ForceMode2D.Impulse);
        Recoil();
        Destroy(bulletGO, 6f);
        muzzleFlash.Play();
    }

    IEnumerator burstFire() {
        for (int i = 0; i < bulletsToShoot; i++)
            {
                Shoot();
                yield return new WaitForSeconds(1f/burstFireRate);
            }
    }

    IEnumerator Reload() {
        isReloading=true;
        yield return new WaitForSeconds(reloadTime);
        ammo=maxAmmo;
        isReloading=false;
    }
}
