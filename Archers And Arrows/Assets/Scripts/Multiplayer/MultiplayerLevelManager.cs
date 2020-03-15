﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
//using Photon.Pun.Demo.Asteroids;

namespace RhinoGame
{
    public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
    {
        
        public GameObject[] spawningPos;
        public static MultiplayerLevelManager Instance = null;
        public int MaxScore = 5;
        public Text InfoText;
        public Text Timer; /// Need to convert to panel
        public float timeLimit = 120;
        bool timeOver = false;
        public GameObject winnerText;
        public AudioClip winningVFX;


        public void Awake()
        {
            Instance = this;
            Timer.text = timeLimit.ToString();
        }

        public void Start()
        {
            StartGame();
            StartCoroutine(StartTimer());
        }

        IEnumerator StartTimer()
        {
            while(timeLimit > 0)
            {
                Timer.text = timeLimit.ToString("n1");
                yield return new WaitForEndOfFrame();
                timeLimit -= Time.deltaTime;
            }
            timeOver = true;
            CheckEndOfGame();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MultiplayerLobby");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(PhotonNetwork.NickName + " joined To " + PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                CheckEndOfGame();
            }
        }

        private void StartGame()   
        {
            int randomPoint = Random.Range(0, 3);
            PhotonNetwork.Instantiate("Chick", spawningPos[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.identity, 0);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("score"))
            {
                CheckEndOfGame();
            }
        }

        private void CheckEndOfGame()
        {
            bool showGameOver = false;

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log("Player: " + p.NickName + "Score: " + p.GetScore());
                
                if (p.GetScore() >= MaxScore || timeOver || PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    showGameOver = true;
                    break;
                }
            }

            if (showGameOver)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                string winner = "";
                int score = -1;
                Color color = Color.black;

                foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    if (p.GetScore() > score)
                    {
                        winner = p.NickName;
                        score = p.GetScore();
                        color = AsteroidsGame.GetColor(p.GetPlayerNumber());
                    }
                }

                StartCoroutine(EndOfGame(winner, score, color));
            }
        }

        private IEnumerator EndOfGame(string winner, int score, Color color)
        {
            if(!winnerText.activeSelf)
            {
                winnerText.SetActive(true);
            }
            
            float timer = 4.0f;
            AudioManager.StopBackGroundMusic();
            AudioManager.Play3D(winningVFX, Camera.main.transform.position);
            
            while (timer > 0.0f)
            {
                winnerText.GetComponentInChildren<Text>().color = color;
                GameObject.Find("Borders").GetComponent<Image>().color = color;
                winnerText.GetComponentInChildren<Text>().text = string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;
            }
            PhotonNetwork.LeaveRoom();
        }

    }
}