﻿using System.Collections;
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

    //Weapon fields
    public Weapon[] weapons;

    public bool _____________________;

    public Bounds bounds;

    public delegate void WeaponFireDelegate();

    public WeaponFireDelegate fireDelegate;

    void Awake() {
        S = this;
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }

    void Start () {
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
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

        if (go != null) 
            if (go == lastTriggerGo) {
                return;
            }

        lastTriggerGo = go;
        if (go.tag == "Enemy") {
            shieldLevel--;
            Destroy(go);
        } else if (go.tag == "PowerUp") {
            AbsorbPowerUp(go);
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
    
    public void AbsorbPowerUp (GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield: //If it's the shield
                shieldLevel++;
                break;

            default: //if it's any weapon powerup
                if (pu.type == weapons[0].type) {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) {
                        w.SetType(pu.type);
                    }
                } else {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
    Weapon GetEmptyWeaponSlot() {
        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i].type == WeaponType.none) {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }
}
