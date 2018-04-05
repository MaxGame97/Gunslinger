using UnityEngine.Networking;
using UnityEngine;

public class Bullet : NetworkBehaviour {

    private NetworkIdentity owner;
    [SerializeField] private int bulletDamage;
    
    public void SetOwner(NetworkIdentity id)
    {
        owner = id;
    }

    [Command]
    void CmdPlayerHit(int _damage, GameObject _player, NetworkIdentity shooter)
    {
        _player.GetComponent<PlayerHealth>().RpcTakeDamage(_damage, owner);
        Debug.Log(shooter + " dealt " + _damage + " to " + _player);
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collided");
        if (other.gameObject.CompareTag("Player"))
            CmdPlayerHit(bulletDamage, other.gameObject, owner);

        //Play hit particles at other.contacts[0].point? 
        Destroy(this.gameObject);
    }
}
