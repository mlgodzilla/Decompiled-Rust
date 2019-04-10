// Decompiled with JetBrains decompiler
// Type: GenerateRiverMeshes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class GenerateRiverMeshes : ProceduralComponent
{
  public Material RiverMaterial;
  public PhysicMaterial RiverPhysicMaterial;

  public override void Process(uint seed)
  {
    foreach (PathList river in TerrainMeta.Path.Rivers)
    {
      using (List<Mesh>.Enumerator enumerator = river.CreateMesh().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Mesh current = enumerator.Current;
          GameObject gameObject = new GameObject("River Mesh");
          M0 m0 = gameObject.AddComponent<MeshCollider>();
          ((Collider) m0).set_sharedMaterial(this.RiverPhysicMaterial);
          ((MeshCollider) m0).set_sharedMesh(current);
          gameObject.AddComponent<RiverInfo>();
          gameObject.AddComponent<WaterBody>();
          gameObject.AddComponent<AddToWaterMap>();
          gameObject.set_tag("River");
          gameObject.set_layer(4);
          gameObject.SetHierarchyGroup(river.Name, true, false);
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
