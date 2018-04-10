using UnityEngine.Networking;
using UnityEngine;


public class PlayerWeapon : NetworkBehaviour {

    public NetworkInstanceId owner;  //Identity of the owner
    public GameObject bulletPrefab;
    public GameObject muzzle;
    private GameObject reloadText;
    private float reloadTimer;

    [SerializeField] private int ammoCount;
    [SerializeField] private float reloadTime;
    [SerializeField] private bool isReloading;
    [SerializeField] private int magSize = 6;
    [SerializeField] private int ammoMissing;

    void Start () {

        ammoCount = magSize;
        reloadTime = 3f;
        // Find the muzzle object on the player and log error if none is found.
        if (muzzle == null)
        {
            Debug.LogError("No muzzle detected for player prefab.");
            this.enabled = false;
        }
        if (bulletPrefab == null)
        {
            Debug.LogError("No bulletPrefab detected for Firing Script.");
            this.enabled = false;
        }
        
        GameObject healthCanvas = GameObject.Find("Health - Canvas");
        if(healthCanvas != null)
        {
            reloadText = healthCanvas.transform.GetChild(1).gameObject;
        }
    }
    public void SetOwner(NetworkInstanceId id)
    {
        owner = id;
    }

    // Updates every frame.
    void Update () {

        // If it isn't a local player then skip this update function.
        if (!isLocalPlayer && !hasAuthority)
        {
            return;
        }
        // If player has ammo and is not in reload state: Fire revolver. Decrease ammoCount and increase missing ammo.
        if (Input.GetButtonDown("Fire1") && ammoCount >= 1 && isReloading == false)
        {
            CmdShoot(muzzle.transform.position, muzzle.transform.rotation, owner);
            ammoCount--;
            ammoMissing = magSize - ammoCount;
        }

        // Reload revolver if not already reloading. 
        if (Input.GetButtonDown("Fire2") && isReloading == false && ammoCount != magSize)
        {
            Reload();
            
        }
        // Start reload timer until isReload is false again. 
        if(isReloading == true)
        {
            if(Time.time >= reloadTimer)
            {
                isReloading = false;
                reloadTimer = 0f;

                if (reloadText != null)
                    reloadText.SetActive(false);
            }
        }
	}



    // Create the bullet object relative to the muzzle position and add velocity. 
    [Command]
    void CmdShoot(Vector3 _position, Quaternion _rotation, NetworkInstanceId shooter)
    {
        GameObject bullet = Instantiate(bulletPrefab, _position, _rotation);
        //bullet.GetComponent<Bullet>().SetOwner(shooter);
        NetworkServer.Spawn(bullet);
    }

    // Reload the revolver.
    void Reload()
    {
        isReloading = true;
        reloadTimer = Time.time + reloadTime;
        ammoCount = magSize;
        ammoMissing = 0;

        if (reloadText != null)
            reloadText.SetActive(true);
    }
}
