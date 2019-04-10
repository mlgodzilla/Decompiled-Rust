// Decompiled with JetBrains decompiler
// Type: Sandstorm
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Sandstorm : MonoBehaviour
{
  public ParticleSystem m_psSandStorm;
  public float m_flSpeed;
  public float m_flSwirl;
  public float m_flEmissionRate;

  private void Start()
  {
  }

  private void Update()
  {
    ((Component) this).get_transform().RotateAround(((Component) this).get_transform().get_position(), Vector3.get_up(), Time.get_deltaTime() * this.m_flSwirl);
    Vector3 eulerAngles = ((Component) this).get_transform().get_eulerAngles();
    eulerAngles.x = (__Null) ((double) Mathf.Sin(Time.get_time() * 2.5f) * 7.0 - 7.0);
    ((Component) this).get_transform().set_eulerAngles(eulerAngles);
    if (!Object.op_Inequality((Object) this.m_psSandStorm, (Object) null))
      return;
    this.m_psSandStorm.set_startSpeed(this.m_flSpeed);
    ParticleSystem psSandStorm = this.m_psSandStorm;
    psSandStorm.set_startSpeed(psSandStorm.get_startSpeed() + Mathf.Sin(Time.get_time() * 0.4f) * (this.m_flSpeed * 0.75f));
    this.m_psSandStorm.set_emissionRate(this.m_flEmissionRate + Mathf.Sin(Time.get_time() * 1f) * (this.m_flEmissionRate * 0.3f));
  }

  public Sandstorm()
  {
    base.\u002Ector();
  }
}
