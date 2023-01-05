using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class Vector
{
    public double x,
        y,
        z;

    public Vector(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector()
    {

    }

    public Vector3 toVector3()
    {
        return new Vector3((float)x, (float)y, (float)z);
    }

    public static Vector fromVector3(Vector3 v)
    {
        return new Vector(v.x, v.y, v.z);
    }

    public bool Equals(double x, double y, double z)
    {
        if(this.x == x && this.y == y && this.z == z)
        {
            return true;
        }

        return false;
    }
}
