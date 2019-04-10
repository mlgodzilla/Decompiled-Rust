// Decompiled with JetBrains decompiler
// Type: Impostor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (MeshFilter))]
public class Impostor : MonoBehaviour, IClientComponent
{
  public ImpostorAsset asset;
  [Header("Baking")]
  public GameObject reference;
  public float angle;
  public int resolution;
  public int padding;
  public bool spriteOutlineAsMesh;

  private void OnEnable()
  {
  }

  public Impostor()
  {
    base.\u002Ector();
  }
}
