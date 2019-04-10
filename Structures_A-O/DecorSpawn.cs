// Decompiled with JetBrains decompiler
// Type: DecorSpawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorSpawn : MonoBehaviour, IClientComponent
{
  public SpawnFilter Filter;
  public string ResourceFolder;
  public uint Seed;
  public float ObjectCutoff;
  public float ObjectTapering;
  public int ObjectsPerPatch;
  public float ClusterRadius;
  public int ClusterSizeMin;
  public int ClusterSizeMax;
  public int PatchCount;
  public int PatchSize;
  public bool LOD;

  public DecorSpawn()
  {
    base.\u002Ector();
  }
}
