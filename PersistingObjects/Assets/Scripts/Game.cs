using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Game : PersistableObject
{
    const int saveVersion = 1;
    public PersistentStorage storage;

    public ShapeFactory shapeFactory;
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    List<Shape> shapes;

    public float CreationSpeed { get; set; }
    float creationProgress;

    public float DestructionSpeed { get; set; }
    float destructionProgress;
    

    private void Awake()
    {
        shapes = new List<Shape>();

       
    }
    private void Update()
    {
        creationProgress += Time.deltaTime * CreationSpeed;

        while(creationProgress >= 1)
        {
            creationProgress -= 1;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;

        while(destructionProgress >= 1)
        {
            destructionProgress -= 1;
            DestroyShape();
        }




        if (Input.GetKeyDown(createKey))
        {
            CreateShape();   
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        //writer.Write(-saveVersion); //now in persistentStorage.Save() //write version negative, so it can be checked against older versions that didnt have a version
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }
    
    public override void Load(GameDataReader reader)
    {
        int version = reader.Version; //use negative version, in case its an old File, that didnt have version included yet
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported Future Save Version " + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt(); //if inverted! version nr is smaller than 0, its acutally the count (there is no version then)
        for (int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(.1f, 2f);
        instance.SetColor(Random.ColorHSV(
            hueMin: 0f,hueMax: 1f,
            saturationMin: 0.5f,saturationMax: 1f,
            valueMin: 0.25f,valueMax: 1f,
            alphaMin: 1f,alphaMax: 1f));
        shapes.Add(instance);
    }

    void DestroyShape()
    {
        if(shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
        
    }

    void BeginNewGame()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

   
}
