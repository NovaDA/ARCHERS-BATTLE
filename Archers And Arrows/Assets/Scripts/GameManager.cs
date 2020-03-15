using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using RhinoGame;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioClip menuM;
    public AudioClip gameM;
    public AudioClip winS;

    public static AudioClip menuMusic;
    public static AudioClip gameMusic;
    public static AudioClip wonSound;

    //public string SceneName;

    private void Awake()
    {
       Instance = this;
       DontDestroyOnLoad(this);

       menuMusic = menuM;
       gameMusic = gameM;
       wonSound = winS;
    }
    #region Scene Management
    public void LoadScene(string SceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        Debug.Log(SceneName);
   
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log(" Level Delegate " + " Level Loaded ");
        // Debug.Log(scene.name);
        //Debug.Log(mode);
        AudioManager.PlayBackGroundMusic();
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    //Use this function to generate Random values

    #region SCORE MANAGEMENT JSON
    public void SetRandomData()
    {
        playerData = new PlayerData()
        {
            username = "Player " + UnityEngine.Random.Range(1, 5),
            bestScore = UnityEngine.Random.Range(1, 4),
            date = DateTime.UtcNow,
            totalPlayersInTheGame = UnityEngine.Random.Range(1, 5),
            roomName = "Random Room " + UnityEngine.Random.Range(1, 5)
        };
        PrintPlayerData();
    }

    public void SavePersonalBest()
    {
        // Uncomment this line if you want to use Unity's built-in JSON solution
        //var serializedData = JsonUtility.ToJson(playerData);

        // JSON.net - convert the object into a string
        var serializedData = JsonConvert.SerializeObject(playerData);

        // Write the string to the file
        File.WriteAllText(fileName, serializedData);

        Debug.Log("Data saved!");
    }
    public void LoadPersonalBest()
    {
        try
        {
            // Load the content from the file, if the file does not exist then create it first
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, "");
            }
            var fileContent = File.ReadAllText(fileName);

            // Uncomment this line if you want to use Unity's built-in JSON solution
            //playerData = JsonUtility.FromJson<PlayerData>(fileContent);

            // JSON.net - convert the string into an object
            playerData = JsonConvert.DeserializeObject<PlayerData>(fileContent);

            // Uncomment this line if you want to print out the playerData object
            //PrintPlayerData();
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("File not found: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.Log("Error occured: " + e.Message);
            Debug.Log("Error stack trace: " + e.StackTrace);
        }
    }

    //Use this function to print out the playerData object.

    public void PrintPlayerData()
    {
        if (playerData == null)
        {
            Debug.Log("playerData is null - can't print it!");
            return;
        }
        Debug.Log("**************");
        Debug.Log("username: " + playerData.username);
        Debug.Log("bestScore: " + playerData.bestScore);
        Debug.Log("date: " + playerData.date);
        Debug.Log("totalPlayersInTheGame: " + playerData.totalPlayersInTheGame);
        Debug.Log("roomName: " + playerData.roomName);
        Debug.Log("**************");
    }

    public void ShowPersonalBestPopUp()
    {
        PersonalBestPopUp.SetActive(true);
    }


    public GameObject PersonalBestPopUp;
    [SerializeField]
    private string fileName;
    [HideInInspector]
    public PlayerData playerData;

    #endregion
    
}