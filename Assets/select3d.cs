using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public static class vectorPointExtentions
{
    public static Vector2 toVec2(this VectorPoint self)
    {
        return new Vector2(self.X, self.Y);
    }
}

public class select3d : GridBehaviour<RectPoint>
{
    public static select3d instance;

    void Awake()
    {
        instance = this;
    }

    public CharacterControler character;

    private IGrid<Mockupcell, RectPoint> playgrid;

    private Vector2 dimensions;
    private Vector2 cellFactor;
    private Plane gridPlane;

    public override void InitGrid()
    {
        playgrid = Grid.CastValues<Mockupcell, RectPoint>();

        dimensions = GridBuilder.Dimensions.toVec2();
        cellFactor = GridBuilder.CellSpacingFactor;

        Vector3 gridNormal = GridBuilder.Plane == MapPlane.XY ? Vector3.forward: Vector3.up;

        gridPlane = new Plane(gridNormal,transform.position);

        character.init();
    }

    void Update()
    {
        Vector3 CorrectedMousePosision = Input.mousePosition;
        CorrectedMousePosision.z = Camera.main.nearClipPlane;

        Ray mouseRay = Camera.main.ScreenPointToRay(CorrectedMousePosision);

        float enter;
        bool parralel = gridPlane.Raycast(mouseRay, out enter);

        Vector3 planeIntersect = mouseRay.origin + mouseRay.direction * enter;

        int a = (int)Mathf.Ceil(planeIntersect.x / cellFactor.x);
        int b = (int)Mathf.Ceil(planeIntersect.z / cellFactor.y);

        if (GridBuilder.Plane == MapPlane.XY)
        {
            a = (int)Mathf.Ceil(planeIntersect.x / cellFactor.x);
            b = (int)Mathf.Ceil(planeIntersect.y / cellFactor.y);
        }

        RectPoint mousepoint = new RectPoint(a, b) + new RectPoint(4, 4);

        if(Input.GetMouseButtonDown(0) && !playgrid.IsOutside(mousepoint))
        {
            MyOnClick(mousepoint);
        }

        if (character.isMoving == false)
            SetHover(mousepoint);
        else
            playgrid[hover].highlighted = false;  
    }

    void MyOnClick(RectPoint point)
    {
        if (character.isMoving == false && playgrid[point].passable)
        {
            character.setDest(point);
        }
    }

    private void SetHover(RectPoint currenthover)
    {
        if (playgrid.IsOutside(currenthover))
        {
            playgrid[hover].highlighted = false;
            wasoutside = true;
            return;
        }

        if (wasoutside)
        {
            playgrid[currenthover].highlighted = true;
            hover = currenthover;
            wasoutside = false;
            return;
        }

        if (currenthover != hover)
        {
            playgrid[currenthover].highlighted = true;
            playgrid[hover].highlighted = false;
            hover = currenthover;
        }
    }
    private RectPoint hover;
    private bool wasoutside = false;
}
