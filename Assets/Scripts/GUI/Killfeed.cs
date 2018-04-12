// Feeds are spawned and handled per client, only thing handled by the network is sending and receiving the correct information!

using UnityEngine;

public class Killfeed : MonoBehaviour {

    #region Singleton
    public static Killfeed instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] private float destructionTimer;    // The time until the spawned info is destroyed
    [SerializeField] private Transform list;            // Reference to the object we want to assign as parent
    [SerializeField] private GameObject feedPrefab;     // Reference to the prefab object

    public void Spawnfeed(string _killer, string _target, bool _headshot)
    {
        GameObject feed = Instantiate(feedPrefab, list);             // Spawn object.
        KillfeedInfo info = feed.GetComponent<KillfeedInfo>();       // Get reference

        if (info)
            info.Setup(_killer, _target, _headshot);                 // Initilize the feeds info.

        Destroy(feed, destructionTimer);                             // Destroy it afet descrutionTimer seconds.
    }
}
