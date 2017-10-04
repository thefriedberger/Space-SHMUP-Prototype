using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero S;

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    public float shieldLevel = 1;

    public bool _____________________;

    public Bounds bounds;

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
	}
}
