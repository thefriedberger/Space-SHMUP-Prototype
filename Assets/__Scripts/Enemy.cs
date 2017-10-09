using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;

    public bool ______________________;

    public Bounds bounds;
    public Vector3 boundsCenterOffset;
	
    void Awake () {
        InvokeRepeating("CheckOffScreen", 0f, 2f);
    }
	// Update is called once per frame
	void Update () {
        Move();
	}

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    public Vector3 pos {
        get {
            return (this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }

    void CheckOffScreen () {
        if (bounds.size == Vector3.zero) {
            bounds = Utils.CombineBoundsOfChildren(this.gameObject);

            boundsCenterOffset = bounds.center - transform.position;
        }

        bounds.center = transform.position + boundsCenterOffset;
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
        if (off != Vector3.zero) {
            if(off.y < 0) {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        GameObject other = collision.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                bounds.center = transform.position + boundsCenterOffset;
                if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
                    Destroy(other);
                    break;
                }
                health -= Main.W_DEFS[p.type].damageOnHit;
                if (health <= 0) {
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
