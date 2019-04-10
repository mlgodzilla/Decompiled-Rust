// Decompiled with JetBrains decompiler
// Type: MapInterface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class MapInterface : SingletonComponent<MapInterface>
{
  public static bool IsOpen;
  public RawImage mapImage;
  public Image cameraPositon;
  public ScrollRectEx scrollRect;
  public PaintableImageGrid paintGrid;
  public UIPaintBox paintBox;
  public Toggle showGridToggle;
  public Button FocusButton;
  public GameObject monumentMarkerContainer;
  public GameObject monumentMarkerPrefab;
  public CanvasGroup CanvasGroup;
  public bool followingPlayer;

  public MapInterface()
  {
    base.\u002Ector();
  }
}
