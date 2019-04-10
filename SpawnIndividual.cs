// Decompiled with JetBrains decompiler
// Type: SpawnIndividual
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct SpawnIndividual
{
  public uint PrefabID;
  public Vector3 Position;
  public Quaternion Rotation;

  public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
  {
    this.PrefabID = prefabID;
    this.Position = position;
    this.Rotation = rotation;
  }
}
