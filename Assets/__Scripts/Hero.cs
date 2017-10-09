using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero S;

    public float gameRestartDelay = 2f;

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 5;
    
    //Ship status info
    [SerializeField]
    private float _shieldLevel = 1;

    public bool _____________________;

    public Bounds bounds;
    public delegate void WeaponFireDelegate();

    public WeaponFireDelegate fireDelegate;

    void Awake() {
        S = this;
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }
    
	// Update is called once per frame
	void Update () {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        
        //removes extra speed when moving diagonally
        if ((yAxis > 0 && xAxis > 0) || (yAxis < 0 && xAxis > 0) || (yAxis > 0 && xAxis < 0) || (yAxis < 0 && xAxis < 0)) {
            pos.x += (xAxis * speed * Time.deltaTime) / 1.4f;
            pos.y += (yAxis * speed * Time.deltaTime) / 1.4f;
            transform.position = pos;
        } else {
            pos.x += xAxis * speed * Time.deltaTime;
            pos.y += yAxis * speed * Time.deltaTime;
            transform.position = pos;
        }
        
        bounds.center = transform.position;

        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
        if (off != Vector3.zero) {
            pos -= off;
            transform.position = pos;
        }
        
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }
    }

    public GameObject lastTriggerGo = null;

    void OnTriggerEnter(Collider other) {
        GameObject go = Utils.FindTaggedParent(other.gameObject);

        if (go != null) {
            if (go == lastTriggerGo) {
                return;
            }
            lastTriggerGo = go;
            if (go.tag == "Enemy") {
                shieldLevel--;

                Destroy(go);
            } else {
                print("Triggred: " + go.name);
            }
        } else {
            print("Triggered : " + other.gameObject);
        }
    }

    public float shieldLevel {
        get {
            return (_shieldLevel);
        }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0) {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
