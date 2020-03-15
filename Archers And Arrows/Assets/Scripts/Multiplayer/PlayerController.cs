using UnityEngine;
using CannonGame;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Pun.Demo.Asteroids;


namespace RhinoGame
{
    public class PlayerController : MonoBehaviour
    {
        public float Health = 100;
        public float RotationSpeed = 8.0f;
        public float MovementSpeed = 10f;

        [HideInInspector]
        public FollowTarget camFollow;
        [HideInInspector]
        public bool controllable = true;
        private PhotonView photonView;
        private new Rigidbody rigidbody;
        public Text playerName;
        public Text playerAmmo;
        public Text playerPower;
        public void Awake()
        {
            photonView = GetComponent<PhotonView>();

            rigidbody = GetComponent<Rigidbody>();

            if (photonView.IsMine)
            {
                camFollow = Camera.main.GetComponent<FollowTarget>();
                camFollow.target = transform;
            }

            SetPlayerColor();
        }

        [PunRPC]
        public void Damage(float damage)
        {
            Health -= damage;
        }

        public void Update()
        {
            if (!photonView.IsMine || !controllable)
            {
                return;
            }

            playerPower.text = GetComponent<ArrowShoot>().chargeLimit.ToString();
            playerAmmo.text = GetComponent<ArrowShoot>().ammoPlayer.ToString();
            Vector2 moveDir;

            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                moveDir.x = 0;
                moveDir.y = 0;
                GetComponent<PhotonView>().RPC("ActivateTrail", RpcTarget.AllViaServer, false);
            }
            else
            {
                moveDir.x = Input.GetAxis("Horizontal");
                moveDir.y = Input.GetAxis("Vertical");
                Move(moveDir);
                GetComponent<PhotonView>().RPC("ActivateTrail", RpcTarget.AllViaServer, true);
            }
          
        }

        public void FixedUpdate()
        {
            if (!photonView.IsMine || !controllable)
            {
                return;
            }

            if ((rigidbody.constraints & RigidbodyConstraints.FreezePositionY) != RigidbodyConstraints.FreezePositionY)
            {
                if (transform.position.y > 0)
                {
                    rigidbody.AddForce(Physics.gravity * 2f, ForceMode.Acceleration);
                }
            }
        }

        public void IncreasePower(string powerType, int power)
        {
            switch (powerType)
            {
                case "Health":
                    Health += power;
                    break;
                case "Ammo":
                    if(GetComponent<ArrowShoot>().ammoPlayer < GetComponent<ArrowShoot>().maxAmmo)
                    {
                        GetComponent<ArrowShoot>().ammoPlayer += power;
                        Debug.Log("I am in Bullet" + GetComponent<ArrowShoot>().ammoPlayer);
                    }
                    break;
                case "Charge":
                    if(GetComponent<ArrowShoot>().chargeLimit < GetComponent<ArrowShoot>().maxChargeLimit)
                    {
                        GetComponent<ArrowShoot>().chargeLimit += power;
                    }                 
                    break;
                default:
                    Debug.LogError("Power Not Assigned");
                    break;
            }
        }

        void Move(Vector2 direction = default(Vector2))
        {
            if (direction != Vector2.zero)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y))
                    * Quaternion.Euler(0, camFollow.camTransform.eulerAngles.y, 0);
            }

            Vector3 movementDir = transform.forward * MovementSpeed * Time.deltaTime;
            rigidbody.MovePosition(rigidbody.position + movementDir);
            
        }

        private void SetPlayerColor()
        {
            if(playerName != null)
            {
                playerName.text = photonView.Owner.NickName;
                playerName.color = AsteroidsGame.GetColor(photonView.Owner.ActorNumber - 1);    // Actor Numbers Starts Always From 1
            }
        }
    }
    #region NOT USED
    //public GameObject BulletPrefab;
    //timestamp when next shot should happen
    // private float nextFire;
    ///public float FireRate = 0.75f;
    /// //public Transform ShootingPos;
    ///       /// public int AmmoQuantity = 5;
    /// <summary>
    /// Delay between shots.
    /// </summary>
    /// 

    //GameObject gear = transform.Find("Gear").gameObject;

    //        foreach (Transform child in gear.transform)
    //        {
    //            Renderer render = child.gameObject.GetComponent<Renderer>();
    //render.material.color = AsteroidsGame.GetColor(photonView.Owner.ActorNumber - 1);
    //        }
    #endregion
}