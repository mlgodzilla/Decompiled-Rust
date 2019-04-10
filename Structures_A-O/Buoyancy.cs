// Decompiled with JetBrains decompiler
// Type: Buoyancy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using UnityEngine;

public class Buoyancy : BaseMonoBehaviour
{
  public float buoyancyScale = 1f;
  public bool doEffects = true;
  public float waveHeightScale = 0.5f;
  public BuoyancyPoint[] points;
  public GameObjectRef[] waterImpacts;
  public Rigidbody rigidBody;
  public float submergedFraction;
  public bool clientSide;
  public Action<bool> SubmergedChanged;
  private float timeOutOfWater;
  private float timeInWater;
  private Buoyancy.BuoyancyPointData[] pointData;
  private Vector2[] pointPositionArray;
  private Vector2[] pointPositionUVArray;
  private float[] pointTerrainHeightArray;
  private float[] pointWaterHeightArray;

  public static string DefaultWaterImpact()
  {
    return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
  }

  public void Sleep()
  {
    ((Behaviour) this).set_enabled(false);
    this.InvokeRandomized(new Action(this.CheckForWake), 0.5f, 0.5f, 0.1f);
  }

  public void Wake()
  {
    ((Behaviour) this).set_enabled(true);
    this.CancelInvoke(new Action(this.CheckForWake));
  }

  public void CheckForWake()
  {
    if (!((double) WaterSystem.GetHeight(((Component) this).get_transform().get_position()) > Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.2f)).y & !this.rigidBody.IsSleeping()))
      return;
    this.Wake();
  }

  public void FixedUpdate()
  {
    bool flag1 = (double) this.submergedFraction > 0.0;
    this.BuoyancyFixedUpdate();
    bool flag2 = (double) this.submergedFraction > 0.0;
    if (this.SubmergedChanged == null || flag1 == flag2)
      return;
    this.SubmergedChanged(flag2);
  }

  public Vector3 GetFlowDirection(Vector2 posUV)
  {
    if (Object.op_Equality((Object) TerrainMeta.WaterMap, (Object) null))
      return Vector3.get_zero();
    Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
    float num = Mathf.Clamp01(Mathf.Abs((float) normalFast.y));
    normalFast.y = (__Null) 0.0;
    Vector3Ex.FastRenormalize(normalFast, num);
    return normalFast;
  }

  public void EnsurePointsInitialized()
  {
    if (this.points == null || this.points.Length == 0)
    {
      Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        GameObject gameObject = new GameObject("BuoyancyPoint");
        gameObject.get_transform().set_parent(((Component) component).get_gameObject().get_transform());
        gameObject.get_transform().set_localPosition(component.get_centerOfMass());
        BuoyancyPoint buoyancyPoint = (BuoyancyPoint) gameObject.AddComponent<BuoyancyPoint>();
        buoyancyPoint.buoyancyForce = component.get_mass() * (float) -Physics.get_gravity().y;
        buoyancyPoint.buoyancyForce *= 1.32f;
        buoyancyPoint.size = 0.2f;
        this.points = new BuoyancyPoint[1];
        this.points[0] = buoyancyPoint;
      }
    }
    if (this.pointData != null && this.pointData.Length == this.points.Length)
      return;
    this.pointData = new Buoyancy.BuoyancyPointData[this.points.Length];
    this.pointPositionArray = new Vector2[this.points.Length];
    this.pointPositionUVArray = new Vector2[this.points.Length];
    this.pointTerrainHeightArray = new float[this.points.Length];
    this.pointWaterHeightArray = new float[this.points.Length];
    for (int index = 0; index < this.points.Length; ++index)
    {
      Transform transform = ((Component) this.points[index]).get_transform();
      Transform parent = transform.get_parent();
      transform.SetParent(((Component) this).get_transform());
      Vector3 localPosition = transform.get_localPosition();
      transform.SetParent(parent);
      this.pointData[index].transform = transform;
      this.pointData[index].localPosition = transform.get_localPosition();
      this.pointData[index].rootToPoint = localPosition;
    }
  }

  public void BuoyancyFixedUpdate()
  {
    if (Object.op_Equality((Object) TerrainMeta.WaterMap, (Object) null))
      return;
    this.EnsurePointsInitialized();
    if (Object.op_Equality((Object) this.rigidBody, (Object) null))
      return;
    if (this.rigidBody.IsSleeping())
      this.Sleep();
    else if ((double) this.buoyancyScale == 0.0)
    {
      this.Sleep();
    }
    else
    {
      float time = Time.get_time();
      float x1 = (float) TerrainMeta.Position.x;
      float z1 = (float) TerrainMeta.Position.z;
      float x2 = (float) TerrainMeta.OneOverSize.x;
      float z2 = (float) TerrainMeta.OneOverSize.z;
      Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
      for (int index = 0; index < this.pointData.Length; ++index)
      {
        BuoyancyPoint point = this.points[index];
        Vector3 vector3 = ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.pointData[index].rootToPoint);
        this.pointData[index].position = vector3;
        float num1 = ((float) vector3.x - x1) * x2;
        float num2 = ((float) vector3.z - z1) * z2;
        this.pointPositionArray[index] = new Vector2((float) vector3.x, (float) vector3.z);
        this.pointPositionUVArray[index] = new Vector2(num1, num2);
      }
      WaterSystem.GetHeight(this.pointPositionArray, this.pointPositionUVArray, this.pointTerrainHeightArray, this.pointWaterHeightArray);
      int num3 = 0;
      for (int index = 0; index < this.points.Length; ++index)
      {
        BuoyancyPoint point = this.points[index];
        Vector3 position = this.pointData[index].position;
        Vector3 localPosition = this.pointData[index].localPosition;
        Vector2 pointPositionUv = this.pointPositionUVArray[index];
        float pointTerrainHeight = this.pointTerrainHeightArray[index];
        float pointWaterHeight = this.pointWaterHeightArray[index];
        WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, pointPositionUv, pointTerrainHeight, pointWaterHeight);
        bool flag = false;
        if (position.y < (double) buoyancyWaterInfo.surfaceLevel)
        {
          flag = true;
          ++num3;
          float currentDepth = buoyancyWaterInfo.currentDepth;
          float num1 = Mathf.InverseLerp(0.0f, point.size, currentDepth);
          float num2 = (float) (1.0 + (double) Mathf.PerlinNoise(point.randomOffset + time * point.waveFrequency, 0.0f) * (double) point.waveScale);
          float num4 = point.buoyancyForce * this.buoyancyScale;
          Vector3 vector3;
          ((Vector3) ref vector3).\u002Ector(0.0f, num2 * num1 * num4, 0.0f);
          Vector3 flowDirection = this.GetFlowDirection(pointPositionUv);
          if (flowDirection.y < 0.999899983406067 && Vector3.op_Inequality(flowDirection, Vector3.get_up()))
          {
            float num5 = num4 * 0.25f;
            ref __Null local1 = ref vector3.x;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local1 = ^(float&) ref local1 + (float) flowDirection.x * num5;
            ref __Null local2 = ref vector3.y;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local2 = ^(float&) ref local2 + (float) flowDirection.y * num5;
            ref __Null local3 = ref vector3.z;
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            // ISSUE: explicit reference operation
            ^(float&) ref local3 = ^(float&) ref local3 + (float) flowDirection.z * num5;
          }
          this.rigidBody.AddForceAtPosition(vector3, position, (ForceMode) 0);
        }
        if (point.doSplashEffects && (!point.wasSubmergedLastFrame & flag || !flag && point.wasSubmergedLastFrame) && this.doEffects)
        {
          Vector3 relativePointVelocity = this.rigidBody.GetRelativePointVelocity(localPosition);
          if ((double) ((Vector3) ref relativePointVelocity).get_magnitude() > 1.0)
          {
            string strName = this.waterImpacts == null || this.waterImpacts.Length == 0 || !this.waterImpacts[0].isValid ? Buoyancy.DefaultWaterImpact() : this.waterImpacts[0].resourcePath;
            Vector3 vector3;
            ((Vector3) ref vector3).\u002Ector(Random.Range(-0.25f, 0.25f), 0.0f, Random.Range(-0.25f, 0.25f));
            if (this.clientSide)
              Effect.client.Run(strName, Vector3.op_Addition(position, vector3), Vector3.get_up(), (Vector3) null);
            else
              Effect.server.Run(strName, Vector3.op_Addition(position, vector3), Vector3.get_up(), (Connection) null, false);
            point.nexSplashTime = Time.get_time() + 0.25f;
          }
        }
        point.wasSubmergedLastFrame = flag;
      }
      if (this.points.Length != 0)
        this.submergedFraction = (float) num3 / (float) this.points.Length;
      if ((double) this.submergedFraction > 0.0)
      {
        this.timeInWater += Time.get_fixedDeltaTime();
        this.timeOutOfWater = 0.0f;
      }
      else
      {
        this.timeOutOfWater += Time.get_fixedDeltaTime();
        this.timeInWater = 0.0f;
      }
      if ((double) this.timeOutOfWater <= 3.0 || !((Behaviour) this).get_enabled())
        return;
      this.Sleep();
    }
  }

  private struct BuoyancyPointData
  {
    public Transform transform;
    public Vector3 localPosition;
    public Vector3 rootToPoint;
    public Vector3 position;
  }
}
