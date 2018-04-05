using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevolverUIscript : MonoBehaviour
{

    public int revolverSize = 6;

    public Image startingBullet;

    public Image emptySlot;

    List<Image> bullets = new List<Image>();

    private int currentSlot;

    public int b = 0;

    public float radiusen;

    private bool loaded = true;

    // Use this for initialization
    void Start()
    {
        Vector3 center = transform.position;
        for (int i = 0; i < revolverSize; i++)
        {
            int a = (360 / revolverSize) * i;
            Vector3 pos = RandomCircle(center, radiusen, a);
            bullets.Add(Instantiate(startingBullet, pos, Quaternion.Euler(0,0,-a), transform));
        }
    }

    Vector3 RandomCircle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0,0,b) * Time.deltaTime);
    }

    public void LoadBullet(Image bulletImage)
    {

    }

    public void ShootBullet()
    {


        if (!bullets.Contains(emptySlot))
        {
            loaded = true;
        }
    }
}
