using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
} 

public class BoatPlacement
{
    public int x;
    public int y;
    public int len;
    public int rot;

    public bool DetectIfEdgesOK()
    {
        if (rot == 0 && x + len > 9) return false;
        if (rot == 1 && y + len > 9) return false;
        if (rot == 2 && x - len < 0) return false;
        if (rot == 3 && y - len < 0) return false;
        return true;
    }

    public Point LocationAtIndex(int index)
    {
        switch (rot)
        {
            case 0:
                return new Point(x + index, y);
            case 1:
                return new Point(x, y + index);
            case 2:
                return new Point(x - index, y);
            case 3:
                return new Point(x, y - index);
            default:
                return new Point(x, y);
        }
    }

    public bool DetectCollisionsOK(BoatPlacement otherBoat)
    {
        for (int i = 0; i <= len; i++)
        {
            for (int j = 0; j <= otherBoat.len; j++)
            {
                Point p1 = LocationAtIndex(i);
                Point p2 = otherBoat.LocationAtIndex(j);

                if (p1.x == p2.x && p1.y == p2.y)
                {
                    return false; //kolizija
                }
            }
        }


        return true;
    }
}

public class DragManager : MonoBehaviour
{
    public Transform lastHover;
    int lastX, lastY;
    public int hoverAmounts = 0;

    List<BoatPlacement> positions;

    string map= "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000+" +
                "0000000000";

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<BoatPlacement>();
    }

    public Point TryPlace(int len, int rot, Transform txf)
    {
        if (hoverAmounts == 0) return new Point(-1, -1);

        //string[] boardstate = map.Split('+');

        BoatPlacement nb = new BoatPlacement();

        nb.x = lastX;
        nb.y = lastY;
        nb.rot = rot;
        nb.len = len;

        if (!nb.DetectIfEdgesOK()) return new Point(-1, -1);

        for (int i = 0; i < positions.Count; i++)
        {
            if (!nb.DetectCollisionsOK(positions[i])) return new Point(-1, -1);
        }

        positions.Add(nb);

        txf.position = lastHover.position;
        txf.localPosition += new Vector3(16, -16, 0);

        return new Point(lastX, lastY);
    }

    public void RemovePlace(Point pt)
    {
        for (int i = 0; i < positions.Count; i++ )
        {
            if (positions[i].x == pt.x && positions[i].y == pt.y)
            {
                positions.RemoveAt(i);
                return;
            }
        }
        return;
    }

    public string ReturnFilledMap()
    {
        string[] boardstate = map.Split('+');

        for (int k = 0; k < positions.Count; k++)
        {
            for (int i = 0; i <= positions[k].len; i++)
            {
                Point p = positions[k].LocationAtIndex(i);
                boardstate[p.y] = boardstate[p.y].Substring(0, p.x) + "2" + boardstate[p.y].Substring(p.x + 1);
            }
        }

        string nmap = "";

        for (int i = 0; i < boardstate.Length - 1; i++)
        {
            nmap += boardstate[i] + "+";
        }
        nmap += boardstate[boardstate.Length - 1];

        return nmap;
    }

    // Update is called once per frame
    public void UpdateHover(Transform n, int x, int y)
    {
        lastHover = n;
        hoverAmounts++;
        lastX = x;
        lastY = y;
    }

    public void EndHover()
    {
        hoverAmounts--;
    }
}
