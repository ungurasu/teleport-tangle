using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalLibrary 
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int initX, int initY)
        {
            x = initX;
            y = initY;
        }

        public static float operator -(Point a, Point b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }
    }

    enum Direction : int
    {
        None = -1,
        Bottom = 0,
        Left = 1,
        Top = 2,
        Right = 3
    }

    enum MazeObjects : int
    {
        Floor = 0,
        Wall = 1,
        Obstacle = 2,
        Enemy = 3,
        Tangle = 4,
        Start = 5,
        End = 6
    }

    enum PlayerStates : int
    { 
        ListenInput = 0,
        Moving = 1,
        ListenObstacleTeleport = 2,
        FadeOut = 3,
        FadeIn = 4
    }

    enum MenuStates : int
    {
        ListenInput = 0,
        FadeOut = 1
    }
}

