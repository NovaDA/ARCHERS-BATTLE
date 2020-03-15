using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CannonGame
{
    public class ArrowShootS : MonoBehaviour
    {
        #region Projectile Properties
        float chargeLevel;
        public float chargerSpeed;
        public float chargeLimit;
        public float maxChargeLimit;
        public float minlaunchForce;
        public float ammoPlayer;
        public float maxAmmo = 10;
        bool fired;
        public Rigidbody projectilePrefab;
        public Transform spawnPos;
        #endregion

        public Text ammo;
        public Text power;

        #region Aim Arrow UI
        public Slider AimSlider;
        #endregion

        public float FireRate = 0.75f;
        private float nextFire;

        private void OnEnable()
        {
            chargeLevel = minlaunchForce;
            AimSlider.value = minlaunchForce;
        }
        private void Start()
        {
            
        }
        // Update is called once per frame
        void Update()
        {

            AimSlider.value = minlaunchForce;
            ammo.text = ammoPlayer.ToString();
            power.text = chargeLimit.ToString();
            if (ammoPlayer == 0)
            return;
            
            if (chargeLevel >= chargeLimit && !fired)
            {
                chargeLevel = chargeLimit;
                Fire();
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
                Fire();
                
            }
        }
        private void Fire()
        {
            fired = false;
            Rigidbody bullet = Instantiate(projectilePrefab, spawnPos.position, gameObject.transform.rotation) as Rigidbody;
            bullet.velocity = chargeLevel * spawnPos.forward;
            chargeLevel = minlaunchForce;
            ammoPlayer--;
        }
    }
}

