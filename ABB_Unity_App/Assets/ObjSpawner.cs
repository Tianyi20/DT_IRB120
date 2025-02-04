using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;




public class ObjSpawner : MonoBehaviour, ICanRegisterEvent, ICanSendEvent
{
    [SerializeField] Transform spawner;
    [SerializeField] GameObject Sphere;
    [SerializeField] GameObject Rectangle;
    [SerializeField] GameObject Cube;
    [SerializeField] GameObject head;

    [SerializeField] ConveyorsWithModel conveyor;
    //[SerializeField] BoxCollider EndTrigger;

    (ShapeType,GameObject) CurrentShapeObjPair;
   

    GameObject sphere_clone;
    GameObject rectangle_clone;
    GameObject cube_clone;
    public static string[] shape_last = { "0.0", "0.0", "0.0" };
    public static string[] magnetic_last = { "0.0" };
    public static int test = 0;
    // shape_last [0] 作为上一次Rectangle的判断
    // shape_last [1] 作为上一次Cube的判断
    //shape_last [1] 作为上一次Sphere的判断
    public IArchitecture GetArchitecture()
    {
        return Abb.Interface;
    }


    void Awake() {
        this.RegisterEvent<SpawnObjEvent>(OnSpawnObj).UnRegisterWhenGameObjectDestroyed(gameObject);
        //this.RegisterEvent<OnTriggerEnterEventTrigger>(OnTriggerEnter) EndTrigger.OnTriggerEnterEvent += OnTriggerEnter;

        this.RegisterEvent<MagEvent>(OnMag).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<ConveyerEvent>(OnConveyer).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<UltraRedEvent>(OnUltraRed).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<StackingEvent>(OnStacking).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void OnMag(MagEvent e)
    {
        if (CurrentShapeObjPair.Item2 == null) return;

        if (e.IsOn)
        {
            CurrentShapeObjPair.Item2.transform.GetOrAddComponent<Rigidbody>().isKinematic = true;
            CurrentShapeObjPair.Item2.transform.SetParent(head.transform);
            CurrentShapeObjPair.Item2.transform.localPosition = Vector3.zero;
        }
        else 
        {
            CurrentShapeObjPair.Item2.transform.SetParent(null);
            CurrentShapeObjPair.Item2.transform.GetOrAddComponent<Rigidbody>().isKinematic = false;
        }
    }

    void OnConveyer(ConveyerEvent e)
    {
        if (e.IsOn)
        {
            conveyor.speed = 0.52f;
        }
        else
        {
            conveyor.speed = 0f;
        }
    }
    void OnUltraRed(UltraRedEvent e)
    {
        Debug.Log(nameof(OnUltraRed));
    }

    void OnStacking(StackingEvent e)
    {
        if(e.IsOn)
        {
            this.SendEvent(new SpawnObjEvent { type = e.shapeType });
        }
    }



    void OnSpawnObj(SpawnObjEvent e)
    {
        if (e.type == ShapeType.Sphere)
        {
            sphere_clone = Instantiate(Sphere, spawner.transform);
            CurrentShapeObjPair = (e.type,sphere_clone);
        }
        else if (e.type == ShapeType.Rectangle)
        {
            rectangle_clone= Instantiate(Rectangle, spawner.transform);
            CurrentShapeObjPair = (e.type, rectangle_clone);
        }
        else if (e.type == ShapeType.Cube) {
            cube_clone = Instantiate(Cube, spawner.transform);
            CurrentShapeObjPair = (e.type, cube_clone);
        }
    }

    




    //void Update() 
    //{

    //    if (shape_last[0] == "0.0" && GlobalVariables_RWS_client.robotiosystemDo[2]=="1")
    //    {
    //        this.SendEvent(new SpawnObjEvent { obj = ShapeType.Sphere });
    //        shape_last[0] = "1.0";
    //    }
    //    else if (shape_last[1] == "0.0" && GlobalVariables_RWS_client.robotiosystemDo[3] == "1")
    //    {
    //        this.SendEvent(new SpawnObjEvent { obj = ShapeType.Cube });
    //        shape_last[1] = "1.0";
    //    }
    //    else if (shape_last[2] == "0.0" && GlobalVariables_RWS_client.robotiosystemDo[4] == "1")
    //    {
    //        this.SendEvent(new SpawnObjEvent { obj = ShapeType.Rectangle });
    //        shape_last[2] = "1.0";
    //    }

    //    if (GlobalVariables_RWS_client.robotiosystemDo[2] == "0" && GlobalVariables_RWS_client.robotiosystemDo[3] == "0" && GlobalVariables_RWS_client.robotiosystemDo[4] == "0")
    //    {
    //        shape_last[0] = "0.0";
    //        shape_last[1] = "0.0";
    //        shape_last[2] = "0.0";
    //    }





    //    if (GlobalVariables_RWS_client.robotiosystemDo[0] == "1" && GlobalVariables_RWS_client.robotiosystemDo[2] == "1" && magnetic_last[0] == "0.0")
    //    {
    //        sphere_clone.transform.parent = Jixiezhua.transform;
    //        magnetic_last[0]= "1.0";
    //    }
    //    else if (GlobalVariables_RWS_client.robotiosystemDo[0] == "1" && GlobalVariables_RWS_client.robotiosystemDo[3] == "1" && magnetic_last[0] == "0.0")
    //    {
    //        cube_clone.transform.parent = Jixiezhua.transform;
    //        magnetic_last[0] = "1.0";
    //    }
    //    else if (GlobalVariables_RWS_client.robotiosystemDo[0] == "1" && GlobalVariables_RWS_client.robotiosystemDo[4] == "1" && magnetic_last[0] == "0.0")
    //    {
    //        rectangle_clone.transform.parent = Jixiezhua.transform;
    //        magnetic_last[0] = "1.0";
    //    }
        
        
    //    if(magnetic_last[0] == "1.0" && GlobalVariables_RWS_client.robotiosystemDo[0] == "0.0" )
    //    {
    //        sphere_clone.transform.parent = null;
    //        rectangle_clone.transform.parent = null;
    //        cube_clone.transform.parent = null;
    //        magnetic_last[0] = "0.0";
    //        Debug.Log("in");
    //    }
        

    //    //UnityEngine.Debug.Log(GlobalVariables_RWS_client.robotiosystemDo[1]);
    //    if (GlobalVariables_RWS_client.robotiosystemDo[1] == "0")
    //    {
    //        conveyor.speed = 0f;
    //    }
    //    else
    //    {
    //        conveyor.speed = 1f;
    //    }




    
    //}
}

public class SpawnObjEvent 
{
    public ShapeType type;
}

public enum ShapeType
{
    Sphere,
    Rectangle,
    Cube
}


public class Abb: Architecture<Abb>
{
    protected override void Init() 
    {

    }
}

public class BoolEvent
{
    public bool IsOn;
}

public class MagEvent : BoolEvent
{

}

public class ConveyerEvent : BoolEvent
{

}

public class UltraRedEvent : BoolEvent
{

}

public class StackingEvent : BoolEvent
{
    public ShapeType shapeType;
}