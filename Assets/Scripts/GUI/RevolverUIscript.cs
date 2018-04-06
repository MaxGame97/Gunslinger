using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevolverUIscript : MonoBehaviour
{

    public int revolverSize = 6;

    public Image startingBullet;

    public Image emptySlot;

    public Sprite emptySloth;

    public Sprite startingBallet;

    List<Image> bullets = new List<Image>();

    private int currentSlot = 0;

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
            Vector3 pos = Circle(center, radiusen, a);
            bullets.Add(Instantiate(startingBullet, pos, Quaternion.Euler(0,0,-a), transform));
        }
    }

    Vector3 Circle(Vector3 center, float radius, int a)
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootBullet();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadBullet(startingBallet);
        }
    }

    public void LoadBullet(Sprite bulletImage)
    {
        if (!loaded)
        {
            bullets[currentSlot % revolverSize].sprite = bulletImage;
            ChangeCurrentSlot();
            if (currentSlot % revolverSize == 0)
                loaded = !loaded;
        }
    }

    public void ShootBullet()
    {
        if (loaded)
        {
            bullets[currentSlot % revolverSize].sprite = emptySloth;
            ChangeCurrentSlot();
            if (currentSlot % revolverSize == 0)
                loaded = !loaded;
        }
    }

    private void ChangeCurrentSlot()
    {
        currentSlot++;
        StopAllCoroutines();
        StartCoroutine(Rotate(0.2f, (360 / revolverSize) * (currentSlot % revolverSize)));
        //transform.rotation = Quaternion.Euler(0,0, (360 / revolverSize)*(currentSlot%revolverSize));
    }

    IEnumerator Rotate(float duration,float target)
    {

        float startRotation = transform.eulerAngles.z;
        float endRotation = target;
        if (target < 0.1f)
        {
            endRotation = 360.0f;
        }
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            yield return null;
        }
    }
}
