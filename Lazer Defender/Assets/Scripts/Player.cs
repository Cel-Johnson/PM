using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]

    [SerializeField] float health = 200f;

    [SerializeField] float moveSpeedX = 10f;

    [SerializeField] float moveSpeedY = 10f;

    [SerializeField] float padding = 0.3f;

    [SerializeField] AudioClip deathSound;

    [SerializeField] [Range(0, 1)] float deathVolume = 0.75f;

    [SerializeField] GameObject deathVFX;

    [SerializeField] float durationOfExplosion = 5f;

    [Header("Projectile")]

    [SerializeField] GameObject lazerPrefab;

    [SerializeField] float lazerSpeed = 5f;

    [SerializeField] float projectileFiringPeriod = 0.2f;

    Coroutine firingCoroutine;

    [SerializeField] AudioClip lazerSound;

    [SerializeField] [Range(0, 1)] float lazerVolume = 0.75f;



    float xMin;
    float xMax;
    float yMin;
    float yMax;
    void Start()
    {
        setupMoveBoundaries();
    }

   

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireRapidly());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }
    IEnumerator FireRapidly()
    {
        while (true)
        {
            GameObject laser = Instantiate(lazerPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, lazerSpeed);
            AudioSource.PlayClipAtPoint(lazerSound, Camera.main.transform.position, lazerVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }
    private void Move()
    {
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeedY;
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeedX;

        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        transform.position = new Vector2(newXPos, newYPos);
    }
    private void setupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health < 1)
        {

            FindObjectOfType<Level>().LoadGameOver();

            Destroy(gameObject);
            GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
            Destroy(explosion, durationOfExplosion);
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathVolume);
        }
    }
}
