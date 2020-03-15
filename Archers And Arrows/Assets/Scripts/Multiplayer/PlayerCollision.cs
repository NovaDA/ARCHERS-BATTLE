﻿using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CannonGame;

namespace RhinoGame
{
    public class PlayerCollision : MonoBehaviour, IPunObservable
    {
        public GameObject Trail;
        public ParticleSystem Destruction;
        public Slider HealthBar;

        private PhotonView photonView;
        private new Rigidbody rigidbody;
        private new Collider collider;
        private GameObject Render;
        private PlayerController playerController;
        private ArrowShoot ArrowShoot;

        // Start is called before the first frame update
        void Awake()
        {
            photonView = GetComponent<PhotonView>();
            rigidbody = GetComponent<Rigidbody>();
            Render = transform.Find("RendererObj").gameObject;
            collider = GetComponent<Collider>();
            playerController = GetComponent<PlayerController>();
            ArrowShoot = GetComponent<ArrowShoot>();
        }

        // Update is called once per frame
        void Update()
        {
            HealthBar.value = playerController.Health;
        }

        private IEnumerator WaitForRespawn()
        {
            yield return new WaitForSeconds(AsteroidsGame.PLAYER_RESPAWN_TIME);

            photonView.RPC("RespawnPlayer", RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void DestroyPlayer()
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.isKinematic = true;

            collider.enabled = false;
            Destruction.Play();
            Render.SetActive(false);
            playerController.controllable = false;
            ArrowShoot.controllable = false;
            HealthBar.gameObject.SetActive(false);
            StartCoroutine("WaitForRespawn");
            #region NOT USED
            //if (photonView.IsMine)
            //{
            //    object lives;
            //    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives))
            //    {
            //        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { AsteroidsGame.PLAYER_LIVES, ((int)lives <= 1) ? 0 : ((int)lives - 1) } });

            //        if (((int)lives) > 1)
            //        {
            //            StartCoroutine("WaitForRespawn");
            //        }
            //    }
            //}
            #endregion
        }

        [PunRPC]
        public void ActivateTrail(bool activation)
        {
            Trail.SetActive(activation);
        }

        [PunRPC]
        public void RespawnPlayer()
        {
            rigidbody.isKinematic = false;
            collider.enabled = true;
            Render.SetActive(true);
            playerController.controllable = true;
            ArrowShoot.controllable = true;
            playerController.Health = 100;
            HealthBar.gameObject.SetActive(true);
            Destruction.Stop();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(playerController.Health);
            }
            else
            {
                // Network player, receive data
                playerController.Health = (float)stream.ReceiveNext();
            }
        }

    }
}