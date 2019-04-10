// Decompiled with JetBrains decompiler
// Type: Gibbable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Gibbable : MonoBehaviour, IClientComponent
{
  public GameObject gibSource;
  public GameObject materialSource;
  public bool copyMaterialBlock;
  public PhysicMaterial physicsMaterial;
  public GameObjectRef fxPrefab;
  public bool spawnFxPrefab;
  [Tooltip("If enabled, gibs will spawn even though we've hit a gib limit")]
  public bool important;
  public float explodeScale;

  public Gibbable()
  {
    base.\u002Ector();
  }
}
