using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class Mockupcell : SpriteCell
{
    public bool highlighted;
    public bool passable;

    void Start()
    {
        Color = passable ? Color.white : Color.black;
    }

    void Update()
    {
        if(passable)
            Color = highlighted ? Color.red : Color.white;
    }
}
