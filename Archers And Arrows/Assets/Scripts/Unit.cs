using RhinoGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    //[HideInInspector]
    public int Health = 100;
	public Slider HealthBar;
	public AudioClip shotClip;
	public GameObject shotFX;
	private float nextFire;
	public float fireRate = 0.75f;
	public GameObject bullet;
	public Transform shotPos;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			var bullet = collision.gameObject.GetComponent<Bullet>();
			TakeDamage(bullet.damage);
		}
	}
    public virtual void AddHealth(int energy)
    {
        Health += energy;
        HealthBar.value = Health;
    }
	public virtual void TakeDamage(int damage)
	{
		Health -= damage;
        HealthBar.value = Health;
        if (Health <= 0)
		{
			UnitDied();
		}
	}

	public void Shoot(Vector2 direction = default(Vector2))
	{
		if (Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;

			GameObject obj = PoolManager.Spawn(bullet, shotPos.position, transform.rotation);
			Bullet blt = obj.GetComponent<Bullet>();

			if (shotFX)
				PoolManager.Spawn(shotFX, shotPos.position, Quaternion.identity);
			if (shotClip)
				AudioManager.Play3D(shotClip, shotPos.position, 0.1f);
		}
	}

	public virtual void UnitDied()
	{
		gameObject.SetActive(false);
	}
}
