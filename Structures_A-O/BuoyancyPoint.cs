// Decompiled with JetBrains decompiler
// Type: BuoyancyPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BuoyancyPoint : MonoBehaviour
{
  public float buoyancyForce;
  public float size;
  public float randomOffset;
  public float waveScale;
  public float waveFrequency;
  public bool wasSubmergedLastFrame;
  public float nexSplashTime;
  public bool doSplashEffects;

  public void Start()
  {
    this.randomOffset = Random.Range(0.0f, 20f);
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), this.size * 0.5f);
  }

  public BuoyancyPoint()
  {
    base.\u002Ector();
  }
}
