using UnityEngine;
using System.Collections;
using Gamelogic.Grids;
using System;

public class CharacterControler : MonoBehaviour
{
    private RectPoint pos;
    private RectPoint destination;
    public bool isMoving = false;

    private RectMap map = new RectMap(new Vector2(1, 1));

    public float speed;

    void Awake()
    {
        pos = map.RawWorldToGrid((Vector2)transform.position + new Vector2(4,4));
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
            MockupGridBehavior.Instance.Grid.CastValues<Mockupcell, RectPoint>(), 
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
            Vector2 worldpos = map.GridToWorld(pos);
            Vector2 worldNext = map.GridToWorld(nextPos);

            float elapsed = 0;
            float to_elapse = (worldNext - worldpos).magnitude / speed;
            while(elapsed < to_elapse)
            {
                elapsed += Time.deltaTime;

                transform.position = Vector2.Lerp(worldpos, worldNext, elapsed / to_elapse) - new Vector2(4.5f,4.5f);

                yield return 0;
            }
            transform.position = worldNext;
            pos = nextPos;
        }
        transform.position = map.GridToWorld(destination) - new Vector2(4.5f, 4.5f);
        pos = destination;

        isMoving = false;
    }
}
