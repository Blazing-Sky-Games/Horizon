using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using System;

public class CharacterControler : MonoBehaviour
{
    private RectPoint pos;
    private RectPoint destination;
    public bool isMoving = false;

    public float speed;

    public void init()
    {
        pos = select3d.instance.Map[transform.position];
        destination = pos;
    }

    public void setDest(RectPoint newdestination)
    {
        if (isMoving) return;

        if(newdestination!=pos)
        {
            destination = newdestination;
            StartCoroutine(goToDest());
        }
    }

    IEnumerator goToDest()
    {
        isMoving = true;

        Func<Mockupcell, bool> acc = (Mockupcell c) => 
        {
            return c.passable;
        };

        Func<RectPoint, RectPoint, float> h = (RectPoint p, RectPoint q) =>
        {
            return p.DistanceFrom(q);
        };

        Func<RectPoint, RectPoint, float> nd = (RectPoint p, RectPoint q) =>
        {
            return 1;
        };

        var path = Algorithms.AStar<Mockupcell,RectPoint>(
            select3d.instance.Grid.CastValues<Mockupcell, RectPoint>(), 
            pos, 
            destination,
            h,
            acc,
            nd
         );

        RectPoint nextPos;
        foreach(RectPoint point in path)
        {
            nextPos = point;
            Vector3 worldpos = select3d.instance.Map[pos];
            Vector3 worldNext = select3d.instance.Map[nextPos];
            worldpos.y = transform.position.y;
            worldNext.y = worldpos.y;

            float elapsed = 0;
            float to_elapse = (worldNext - worldpos).magnitude / speed;
            while(elapsed < to_elapse)
            {
                elapsed += Time.deltaTime;

                transform.position = Vector3.Lerp(worldpos, worldNext, elapsed / to_elapse);

                yield return 0;
            }
            transform.position = worldNext;
            pos = nextPos;
        }
        float oldy = transform.position.y;
        transform.position = select3d.instance.Map[destination];
        transform.position = new Vector3(transform.position.x, oldy, transform.position.z);
        pos = destination;

        isMoving = false;
    }
}
