using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class lightsOutGridBehavior : GridBehaviour<RectPoint>
{
    private IGrid<LightsOutCell, RectPoint> lightsOutGrid;

    public override void InitGrid ()
	{
		lightsOutGrid = Grid.CastValues<LightsOutCell, RectPoint>();

        foreach(var point in lightsOutGrid)
        {
            lightsOutGrid[point].IsOn = Random.Range(0, 2) == 1;
        }
	}

    private void ToggleNeighbors(RectPoint point)
    {
        var neighbors = lightsOutGrid.GetNeighbors(point);

        foreach(var neighbor in neighbors)
        {
            lightsOutGrid[neighbor].IsOn = !lightsOutGrid[neighbor].IsOn;
        }
    }

    public void OnClick(RectPoint point)
    {
        ToggleNeighbors(point);
    }
}
