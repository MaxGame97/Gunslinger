using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevolverUIscript : MonoBehaviour
{
    //The number of Bullet slots in the revolver, this gets set by PlayerWeapon
    public int revolverSize = 6;

    //Prafab for the starting image wich is supposed to be a loaded bullet
    public Image startingBullet;

    //The sprite for the empty bullet
    public Sprite emptySlot;

    //the sprite for the loaded bullet
    public Sprite startingBulletSprite;

    //The list containing the bullet images
    List<Image> bullets = new List<Image>();

    //Variable determining the curent slot that is at the top
    private int currentSlot = 0;

    //The radius of the circle
    public float radius = 60;

    //The size of the images (between 0 and 1).
    public float scale = 1.0f;

    //The relative position of the center of the circle and the bottom right corner of the screen
    public Vector3 relativePosition = new Vector3(-150, 150, 0);

    //The time it takes in seconds to rotate one step, only used for shooting
    public float revolveSpeed = 0.2f;

    //Bool used to make the reload animation wait for each step of the reload
    public bool waiting;


    void Start()
    {
        //Places the center of the revolverUI relative to the bottom right corner
        transform.position += relativePosition;

        //Temporary variable to store the transform position
        Vector3 center = transform.position;

        //Spawn the images in a circle around the center
        for (int i = 0; i < revolverSize; i++)
        {
            //Calcylates the rotation of each of the images for initialization
            int a = (360 / revolverSize) * i;

            //Temp variable to store the position of the image for initialization
            Vector3 pos = Circle(center, radius, a);

            //Instatiate the images
            bullets.Add(Instantiate(startingBullet, pos, Quaternion.Euler(0,0,-a), transform));

            //Sets the size of the images
            bullets[i].transform.localScale *= scale;
        }
    }

    //Calculates the position of the images for use when instatiating
    private Vector3 Circle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    //Testing update, to check everything without player
    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Mouse0))
    //    //{
    //    //    ShootBullet();
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.Mouse1))
    //    //{
    //    //    LoadBullet();
    //    //}
    //}

    //Loads the gun, Sprite bullet image might be used later when more bullets are added, this is called from PlayerWeapon
    public void LoadBullet(float reloadTime/*Sprite bulletImage*/)
    {
        //Changes the current slot to one to make sure the reload finnishes in slot 0
        currentSlot = 1;

        //sets the current rotation to 0 since we only want to start a reload from the first slot
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        //emptys out the gun before reaload (Sets all the sprites to the empty sprite
        for (int i = 0; i < revolverSize; i++)
        {
            bullets[i].sprite = emptySlot;
        }
        
        //Make sure no other corutines are running (only for this behavior, no others are affected)
        StopAllCoroutines();

        //Starts the reloading corutine, the duration parameter is eaqual to the whole reload time divided by the number of bullets
        //The target parameter is eaqual to 360 divided by the number of bullets
        //The bulletImage parameter is the sprite of the bullet that is to be loaded
        StartCoroutine(LoadRotate(reloadTime/revolverSize, (360 / revolverSize), startingBulletSprite));

        //Set the current position to 0
        currentSlot = 0;
    }

    //Shoots the gun, is called from PlayerWeapon
    public void ShootBullet()
    {
        //Switches the current sprite for the empty sprite
        bullets[currentSlot % revolverSize].sprite = emptySlot;

        //Increases current slot
        currentSlot++;

        //Rotates the UI one step
        ChangeCurrentSlot();
    }

    //Rotates The UI one step
    private void ChangeCurrentSlot()
    {
        //Make sure no other corutines are running (only for this behavior, no others are affected)
        StopAllCoroutines();

        //Rotates the UI one step smoothly, duration is the time it takes to rotate it and target is the next position
        StartCoroutine(Rotate(revolveSpeed, (360 / revolverSize) * (currentSlot % revolverSize)));
    }

    //Rotates the UI one step smoothly, duration is the time it takes to rotate it and target is the next position
    IEnumerator Rotate(float duration,float target)
    {
        //Temp variables to use in the corutine
        float startRotation = transform.eulerAngles.z;
        float endRotation = target;

        //Makes sure that it rotates a full circle and doesn't turn back for the last step
        if (target < 0.1f)
        {
            endRotation = 360.0f;
        }

        //Temp variable to store time in
        float t = 0.0f;

        //Runs during the duration passed as a parameter
        while (t < duration)
        {
            //Time passed since last frame is added to t
            t += Time.deltaTime;

            //Calculates the new rotation based on how much time is left
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;

            //Sets the new rotation
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);

            //Waits for next frame
            yield return null;
        }

        //Sets waiting to false (is used when reloading the gun)
        waiting = false;
    }

    //Fully reloads the UI Smoothly, duration is the time for the complete reload, target is the first target for the rotation and bulletImage is the sprite to be used
    IEnumerator LoadRotate(float duration, float target, Sprite bulletImage)
    {
        //Temp variables to use in the corutine
        float startRotation = transform.eulerAngles.z;
        float endRotation = target;

        //Makes sure that it rotates a full circle and doesn't turn back for the last step
        if (target < 0.1f)
        {
            endRotation = 360.0f;
        }

        //Rotates one step for each slot in the revolver
        for (int i = 0; i < revolverSize; i++)
        {
            //Sets the sprite to the new sprite
            bullets[i].sprite = bulletImage;

            //Set waiting to be true
            waiting = true;

            //Rotate the UI one step
            StartCoroutine(Rotate(duration, endRotation));

            //Wait for the rotation to finnish
            yield return new WaitUntil(() => waiting == false);

            //Set the next target for the rotation
            endRotation += 360/revolverSize;
        }
    }

    //sets the revolverSize, this is called from Player Weapon
    public void SetRevolverSize(int magSize)
    {
        revolverSize = magSize;
    }
}
