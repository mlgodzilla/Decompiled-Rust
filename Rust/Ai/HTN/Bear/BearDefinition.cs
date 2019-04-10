// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.Bear.BearDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System.Collections;
using UnityEngine;

namespace Rust.Ai.HTN.Bear
{
  [CreateAssetMenu(menuName = "Rust/AI/Animals/Bear Definition")]
  public class BearDefinition : BaseNpcDefinition
  {
    [Header("Sensory Extensions")]
    public float StandingAggroRange = 40f;
    [Header("Audio")]
    public Vector2 IdleEffectRepeatRange = new Vector2(10f, 15f);
    [Header("Corpse")]
    public GameObjectRef CorpsePrefab;
    [Header("Equipment")]
    public LootContainer.LootSpawnSlot[] Loot;
    public GameObjectRef IdleEffect;
    public GameObjectRef DeathEffect;
    private bool _isEffectRunning;

    public float SqrStandingAggroRange
    {
      get
      {
        return this.StandingAggroRange * this.StandingAggroRange;
      }
    }

    public float AggroRange(bool isStanding)
    {
      if (!isStanding)
        return this.Engagement.AggroRange;
      return this.StandingAggroRange;
    }

    public float SqrAggroRange(bool isStanding)
    {
      if (!isStanding)
        return this.Engagement.SqrAggroRange;
      return this.SqrStandingAggroRange;
    }

    public override void StartVoices(HTNAnimal target)
    {
      if (this._isEffectRunning)
        return;
      this._isEffectRunning = true;
      ((MonoBehaviour) target).StartCoroutine(this.PlayEffects(target));
    }

    public override void StopVoices(HTNAnimal target)
    {
      if (!this._isEffectRunning)
        return;
      this._isEffectRunning = false;
    }

    private IEnumerator PlayEffects(HTNAnimal target)
    {
      while (this._isEffectRunning && Object.op_Inequality((Object) target, (Object) null) && (Object.op_Inequality((Object) ((Component) target).get_transform(), (Object) null) && !target.IsDestroyed) && !target.IsDead())
      {
        if (this.IdleEffect.isValid)
          Effect.server.Run(this.IdleEffect.resourcePath, (BaseEntity) target, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
        yield return (object) CoroutineEx.waitForSeconds(Random.Range((float) this.IdleEffectRepeatRange.x, (float) (this.IdleEffectRepeatRange.y + 1.0)));
      }
    }

    public override BaseCorpse OnCreateCorpse(HTNAnimal target)
    {
      if (this.DeathEffect.isValid)
        Effect.server.Run(this.DeathEffect.resourcePath, (BaseEntity) target, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      using (TimeWarning.New("Create corpse", 0.1f))
      {
        BaseCorpse baseCorpse = target.DropCorpse(this.CorpsePrefab.resourcePath);
        if (Object.op_Implicit((Object) baseCorpse))
        {
          if (Object.op_Inequality((Object) target.AiDomain, (Object) null) && Object.op_Inequality((Object) target.AiDomain.NavAgent, (Object) null) && target.AiDomain.NavAgent.get_isOnNavMesh())
            ((Component) baseCorpse).get_transform().set_position(Vector3.op_Addition(((Component) baseCorpse).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_down(), target.AiDomain.NavAgent.get_baseOffset())));
          baseCorpse.InitCorpse((BaseEntity) target);
          baseCorpse.Spawn();
          baseCorpse.TakeChildren((BaseEntity) target);
        }
        return baseCorpse;
      }
    }
  }
}
