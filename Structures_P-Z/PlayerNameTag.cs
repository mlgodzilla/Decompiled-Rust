// Decompiled with JetBrains decompiler
// Type: PlayerNameTag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class PlayerNameTag : MonoBehaviour
{
  public CanvasGroup canvasGroup;
  public Text text;
  public Gradient color;
  public float minDistance;
  public float maxDistance;
  public Vector3 positionOffset;
  public Transform parentBone;

  public PlayerNameTag()
  {
    base.\u002Ector();
  }
}
