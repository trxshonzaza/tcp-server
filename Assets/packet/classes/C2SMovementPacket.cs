using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class C2SMovementPacket : Packet
{
    public C2SMovementPacket(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
    {

    }

    public string username;

    public Vector moveDirection,
       slopeMoveDirection,
        velocity,
        absolutePos;

    public float movementMultiplier,
        airMultiplier,
        moveSpeed,
        currentDrag;

    public bool isGrounded;


    public override void ReadPacket()
    {
        username = ReadString();
        moveDirection = ReadVector();
        slopeMoveDirection = ReadVector();
        velocity = ReadVector();
        absolutePos = ReadVector();
        movementMultiplier = (float)ReadDouble();
        airMultiplier = (float)ReadDouble();
        moveSpeed = (float)ReadDouble();
        isGrounded = ReadBool();
        currentDrag = (float)ReadDouble();
    }

    public override void WritePacket()
    {
        InsertByte((byte)0x2A);
        InsertString(username);
        InsertVector(moveDirection.x, moveDirection.y, moveDirection.z);
        InsertVector(slopeMoveDirection.x, slopeMoveDirection.y, slopeMoveDirection.z);
        InsertVector(velocity.x, velocity.y, velocity.z);
        InsertVector(absolutePos.x, absolutePos.y, absolutePos.z);
        InsertDouble(movementMultiplier);
        InsertDouble(airMultiplier);
        InsertDouble(moveSpeed);
        InsertBool(isGrounded);
        InsertDouble(currentDrag);
        Send();
    }
}
