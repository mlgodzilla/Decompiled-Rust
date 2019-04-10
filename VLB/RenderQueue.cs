// Decompiled with JetBrains decompiler
// Type: VLB.RenderQueue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace VLB
{
  public enum RenderQueue
  {
    Custom = 0,
    Background = 1000, // 0x000003E8
    Geometry = 2000, // 0x000007D0
    AlphaTest = 2450, // 0x00000992
    GeometryLast = 2500, // 0x000009C4
    Transparent = 3000, // 0x00000BB8
    Overlay = 4000, // 0x00000FA0
  }
}
