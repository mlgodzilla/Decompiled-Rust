// Decompiled with JetBrains decompiler
// Type: NewsButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class NewsButton : MonoBehaviour
{
  public int storyNumber;
  public NewsSource.Story story;
  public CanvasGroup canvasGroup;
  public Text text;
  public Text author;
  public RawImage image;
  private float randomness;

  public NewsButton()
  {
    base.\u002Ector();
  }
}
