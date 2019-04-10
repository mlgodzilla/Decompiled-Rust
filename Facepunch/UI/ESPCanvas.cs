// Decompiled with JetBrains decompiler
// Type: Facepunch.UI.ESPCanvas
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Facepunch.UI
{
  public class ESPCanvas : MonoBehaviour
  {
    [Tooltip("Max amount of elements to show at once")]
    public int MaxElements;
    [Tooltip("Amount of times per second we should update the visible panels")]
    public float RefreshRate;
    [Tooltip("This object will be duplicated in place")]
    public ESPPlayerInfo Source;
    [Tooltip("Entities this far away won't be overlayed")]
    public float MaxDistance;

    public ESPCanvas()
    {
      base.\u002Ector();
    }
  }
}
