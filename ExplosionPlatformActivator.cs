// Decompiled with JetBrains decompiler
// Type: ExplosionPlatformActivator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionPlatformActivator : MonoBehaviour
{
  public GameObject Effect;
  public float TimeDelay;
  public float DefaultRepeatTime;
  public float NearRepeatTime;
  private float currentTime;
  private float currentRepeatTime;
  private bool canUpdate;

  private void Start()
  {
    this.currentRepeatTime = this.DefaultRepeatTime;
    this.Invoke("Init", this.TimeDelay);
  }

  private void Init()
  {
    this.canUpdate = true;
    this.Effect.SetActive(true);
  }

  private void Update()
  {
    if (!this.canUpdate || Object.op_Equality((Object) this.Effect, (Object) null))
      return;
    this.currentTime += Time.get_deltaTime();
    if ((double) this.currentTime <= (double) this.currentRepeatTime)
      return;
    this.currentTime = 0.0f;
    this.Effect.SetActive(false);
    this.Effect.SetActive(true);
  }

  private void OnTriggerEnter(Collider coll)
  {
    this.currentRepeatTime = this.NearRepeatTime;
  }

  private void OnTriggerExit(Collider other)
  {
    this.currentRepeatTime = this.DefaultRepeatTime;
  }

  public ExplosionPlatformActivator()
  {
    base.\u002Ector();
  }
}
