// Decompiled with JetBrains decompiler
// Type: EyeBlink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EyeBlink : MonoBehaviour
{
  public Transform LeftEye;
  public Vector3 LeftEyeOffset;
  public Transform RightEye;
  public Vector3 RightEyeOffset;
  public Vector2 TimeWithoutBlinking;
  public float BlinkSpeed;

  public EyeBlink()
  {
    base.\u002Ector();
  }
}
