using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {


    public static Bounds BoundsUnion (Bounds b0, Bounds b1) {
        if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
            return (b1);
        } else if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
            return (b0);
        } else if (b0.size == Vector3.zero && b1.size != Vector3.zero) {
            return (b0);
        }

        b0.Encapsulate(b1.min);
        b0.Encapsulate(b1.max);
        return (b0);
    }

    public static Bounds CombineBoundsOfChildren (GameObject go) {
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);

        if(go.GetComponent<Renderer>() != null) {
            b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
        }
        if (go.GetComponent<Collider>() != null) {
            b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
        }
        foreach (Transform t in go.transform) {
            b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
        }

        return (b);
    }

    static public Bounds camBounds {
        get {
            if(_camBounds.size == Vector3.zero) {
                SetCameraBounds();
            }
            return (_camBounds);
        }
    }

    static private Bounds _camBounds;

    public static void SetCameraBounds (Camera cam=null) {
        if (cam == null) cam = Camera.main;

        Vector3 topLeft = new Vector3(0, 0, 0);
        Vector3 bottomRight = new Vector3(Screen.width, Screen.height, 0);

        Vector3 boundTLN = cam.ScreenToWorldPoint(topLeft);
        Vector3 boundBRF = cam.ScreenToWorldPoint(bottomRight);

        boundTLN.z += cam.nearClipPlane;
        boundBRF.z += cam.nearClipPlane;

        Vector3 center = (boundTLN + boundBRF) / 2f;
        _camBounds = new Bounds(center, Vector3.zero);

        _camBounds.Encapsulate(boundTLN);
        _camBounds.Encapsulate(boundBRF);
    }
}
