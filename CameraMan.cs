// Decompiled with JetBrains decompiler
// Type: CameraMan
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class CameraMan : SingletonComponent<CameraMan>
{
  public bool OnlyControlWhenCursorHidden;
  public bool NeedBothMouseButtonsToZoom;
  public float LookSensitivity;
  public float MoveSpeed;

  public CameraMan()
  {
    base.\u002Ector();
  }
}
