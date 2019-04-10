// Decompiled with JetBrains decompiler
// Type: LeavesBlowing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LeavesBlowing : MonoBehaviour
{
  public ParticleSystem m_psLeaves;
  public float m_flSwirl;
  public float m_flSpeed;
  public float m_flEmissionRate;

  private void Start()
  {
  }

  private void Update()
  {
    ((Component) this).get_transform().RotateAround(((Component) this).get_transform().get_position(), Vector3.get_up(), Time.get_deltaTime() * this.m_flSwirl);
    if (!Object.op_Inequality((Object) this.m_psLeaves, (Object) null))
      return;
    this.m_psLeaves.set_startSpeed(this.m_flSpeed);
    ParticleSystem psLeaves = this.m_psLeaves;
    psLeaves.set_startSpeed(psLeaves.get_startSpeed() + Mathf.Sin(Time.get_time() * 0.4f) * (this.m_flSpeed * 0.75f));
    this.m_psLeaves.set_emissionRate(this.m_flEmissionRate + Mathf.Sin(Time.get_time() * 1f) * (this.m_flEmissionRate * 0.3f));
  }

  public LeavesBlowing()
  {
    base.\u002Ector();
  }
}
