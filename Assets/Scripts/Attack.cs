using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public enum AttackShape { Ray, Cone, Circle}

    public AttackShape attackShape;
    public float rangePrimary;
    public float rangeSecondary;
    public float coneDegrees;
    public bool targetSelf;
    public bool targetObjects;
    public float originOffset;
    public Color attackColor;

    private Vector3 attacker;
    private Vector3 mousePosition;
    private AttackType attackThing;

	// Use this for initialization
	void Start ()
    {
        attacker = GetComponent<Transform>().position;
        attackThing = GetComponent<AttackType>();
        mousePosition = attackThing.GetCurrentMousePosition().GetValueOrDefault();
    }
	
	// Update is called once per frame
	void Update ()
    {
        mousePosition = attackThing.GetCurrentMousePosition().GetValueOrDefault();
        switch (attackShape)
        {
            case AttackShape.Ray:
                attackThing.RayAttack(attacker, (mousePosition - attacker), rangePrimary, GetLayerMask(), targetSelf, attackColor); break;
            case AttackShape.Cone:
                attackThing.ConeAttack(attacker, (mousePosition - attacker), rangePrimary, coneDegrees, GetLayerMask(), targetSelf, attackColor); break;
            case AttackShape.Circle:
                attackThing.CircleAttack(attacker, mousePosition, rangePrimary, rangeSecondary, GetLayerMask(), targetSelf, attackColor); break;
            default: break;
        }
        attacker = GetComponent<Transform>().position;
    }

    int GetLayerMask()
    {
        int layerMask = 1 << 8;
        if (targetObjects)
            layerMask += 1 << 10;

        return layerMask;
    }

}
