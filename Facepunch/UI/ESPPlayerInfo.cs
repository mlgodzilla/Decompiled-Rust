// Decompiled with JetBrains decompiler
// Type: Facepunch.UI.ESPPlayerInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace Facepunch.UI
{
  public class ESPPlayerInfo : MonoBehaviour
  {
    public Vector3 WorldOffset;
    protected Text Text;
    protected Image Image;
    public Gradient gradientNormal;
    public Gradient gradientTeam;
    public QueryVis visCheck;

    public BasePlayer Entity { get; set; }

    public ESPPlayerInfo()
    {
      base.\u002Ector();
    }
  }
}
