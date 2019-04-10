// Decompiled with JetBrains decompiler
// Type: rottest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class rottest : MonoBehaviour
{
  public Transform turretBase;
  public Vector3 aimDir;

  private void Start()
  {
  }

  private void Update()
  {
    this.aimDir = new Vector3(0.0f, 45f * Mathf.Sin(Time.get_time() * 6f), 0.0f);
    this.UpdateAiming();
  }

  public void UpdateAiming()
  {
    if (Vector3.op_Equality(this.aimDir, Vector3.get_zero()))
      return;
    Quaternion quaternion = Quaternion.Euler(0.0f, (float) this.aimDir.y, 0.0f);
    if (!Quaternion.op_Inequality(((Component) this).get_transform().get_localRotation(), quaternion))
      return;
    ((Component) this).get_transform().set_localRotation(Quaternion.Lerp(((Component) this).get_transform().get_localRotation(), quaternion, Time.get_deltaTime() * 8f));
  }

  public rottest()
  {
    base.\u002Ector();
  }
}
