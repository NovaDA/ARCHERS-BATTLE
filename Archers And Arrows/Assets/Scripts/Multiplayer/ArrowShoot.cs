using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace CannonGame
{
    public class ArrowShoot : MonoBehaviour
    {

        #region Projectile Properties
        public GameObject arrowInGear;
        float chargeLevel;
        public float chargerSpeed;
        public float chargeLimit;
        public float maxChargeLimit = 25;
        public float minlaunchForce;
        bool fired;
        public Rigidbody projectilePrefab;
        public Transform spawnPos;
        #endregion
        public int ammoPlayer;
        public int maxAmmo;
        #region Aim Arrow UI
        public Slider AimSlider;
        #endregion
        public bool controllable = true;
        public float FireRate = 0.75f;
        private float nextFire;
        private PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            maxAmmo = 10;
        }
        private void OnEnable()
        {
            chargeLevel = minlaunchForce;
            AimSlider.value = minlaunchForce;
        }
        private void Start()
        {
            chargerSpeed = chargeLimit - minlaunchForce;
        }

        void Update()
        {

            if (!photonView.IsMine || !controllable)
                return;

            if(ammoPlayer != 0)
            {
                AimSlider.value = minlaunchForce;
                photonView.RPC("SetArrowInGear", RpcTarget.AllViaServer, true);

                if (chargeLevel >= chargeLimit && !fired)
                {
                    chargeLevel = chargeLimit;
                    photonView.RPC("Firing", RpcTarget.AllViaServer, transform.rotation, chargeLevel);  
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    fired = false;
                    chargeLevel = minlaunchForce;
                }
                else if (Input.GetMouseButton(0) && !fired)
                {
                    chargeLevel += chargerSpeed * Time.deltaTime;
                    AimSlider.value = chargeLevel;

                }
                else if (Input.GetMouseButtonUp(0) && !fired)
                {
                    photonView.RPC("Firing", RpcTarget.AllViaServer, transform.rotation, chargeLevel);

                }
            }
            
        }

        [PunRPC]
        private void SetArrowInGear(bool state)
        {
            arrowInGear.SetActive(state);
        }

        [PunRPC]
        private void Firing(Quaternion rotation, float charge, PhotonMessageInfo info)
        {
            if (Time.time > nextFire)
            {
                fired = false;
                nextFire = Time.time + FireRate;
                float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
                Rigidbody bullet = Instantiate(projectilePrefab, spawnPos.position, spawnPos.transform.rotation) as Rigidbody;
                bullet.GetComponent<ArrowBehaviour>().InitializeArrow(photonView.Owner, spawnPos.forward , Mathf.Abs(lag), charge);
                chargeLevel = 0;
                ammoPlayer--;
                if(ammoPlayer == 0)
                {
                    photonView.RPC("SetArrowInGear", RpcTarget.AllViaServer, false);
                }
            }

        }
    }
}

