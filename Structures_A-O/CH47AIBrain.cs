// Decompiled with JetBrains decompiler
// Type: CH47AIBrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CH47AIBrain : BaseAIBrain<CH47HelicopterAIController>
{
  public const int CH47State_Idle = 1;
  public const int CH47State_Patrol = 2;
  public const int CH47State_Land = 3;
  public const int CH47State_Dropoff = 4;
  public const int CH47State_Orbit = 5;
  public const int CH47State_Retreat = 6;
  public const int CH47State_Egress = 7;
  private float age;

  public override void InitializeAI()
  {
    base.InitializeAI();
    this.AIStates = new BaseAIBrain<CH47HelicopterAIController>.BasicAIState[8];
    this.AddState((BaseAIBrain<CH47HelicopterAIController>.BasicAIState) new CH47AIBrain.IdleState(), 1);
    this.AddState((BaseAIBrain<CH47HelicopterAIController>.BasicAIState) new CH47AIBrain.PatrolState(), 2);
    this.AddState((BaseAIBrain<CH47HelicopterAIController>.BasicAIState) new CH47AIBrain.OrbitState(), 5);
    this.AddState((BaseAIBrain<CH47HelicopterAIController>.BasicAIState) new CH47AIBrain.EgressState(), 7);
    this.AddState((BaseAIBrain<CH47HelicopterAIController>.BasicAIState) new CH47AIBrain.DropCrate(), 4);
    this.AddState((BaseAIBrain<CH47HelicopterAIController>.BasicAIState) new CH47AIBrain.LandState(), 3);
  }

  public void FixedUpdate()
  {
    if (Object.op_Equality((Object) this.baseEntity, (Object) null) || this.baseEntity.isClient)
      return;
    this.AIThink(Time.get_fixedDeltaTime());
  }

  public void OnDrawGizmos()
  {
    this.GetCurrentState()?.DrawGizmos();
  }

  public override void AIThink(float delta)
  {
    this.age += delta;
    base.AIThink(delta);
  }

  public class IdleState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
  {
    public override float GetWeight()
    {
      return 0.1f;
    }

    public override void StateEnter()
    {
      CH47HelicopterAIController entity = this.brain.GetEntity();
      Vector3 position1 = this.brain.GetEntity().GetPosition();
      Vector3 velocity = this.brain.GetEntity().rigidBody.get_velocity();
      Vector3 vector3 = Vector3.op_Multiply(((Vector3) ref velocity).get_normalized(), 10f);
      Vector3 position2 = Vector3.op_Addition(position1, vector3);
      entity.SetMoveTarget(position2);
      base.StateEnter();
    }
  }

  public class PatrolState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
  {
    public static float patrolApproachDist = 75f;
    public List<Vector3> visitedPoints = new List<Vector3>();
    public bool testing = true;

    public bool AtPatrolDestination()
    {
      return (double) Vector3Ex.Distance2D(this.brain.mainInterestPoint, this.brain.GetEntity().GetPosition()) < (double) CH47AIBrain.PatrolState.patrolApproachDist;
    }

    public override bool CanInterrupt()
    {
      if (base.CanInterrupt())
        return this.AtPatrolDestination();
      return false;
    }

    public override float GetWeight()
    {
      if (!this.IsInState())
        return 1f + Mathf.InverseLerp(30f, 120f, this.TimeSinceState()) * 5f;
      return this.AtPatrolDestination() && (double) this.TimeInState() > 2.0 ? 0.0f : 3f;
    }

    public MonumentInfo GetRandomValidMonumentInfo()
    {
      int count = TerrainMeta.Path.Monuments.Count;
      int num = Random.Range(0, count);
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = index1 + num;
        if (index2 >= count)
          index2 -= count;
        MonumentInfo monument = TerrainMeta.Path.Monuments[index2];
        if (monument.Type != MonumentType.Cave && monument.Type != MonumentType.WaterWell && monument.Tier != MonumentTier.Tier0)
          return monument;
      }
      return (MonumentInfo) null;
    }

    public Vector3 GetRandomPatrolPoint()
    {
      Vector3.get_zero();
      MonumentInfo monumentInfo = (MonumentInfo) null;
      if (Object.op_Inequality((Object) TerrainMeta.Path, (Object) null) && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
      {
        int count = TerrainMeta.Path.Monuments.Count;
        int num = Random.Range(0, count);
        for (int index1 = 0; index1 < count; ++index1)
        {
          int index2 = index1 + num;
          if (index2 >= count)
            index2 -= count;
          MonumentInfo monument = TerrainMeta.Path.Monuments[index2];
          if (monument.Type != MonumentType.Cave && monument.Type != MonumentType.WaterWell && (monument.Tier != MonumentTier.Tier0 && (monument.Tier & MonumentTier.Tier0) <= (MonumentTier) 0))
          {
            bool flag = false;
            using (List<Vector3>.Enumerator enumerator = this.visitedPoints.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                Vector3 current = enumerator.Current;
                if ((double) Vector3Ex.Distance2D(((Component) monument).get_transform().get_position(), current) < 100.0)
                {
                  flag = true;
                  break;
                }
              }
            }
            if (!flag)
            {
              monumentInfo = monument;
              break;
            }
          }
        }
        if (Object.op_Equality((Object) monumentInfo, (Object) null))
        {
          this.visitedPoints.Clear();
          monumentInfo = this.GetRandomValidMonumentInfo();
        }
      }
      Vector3 vector3_1;
      if (Object.op_Inequality((Object) monumentInfo, (Object) null))
      {
        this.visitedPoints.Add(((Component) monumentInfo).get_transform().get_position());
        vector3_1 = ((Component) monumentInfo).get_transform().get_position();
      }
      else
      {
        float x = (float) TerrainMeta.Size.x;
        float num = 30f;
        Vector3 vector3_2 = Vector3Ex.Range(-1f, 1f);
        vector3_2.y = (__Null) 0.0;
        ((Vector3) ref vector3_2).Normalize();
        vector3_1 = Vector3.op_Multiply(vector3_2, x * Random.Range(0.0f, 0.75f));
        vector3_1.y = (__Null) (double) num;
      }
      return vector3_1;
    }

    public override void StateEnter()
    {
      base.StateEnter();
      Vector3 randomPatrolPoint = this.GetRandomPatrolPoint();
      this.brain.mainInterestPoint = randomPatrolPoint;
      float num1 = Mathf.Max(TerrainMeta.WaterMap.GetHeight(randomPatrolPoint), TerrainMeta.HeightMap.GetHeight(randomPatrolPoint));
      float num2 = num1;
      RaycastHit raycastHit;
      if (Physics.SphereCast(Vector3.op_Addition(randomPatrolPoint, new Vector3(0.0f, 200f, 0.0f)), 20f, Vector3.get_down(), ref raycastHit, 300f, 1218511105))
        num2 = Mathf.Max((float) ((RaycastHit) ref raycastHit).get_point().y, num1);
      this.brain.mainInterestPoint.y = (__Null) ((double) num2 + 30.0);
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
    }
  }

  public class LandState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
  {
    private float landingHeight = 20f;
    private float landedForSeconds;
    private float lastLandtime;
    private float nextDismountTime;

    public override float GetWeight()
    {
      if (!this.GetEntity().ShouldLand())
        return 0.0f;
      float num = Time.get_time() - this.lastLandtime;
      if (this.IsInState() && (double) this.landedForSeconds < 12.0)
        return 1000f;
      return !this.IsInState() && (double) num > 10.0 ? 9000f : 0.0f;
    }

    public override void StateThink(float delta)
    {
      Vector3 position = ((Component) this.brain.GetEntity()).get_transform().get_position();
      CH47LandingZone closest = CH47LandingZone.GetClosest(this.brain.GetEntity().landingTarget);
      if (!Object.op_Implicit((Object) closest))
        return;
      Vector3 velocity = this.brain.GetEntity().rigidBody.get_velocity();
      float magnitude = ((Vector3) ref velocity).get_magnitude();
      double num1 = (double) Vector3.Distance(((Component) closest).get_transform().get_position(), position);
      float num2 = Vector3Ex.Distance2D(((Component) closest).get_transform().get_position(), position);
      double num3 = (double) Mathf.InverseLerp(1f, 20f, num2);
      bool enabled = (double) num2 < 100.0;
      bool on = (double) num2 > 15.0 && position.y < ((Component) closest).get_transform().get_position().y + 10.0;
      this.brain.GetEntity().EnableFacingOverride(enabled);
      this.brain.GetEntity().SetAltitudeProtection(on);
      int num4 = (double) Mathf.Abs((float) (((Component) closest).get_transform().get_position().y - position.y)) >= 3.0 || (double) num2 > 5.0 ? 0 : ((double) magnitude < 1.0 ? 1 : 0);
      if (num4 != 0)
      {
        this.landedForSeconds += delta;
        if ((double) this.lastLandtime == 0.0)
          this.lastLandtime = Time.get_time();
      }
      this.landingHeight -= 4f * (1f - Mathf.InverseLerp(0.0f, 7f, num2)) * Time.get_deltaTime();
      if ((double) this.landingHeight < -5.0)
        this.landingHeight = -5f;
      this.brain.GetEntity().SetAimDirection(((Component) closest).get_transform().get_forward());
      this.brain.GetEntity().SetMoveTarget(Vector3.op_Addition(this.brain.mainInterestPoint, new Vector3(0.0f, this.landingHeight, 0.0f)));
      if (num4 == 0)
        return;
      if ((double) this.landedForSeconds > 1.0 && (double) Time.get_time() > (double) this.nextDismountTime)
      {
        foreach (BaseVehicle.MountPointInfo mountPoint in this.brain.GetEntity().mountPoints)
        {
          if (Object.op_Implicit((Object) mountPoint.mountable) && mountPoint.mountable.IsMounted())
          {
            this.nextDismountTime = Time.get_time() + 0.5f;
            mountPoint.mountable.DismountAllPlayers();
            break;
          }
        }
      }
      if ((double) this.landedForSeconds <= 8.0)
        return;
      ((CH47AIBrain) ((Component) this.brain).GetComponent<CH47AIBrain>()).age = float.PositiveInfinity;
    }

    public override void StateEnter()
    {
      this.brain.mainInterestPoint = this.GetEntity().landingTarget;
      this.landingHeight = 15f;
      base.StateEnter();
    }

    public override void StateLeave()
    {
      this.brain.GetEntity().EnableFacingOverride(false);
      this.brain.GetEntity().SetAltitudeProtection(true);
      this.brain.GetEntity().SetMinHoverHeight(30f);
      this.landedForSeconds = 0.0f;
      base.StateLeave();
    }

    public override bool CanInterrupt()
    {
      return true;
    }
  }

  public class OrbitState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
  {
    public Vector3 GetOrbitCenter()
    {
      return this.brain.mainInterestPoint;
    }

    public override bool CanInterrupt()
    {
      return base.CanInterrupt();
    }

    public override float GetWeight()
    {
      if (this.IsInState())
        return 5f * (1f - Mathf.InverseLerp(120f, 180f, this.TimeInState()));
      return this.brain._currentState == 2 && (double) Vector3Ex.Distance2D(this.brain.mainInterestPoint, this.brain.GetEntity().GetPosition()) <= (double) CH47AIBrain.PatrolState.patrolApproachDist * 1.10000002384186 ? 5f : 0.0f;
    }

    public override void StateEnter()
    {
      this.brain.GetEntity().EnableFacingOverride(true);
      this.brain.GetEntity().InitiateAnger();
      base.StateEnter();
    }

    public override void StateThink(float delta)
    {
      Vector3 orbitCenter = this.GetOrbitCenter();
      CH47HelicopterAIController entity = this.brain.GetEntity();
      Vector3 position1 = entity.GetPosition();
      Vector3 vector3_1 = Vector3Ex.Direction2D(orbitCenter, position1);
      Vector3 vector3_2 = Vector3.Cross(Vector3.get_up(), vector3_1);
      float num1 = (double) Vector3.Dot(Vector3.Cross(((Component) entity).get_transform().get_right(), Vector3.get_up()), vector3_2) < 0.0 ? -1f : 1f;
      float num2 = 75f;
      Vector3 vector3_3 = Vector3.op_Addition(Vector3.op_UnaryNegation(vector3_1), Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, num1), 0.6f));
      Vector3 position2 = Vector3.op_Addition(orbitCenter, Vector3.op_Multiply(((Vector3) ref vector3_3).get_normalized(), num2));
      entity.SetMoveTarget(position2);
      entity.SetAimDirection(Vector3Ex.Direction2D(position2, position1));
      base.StateThink(delta);
    }

    public override void StateLeave()
    {
      this.brain.GetEntity().EnableFacingOverride(false);
      this.brain.GetEntity().CancelAnger();
      base.StateLeave();
    }
  }

  public class EgressState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
  {
    private bool killing;

    public override bool CanInterrupt()
    {
      return false;
    }

    public override float GetWeight()
    {
      if (this.brain.GetEntity().OutOfCrates() && !this.brain.GetEntity().ShouldLand())
        return 10000f;
      CH47AIBrain component = (CH47AIBrain) ((Component) this.brain).GetComponent<CH47AIBrain>();
      return Object.op_Inequality((Object) component, (Object) null) && (double) component.age > 600.0 ? 10000f : 0.0f;
    }

    public override void StateEnter()
    {
      this.brain.GetEntity().EnableFacingOverride(false);
      Transform transform = ((Component) this.brain.GetEntity()).get_transform();
      Rigidbody rigidBody = this.brain.GetEntity().rigidBody;
      Vector3 velocity1 = rigidBody.get_velocity();
      Vector3 vector3_1;
      if ((double) ((Vector3) ref velocity1).get_magnitude() >= 0.100000001490116)
      {
        Vector3 velocity2 = rigidBody.get_velocity();
        vector3_1 = ((Vector3) ref velocity2).get_normalized();
      }
      else
        vector3_1 = transform.get_forward();
      Vector3 vector3_2 = vector3_1;
      Vector3 vector3_3 = Vector3.Cross(Vector3.Cross(transform.get_up(), vector3_2), Vector3.get_up());
      this.brain.mainInterestPoint = Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(vector3_3, 8000f));
      this.brain.mainInterestPoint.y = (__Null) 90.0;
      this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
      base.StateEnter();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if (this.killing)
        return;
      this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
      if ((double) this.TimeInState() <= 300.0)
        return;
      ((MonoBehaviour) this.brain.GetEntity()).Invoke("DelayedKill", 2f);
      this.killing = true;
    }

    public override void StateLeave()
    {
      base.StateLeave();
    }
  }

  public class DropCrate : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
  {
    private float nextDropTime;

    public override bool CanInterrupt()
    {
      if (base.CanInterrupt())
        return !this.CanDrop();
      return false;
    }

    public bool CanDrop()
    {
      if ((double) Time.get_time() > (double) this.nextDropTime)
        return this.brain.GetEntity().CanDropCrate();
      return false;
    }

    public override float GetWeight()
    {
      if (!this.CanDrop())
        return 0.0f;
      CH47DropZone closest = CH47DropZone.GetClosest(((Component) this.brain.GetEntity()).get_transform().get_position());
      if (Object.op_Equality((Object) closest, (Object) null) || (double) Vector3.Distance(((Component) closest).get_transform().get_position(), this.brain.mainInterestPoint) > 200.0)
        return 0.0f;
      if (this.IsInState())
        return 10000f;
      return this.brain._currentState == 5 && (double) this.brain.GetCurrentState().TimeInState() > 60.0 ? 1000f : 0.0f;
    }

    public override void StateEnter()
    {
      this.brain.GetEntity().SetDropDoorOpen(true);
      this.brain.GetEntity().EnableFacingOverride(false);
      CH47DropZone closest = CH47DropZone.GetClosest(((Component) this.brain.GetEntity()).get_transform().get_position());
      if (Object.op_Equality((Object) closest, (Object) null))
        this.nextDropTime = Time.get_time() + 60f;
      this.brain.mainInterestPoint = ((Component) closest).get_transform().get_position();
      this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
      base.StateEnter();
    }

    public override void StateThink(float delta)
    {
      base.StateThink(delta);
      if (!this.CanDrop() || (double) Vector3Ex.Distance2D(this.brain.mainInterestPoint, ((Component) this.brain.GetEntity()).get_transform().get_position()) >= 5.0)
        return;
      Vector3 velocity = this.brain.GetEntity().rigidBody.get_velocity();
      if ((double) ((Vector3) ref velocity).get_magnitude() >= 5.0)
        return;
      this.brain.GetEntity().DropCrate();
      this.nextDropTime = Time.get_time() + 120f;
    }

    public override void StateLeave()
    {
      this.brain.GetEntity().SetDropDoorOpen(false);
      this.nextDropTime = Time.get_time() + 60f;
      base.StateLeave();
    }
  }
}
