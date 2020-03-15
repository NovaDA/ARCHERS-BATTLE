using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhinoGame;
using System;
using Photon.Pun;

public class PowerUps : MonoBehaviour
{
    public float speed; 
    public string namePowerUp = "";
    public int powerUp;
    private PhotonView photonView;
    public GameObject confetti;
    public AudioClip objectCollected;
    public AudioClip objectNotCollected;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        namePowerUp = gameObject.name.Replace("(Clone)", "");
        AssignPowerUpType(namePowerUp);
        StartCoroutine(RotateObject());
        StartCoroutine(ObjectLifeTime());
        Debug.Log(powerUp);
    }

    private void AssignPowerUpType(string nameP)
    {
        switch (nameP)
        {
            case "Health":
                powerUp = 20;
                break;
            case "Ammo":
                powerUp = 2;
                break;
            case "Charge":
                powerUp = 2;
                break;
            default:
                powerUp = 0;
                break;
        }
    }

    IEnumerator RotateObject()
    {
        while(true)
        {
            gameObject.transform.Rotate(Vector3.up, speed);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ObjectLifeTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(20);
            DestroyObject(false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.name != null || collision.gameObject.name != "Damage" || collision.gameObject.name != "Arrow(Clone)")
        { 
            if(collision.gameObject.tag == "Player")
            {
                if(collision.gameObject.TryGetComponent(out PlayerController player))
                collision.gameObject.GetComponent<PlayerController>().IncreasePower(namePowerUp, powerUp);
                if (collision.gameObject.TryGetComponent(out Player SinglePlayer))
                collision.gameObject.GetComponent<Player>().IncreasePower(namePowerUp, powerUp);
            }
            DestroyObject(true);
        }
    }

    private void DestroyObject(bool collected)
    {
        Destroy(this.gameObject);
        PoolManager.Spawn(confetti, gameObject.transform.position, Quaternion.identity);
        if(collected)
        {
            AudioManager.Play3D(objectCollected, transform.position);
        }
        else if(!collected)
        {
            AudioManager.Play3D(objectNotCollected, transform.position);
        }
    }

}
