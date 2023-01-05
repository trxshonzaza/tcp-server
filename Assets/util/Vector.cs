using System;
using System.Collections.Generic;
using System.Linq;
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

    public bool Equals(double x, double y, double z)
    {
        if(this.x == x && this.y == y && this.z == z)
        {
            return true;
        }

        return false;
    }
}
