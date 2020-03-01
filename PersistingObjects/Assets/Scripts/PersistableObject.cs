using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour
{
    int shapeId = int.MinValue;
    public int ShapeId {
        get { 
            return shapeId; 
        }
        set {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId");
            }
        }
    }

    
    public virtual void Save(GameDataWriter writer)
    {
        writer.Write(transform.localPosition);
        writer.Write(transform.localRotation);
        writer.Write(transform.localScale);    
    }

    public virtual void Load(GameDataReader reader)
    {
        transform.localPosition = reader.ReadVector3();
        transform.localRotation = reader.ReadQuaternion();
        transform.localScale = reader.ReadVector3();
    }
}
