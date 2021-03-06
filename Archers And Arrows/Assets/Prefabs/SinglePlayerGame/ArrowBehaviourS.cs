﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhinoGame;

public class ArrowBehaviourS : MonoBehaviour
{

    #region Explosion
    public LayerMask enemies;
    public GameObject explosion_particles;
    public AudioClip explosionAudio;
    public float Maxdamage = 100f;
    public float explosionForce = 1000f;
    public float maxLifeTime = 2f;
    public float explosionRadius = 5f;
    #endregion

    Rigidbody rb;
    private Renderer renderer;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnCollisionEnter(Collision collision)    /// First make Arrow Explode ==> then Create a Sphere with a rafius for damage
    {
        explosion_particles.transform.parent = null;
        PoolManager.Spawn(explosion_particles, transform.position, transform.rotation);
        AudioManager.Play3D(explosionAudio, transform.position);
        CheckForObjectCollision();
        /// Destroy(explosion_particles.gameObject, explosion_particles.main.duration);
        Destroy(gameObject);
    }

    private void CheckForObjectCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.name == "Damage")
            {
                Rigidbody target = colliders[i].GetComponentInParent<Rigidbody>();

                if (!target)
                    continue;

                target.AddExplosionForce(explosionForce, transform.position, explosionRadius); // v

                var Enemy = target.GetComponent<Unit>();

                if (!Enemy)
                    continue;

                float damage = CalculateDamage(target.position);
                Debug.Log(damage);

                Enemy.TakeDamage((int)damage);

                if (Enemy.Health <= 0)
                {
                    Enemy.UnitDied();
                }
            }
        }
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDist = explosionToTarget.magnitude;
        float relativeDistance = (explosionRadius - explosionDist) / explosionRadius;
        float damage = relativeDistance * Maxdamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.forward = Vector3.Slerp(gameObject.transform.forward, rb.velocity.normalized, Time.deltaTime*10);
    }
}
