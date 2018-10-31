using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackType : MonoBehaviour {

    public float degreeResolution;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //CircleAttack(new Vector3(0, 0, 0), new Vector3(-3, 3, 0), 7f, 2f, 1 << 8, false, Color.red);

    }

    //This function performs a ray attack, a single line in one direction
    //Mask is used for targeting specific layers of the map (only walls or walls and creatures)
    public void RayAttack(Vector3 origin, Vector3 direction, float range, int mask, bool targetSelf, Color color)
    {

        //Find a target
        RaycastHit2D target = GetTarget(origin, direction, range, mask, targetSelf);

        Vector3 target3 = target.point;

        //if a target is found, draw to it, otherwise draw to max range
        if (target.collider == null)
            DrawRay(origin, origin + direction.normalized * range, color);
        else
            DrawRay(origin, target.point, color);

    }

    //This function performs a cone attack, which is several ray attacks in an arc.
    //Degrees is the size of the attack arc, in both a positive and negative rotation from the target direction.
    public void ConeAttack(Vector3 origin, Vector3 direction, float range, float degrees, int mask, bool targetSelf, Color color)
    {
        //starting rotating negative degrees from the target direction, perform multiple ray attacks.
        for (float i = -degrees/2; i < degrees; i += degreeResolution )
        {
            //avoid creating too many ray attacks
            if (degreeResolution <= 0.01)
                throw new System.ArgumentOutOfRangeException();

        
            RayAttack(origin, Quaternion.Euler(0, 0, i) * direction, range, mask, targetSelf, color);
        }
    }

    //A circle attack has a special property of not having a direction, since rays are cast in all directions
    //Origin is the place the spell was cast, and end is the place the spell is being cast to
    //Range1 is the maximum distance from the origin point, and range2 is the maximum distance rays are cast from the end point
    public void CircleAttack(Vector3 origin, Vector3 end, float range1, float range2, int mask, bool targetSelf, Color color)
    {
        float range = range1;

        //first, we determine if our endpoint is outside our maximum range, if it is, we cap it
        if ((end - origin).magnitude > range1)
            range = range1;
        else
            range = (end - origin).magnitude;

        //next, we determine if there is something between our end point and our origin
        RaycastHit2D target = GetTarget(origin, end - origin, range, mask, false);

        Vector3 target3 = target.point;
        Vector3 offset = target3 - (0.01f) * (end - origin);

        //if there isn't, we cast the circle attack either at the end position or the maximum range
        if (target.collider == null)
        {
            if ((end - origin).magnitude < range1)
                ConeAttack(end, Vector3.up, range2, 180, mask, targetSelf, color);
            else
                ConeAttack((end - origin).normalized * range1 + origin, Vector3.up, range2, 180, mask, targetSelf, color);
        }
        //if there is, we move the origin off the contact point (to avoid extra collisions with the contact point) and then make our circle.
        else
            ConeAttack(offset, Vector3.up, range2, 180, mask, targetSelf, color);

    }

    //Shoots a raycast in a direction from the start to a maximum of range, only targeting layers in layermask.
    protected RaycastHit2D GetTarget(Vector3 start, Vector3 direction, float range, int layerMask, bool targetSelf)
    {

        if (!targetSelf)
            GetComponent<Collider2D>().enabled = false;

        RaycastHit2D hit;

        hit = Physics2D.Raycast(start, direction, range, layerMask);

        GetComponent<Collider2D>().enabled = true;

        return hit;

    }

    //Draws a line from start to end in color.
    protected void DrawRay(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    
        GameObject.Destroy(myLine, Time.deltaTime);
    }

    //returns the current mouse position
    public Vector3? GetCurrentMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);

        }
        return null;
    }

}


