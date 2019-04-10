// Decompiled with JetBrains decompiler
// Type: GenerateRoadMeshes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class GenerateRoadMeshes : ProceduralComponent
{
  public Material RoadMaterial;
  public PhysicMaterial RoadPhysicMaterial;

  public override void Process(uint seed)
  {
    foreach (PathList road in TerrainMeta.Path.Roads)
    {
      using (List<Mesh>.Enumerator enumerator = road.CreateMesh().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Mesh current = enumerator.Current;
          GameObject gameObject = new GameObject("Road Mesh");
          M0 m0 = gameObject.AddComponent<MeshCollider>();
          ((Collider) m0).set_sharedMaterial(this.RoadPhysicMaterial);
          ((MeshCollider) m0).set_sharedMesh(current);
          gameObject.AddComponent<AddToHeightMap>();
          gameObject.set_layer(16);
          gameObject.SetHierarchyGroup(road.Name, true, false);
        }
      }
    }
  }

  public override bool RunOnCache
  {
    get
    {
      return true;
    }
  }
}
