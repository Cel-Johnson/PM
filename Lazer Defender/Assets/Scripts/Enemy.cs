using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100f;

    [SerializeField] int scoreValue = 10;

    [SerializeField] float shotCounter;

    [SerializeField] float maxTimeBetweenShots = 5f;

    [SerializeField] float minTimeBetweenShots = 2f;

    [SerializeField] GameObject projectile;

    [SerializeField] float lazerSpeed = 5f;

    [SerializeField] GameObject deathVFX;

    [SerializeField] float durationOfExplosion = 5f;

    [SerializeField] AudioClip deathSound;

    [SerializeField] [Range(0,1)] float deathVolume = 0.75f;

    [SerializeField] AudioClip lazerSound;

    [SerializeField] [Range(0, 1)] float lazerVolume = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if(shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject EnemyLazer = Instantiate(
            projectile,
            transform.position,
            Quaternion.identity) as GameObject;
        
        EnemyLazer.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -lazerSpeed);
        AudioSource.PlayClipAtPoint(lazerSound, Camera.main.transform.position, lazerVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        if (!damageDealer) { return; }
        damageDealer.Hit();
        if (health < 1)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathVolume);
    }
}
