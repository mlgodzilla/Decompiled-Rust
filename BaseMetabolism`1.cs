// Decompiled with JetBrains decompiler
// Type: BaseMetabolism`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
  public MetabolismAttribute calories = new MetabolismAttribute();
  public MetabolismAttribute hydration = new MetabolismAttribute();
  public MetabolismAttribute heartrate = new MetabolismAttribute();
  protected T owner;
  protected float timeSinceLastMetabolism;

  public virtual void Reset()
  {
    this.calories.Reset();
    this.hydration.Reset();
    this.heartrate.Reset();
  }

  protected virtual void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.owner = default (T);
  }

  public virtual void ServerInit(T owner)
  {
    this.Reset();
    this.owner = owner;
  }

  public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
  {
    this.timeSinceLastMetabolism += delta;
    if ((double) this.timeSinceLastMetabolism <= (double) ConVar.Server.metabolismtick)
      return;
    if (Object.op_Implicit((Object) (object) this.owner) && !this.owner.IsDead())
    {
      this.RunMetabolism(ownerEntity, this.timeSinceLastMetabolism);
      this.DoMetabolismDamage(ownerEntity, this.timeSinceLastMetabolism);
    }
    this.timeSinceLastMetabolism = 0.0f;
  }

  protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
  {
    if ((double) this.calories.value <= 20.0)
    {
      using (TimeWarning.New("Calories Hurt", 0.1f))
        ownerEntity.Hurt((float) ((double) Mathf.InverseLerp(20f, 0.0f, this.calories.value) * (double) delta * 0.0833333358168602), DamageType.Hunger, (BaseEntity) null, true);
    }
    if ((double) this.hydration.value > 20.0)
      return;
    using (TimeWarning.New("Hyration Hurt", 0.1f))
      ownerEntity.Hurt((float) ((double) Mathf.InverseLerp(20f, 0.0f, this.hydration.value) * (double) delta * 0.133333340287209), DamageType.Thirst, (BaseEntity) null, true);
  }

  protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
  {
    if ((double) this.calories.value > 200.0)
      ownerEntity.Heal((float) ((double) Mathf.InverseLerp(200f, 1000f, this.calories.value) * (double) delta * 0.0166666675359011));
    if ((double) this.hydration.value > 200.0)
      ownerEntity.Heal((float) ((double) Mathf.InverseLerp(200f, 1000f, this.hydration.value) * (double) delta * 0.0166666675359011));
    this.hydration.MoveTowards(0.0f, delta * 0.008333334f);
    this.calories.MoveTowards(0.0f, delta * 0.01666667f);
    this.heartrate.MoveTowards(0.05f, delta * 0.01666667f);
  }

  public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
  {
    this.FindAttribute(type)?.Add(amount);
  }

  public bool ShouldDie()
  {
    if (Object.op_Implicit((Object) (object) this.owner))
      return (double) this.owner.Health() <= 0.0;
    return false;
  }

  public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
  {
    switch (type)
    {
      case MetabolismAttribute.Type.Calories:
        return this.calories;
      case MetabolismAttribute.Type.Hydration:
        return this.hydration;
      case MetabolismAttribute.Type.Heartrate:
        return this.heartrate;
      default:
        return (MetabolismAttribute) null;
    }
  }
}
