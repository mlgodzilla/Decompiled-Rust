// Decompiled with JetBrains decompiler
// Type: ConVar.Physics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("physics")]
  public class Physics : ConsoleSystem
  {
    [ServerVar(Help = "The collision detection mode that dropped items and corpses should use")]
    public static int droppedmode = 2;
    [ServerVar(Help = "Send effects to clients when physics objects collide")]
    public static bool sendeffects = true;
    private const float baseGravity = -9.81f;

    [ServerVar]
    public static float bouncethreshold
    {
      get
      {
        return Physics.get_bounceThreshold();
      }
      set
      {
        Physics.set_bounceThreshold(value);
      }
    }

    [ServerVar]
    public static float sleepthreshold
    {
      get
      {
        return Physics.get_sleepThreshold();
      }
      set
      {
        Physics.set_sleepThreshold(value);
      }
    }

    [ServerVar(Help = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive")]
    public static int solveriterationcount
    {
      get
      {
        return Physics.get_defaultSolverIterations();
      }
      set
      {
        Physics.set_defaultSolverIterations(value);
      }
    }

    [ServerVar(Help = "Gravity multiplier")]
    public static float gravity
    {
      get
      {
        return (float) (Physics.get_gravity().y / -9.8100004196167);
      }
      set
      {
        Physics.set_gravity(new Vector3(0.0f, value * -9.81f, 0.0f));
      }
    }

    internal static void ApplyDropped(Rigidbody rigidBody)
    {
      if (Physics.droppedmode <= 0)
        rigidBody.set_collisionDetectionMode((CollisionDetectionMode) 0);
      if (Physics.droppedmode == 1)
        rigidBody.set_collisionDetectionMode((CollisionDetectionMode) 1);
      if (Physics.droppedmode == 2)
        rigidBody.set_collisionDetectionMode((CollisionDetectionMode) 2);
      if (Physics.droppedmode < 3)
        return;
      rigidBody.set_collisionDetectionMode((CollisionDetectionMode) 3);
    }

    [ClientVar]
    [ServerVar(Help = "The amount of physics steps per second")]
    public static float steps
    {
      get
      {
        return 1f / Time.get_fixedDeltaTime();
      }
      set
      {
        if ((double) value < 10.0)
          value = 10f;
        if ((double) value > 60.0)
          value = 60f;
        Time.set_fixedDeltaTime(1f / value);
      }
    }

    [ServerVar(Help = "The slowest physics steps will operate")]
    [ClientVar]
    public static float minsteps
    {
      get
      {
        return 1f / Time.get_maximumDeltaTime();
      }
      set
      {
        if ((double) value < 1.0)
          value = 1f;
        if ((double) value > 60.0)
          value = 60f;
        Time.set_maximumDeltaTime(1f / value);
      }
    }

    public Physics()
    {
      base.\u002Ector();
    }
  }
}
