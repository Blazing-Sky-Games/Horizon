using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class MockupGridBehavior : GridBehaviour<RectPoint>
{
    private static MockupGridBehavior instance;

    public CharacterControler character;

    public static MockupGridBehavior Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        instance = this;
    }

    private IGrid<Mockupcell, RectPoint> playgrid;

    public override void InitGrid()
    {
        playgrid = Grid.CastValues<Mockupcell, RectPoint>();
        character.init();
    }

    void Update()
    {
        if (character.isMoving == false)
            SetHover(MousePosition);
        else
            playgrid[hover].highlighted = false;
    }

    void OnClick(RectPoint point)
    {
        if(character.isMoving == false && playgrid[point].passable)
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

        if(wasoutside)
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
