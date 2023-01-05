using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Unity.VisualScripting;

public abstract class Packet
{
    private BinaryWriter writer;
    private BinaryReader reader;

    public Packet(BinaryWriter writer, BinaryReader reader)
    {
        this.writer = writer;
        this.reader = reader;
    }

    public int InsertInt(int _)
    {
        writer.Write(_);
        return _;
    }

    public byte InsertByte(byte _)
    {
        writer.Write((byte)_);
        return _;
    }

    public sbyte InsertSByte(sbyte _)
    {
        writer.Write((sbyte)_);
        return _;
    }

    public long InsertLong(long _)
    {
        writer.Write(_);
        return _;
    }

    public bool InsertBool(bool _)
    {
        writer.Write(_);
        return _;
    }

    public uint InsertUint(uint _)
    {
        writer.Write(_);
        return _;
    }

    public short InsertShort(short _)
    {
        writer.Write(_);
        return _;
    }

    public string InsertString(string _)
    {
        writer.Write(_);
        return _;
    }

    public double InsertDouble(double _)
    {
        writer.Write(_);
        return _;
    }

    public ulong InsertUlong(ulong _)
    {
        writer.Write(_);
        return _;
    }

    public decimal InsertDecimal(decimal _)
    {
        writer.Write(_);
        return _;
    }

    public Vector InsertVector(double x, double y, double z)
    {
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);

        return new Vector(x, y, z);
    }

    public byte[] InsertBytes(byte[] bytes)
    {
        writer.Write(bytes);

        return bytes;
    }

    public int ReadInt()
    {
        int read = reader.ReadInt32();
        return read;
    }

    public long ReadLong()
    {
        long read = reader.ReadInt64();
        return read;
    }

    public bool ReadBool()
    {
        bool read = reader.ReadBoolean();
        return read;
    }

    public uint ReadUint()
    {
        uint read = reader.ReadUInt32();
        return read;
    }

    public short ReadShort()
    {
        short read = reader.ReadInt16();
        return read;
    }

    public string ReadString()
    {
        string read = reader.ReadString();
        return read;
    }

    public double ReadDouble()
    {
        double read = reader.ReadDouble();
        return read;
    }

    public ulong ReadUlong()
    {
        ulong read = reader.ReadUInt64();
        return read;
    }

    public byte ReadByte()
    {
        byte read = reader.ReadByte();
        return read;
    }

    public sbyte ReadSByte()
    {
        sbyte read = reader.ReadSByte();
        return read;
    }

    public decimal ReadDecimal()
    {
        decimal read = reader.ReadDecimal();
        return read;
    }

    public Vector ReadVector()
    {
        float x = (float)reader.ReadDouble();
        float y = (float)reader.ReadDouble();
        float z = (float)reader.ReadDouble();

        return new Vector(x, y, z);
    }

    public byte[] ReadBytes(long length)
    {
        byte[] bytes = reader.ReadBytes((int)length);

        return bytes;
    }

    public void Send()
    {
        writer.BaseStream.Flush();
    }

    public abstract void ReadPacket();
    public abstract void WritePacket();

    public Client SetReaderAndWriter(Client client)
    {
        this.reader = client.GetReader();
        this.writer = client.GetWriter();
        return client;
    }
} 
