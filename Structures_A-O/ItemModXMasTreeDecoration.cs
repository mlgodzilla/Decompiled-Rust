// Decompiled with JetBrains decompiler
// Type: ItemModXMasTreeDecoration
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ItemModXMasTreeDecoration : ItemMod
{
  public ItemModXMasTreeDecoration.xmasFlags flagsToChange;

  public enum xmasFlags
  {
    pineCones = 128, // 0x00000080
    candyCanes = 256, // 0x00000100
    gingerbreadMen = 512, // 0x00000200
    Tinsel = 1024, // 0x00000400
    Balls = 2048, // 0x00000800
    Star = 16384, // 0x00004000
    Lights = 32768, // 0x00008000
  }
}
