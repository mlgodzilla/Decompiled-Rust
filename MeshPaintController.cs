// Decompiled with JetBrains decompiler
// Type: MeshPaintController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class MeshPaintController : MonoBehaviour, IClientComponent
{
  public Camera pickerCamera;
  public Texture2D brushTexture;
  public Vector2 brushScale;
  public Color brushColor;
  public float brushSpacing;
  public RawImage brushImage;
  private Vector3 lastPosition;

  public MeshPaintController()
  {
    base.\u002Ector();
  }
}
