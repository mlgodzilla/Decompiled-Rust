// Decompiled with JetBrains decompiler
// Type: MapGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class MapGrid : MonoBehaviour
{
  public Text coordinatePrefab;
  public int gridCellSize;
  public float lineThickness;
  public CanvasGroup group;
  public float visibleAlphaLevel;
  public RawImage TargetImage;
  public bool show;

  public MapGrid()
  {
    base.\u002Ector();
  }
}
