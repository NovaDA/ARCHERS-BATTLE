using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerUpManager : MonoBehaviour
{
    int numberPowerUpsActive = 0;
    int sizeSpawnLocation = 40;
    int heightSpawnLocation = 20;
    public GameObject[] powerUpsPref;

    private void Start()
    {
        StartCoroutine(SpawnPowerUps());
        StartCoroutine(CheckPowerUpsAroundArena());
    }
  
    IEnumerator CheckPowerUpsAroundArena()
    {
        while(true)
        {
            PowerUps[] powerUpsInTheArena = FindObjectsOfType<PowerUps>();
            numberPowerUpsActive = powerUpsInTheArena.Length;
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            if(numberPowerUpsActive < 10)
            {
                int probabilityPowerUp = GetProbability();
                GameObject powerUp = powerUpsPref[probabilityPowerUp];
                PhotonNetwork.Instantiate(powerUp.name, GetRandomPositionOnMap(), Quaternion.identity, 0);
            }
            yield return new WaitForSeconds(2.0f);
        }
    }

    private int GetProbability()
    {
        int randomN = UnityEngine.Random.Range(0, 101);

        if(randomN <= 50) // 50% p of Arrow
        {
            return 0;
        }
        else if(randomN <= 90 && randomN > 50) // 40% p of Health
        {
            return 1;
        }
        else   // 10% p of PowerUp
        {
            return 2;
        }
    }

    private Vector3 GetRandomPositionOnMap()
    {
        int randX = UnityEngine.Random.Range(-sizeSpawnLocation, sizeSpawnLocation + 1);
        int randZ = UnityEngine.Random.Range(-sizeSpawnLocation, sizeSpawnLocation + 1);
        Vector3 randPos = new Vector3(randX, heightSpawnLocation, randZ);
        return randPos;
    }
}
