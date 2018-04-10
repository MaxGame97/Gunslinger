using UnityEngine.Networking;
using UnityEngine;


public class PlayerWeapon : NetworkBehaviour {

    public GameObject bulletPrefab;
    public GameObject muzzle;

    [SerializeField]
    private int ammoCount;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private float reloadTimer;
    [SerializeField]
    private bool isReloading;
    [SerializeField]
    private int magSize = 6;

    private GameObject reloadText;
    [SerializeField] [SyncVar] private GameObject owner;  //Identity of the owner
    public GameObject Owner
    {
       get { return owner; }
    }

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
        
        GameObject go = GameObject.Find("Health - Canvas");
        if(go != null)
        {
            reloadText = go.transform.GetChild(1).gameObject;
        }
    }

    void Update ()
    {
        if (!hasAuthority)
            return;
       
        // If player has ammo and is not in reload state: Fire revolver and decrease ammoCount.
        if (Input.GetButtonDown("Fire1") && ammoCount >= 1 && isReloading == false)
        {
            CmdShoot(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation, owner);
            ammoCount--;
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

    // Reload the revolver.
    void Reload()    
    {
        isReloading = true;
        reloadTimer = Time.time + reloadTime;
        ammoCount = magSize;

        if (reloadText != null)
            reloadText.SetActive(true);
    }

    // Assign the owner of this weapon
    public void SetOwner(GameObject id) 
    {
        if (id == null)
            Debug.LogWarning("id is null!");
        owner = id;
    }

    /// <summary>
    /// COMMANDS 
    /// </summary>

    // Create the bullet object relative to the muzzle position and add velocity. 
    [Command]
    void CmdShoot(GameObject _bulletPrefab, Vector3 _position, Quaternion _rotation, GameObject _shooter)
    {
        GameObject bullet = Instantiate(bulletPrefab, _position, _rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 250f;
        NetworkServer.SpawnWithClientAuthority(bullet, _shooter.GetComponent<NetworkIdentity>().connectionToClient);
        bullet.GetComponent<Bullet>().RpcSetOwner(_shooter);
    }
}
