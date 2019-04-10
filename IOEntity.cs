// Decompiled with JetBrains decompiler
// Type: IOEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class IOEntity : BaseCombatEntity
{
  [ServerVar]
  [Help("How many miliseconds to budget for processing io entities per server frame")]
  public static float framebudgetms = 1f;
  [ServerVar]
  public static float responsetime = 0.1f;
  [ServerVar]
  public static int backtracking = 8;
  public static Queue<IOEntity> _processQueue = new Queue<IOEntity>();
  [Header("IOEntity")]
  public Transform debugOrigin;
  public ItemDefinition sourceItem;
  [NonSerialized]
  public int lastResetIndex;
  public const BaseEntity.Flags Flag_ShortCircuit = BaseEntity.Flags.Reserved7;
  public const BaseEntity.Flags Flag_HasPower = BaseEntity.Flags.Reserved8;
  public IOEntity.IOSlot[] inputs;
  public IOEntity.IOSlot[] outputs;
  public IOEntity.IOType ioType;
  private int cachedOutputsUsed;
  private int lastPassthroughEnergy;
  private int lastEnergy;
  protected int currentEnergy;
  private float lastUpdateTime;
  protected bool ensureOutputsUpdated;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("IOEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 4161541566U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Server_RequestData "));
          using (TimeWarning.New("Server_RequestData", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("Server_RequestData", (BaseEntity) this, player, 6f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Server_RequestData(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Server_RequestData");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetState()
  {
    this.lastResetIndex = 0;
    this.cachedOutputsUsed = 0;
    this.lastPassthroughEnergy = 0;
    this.lastEnergy = 0;
    this.currentEnergy = 0;
    this.lastUpdateTime = 0.0f;
    this.ensureOutputsUpdated = false;
  }

  public string GetDisplayName()
  {
    if (Object.op_Inequality((Object) this.sourceItem, (Object) null))
      return this.sourceItem.displayName.translated;
    return this.ShortPrefabName;
  }

  public virtual bool IsRootEntity()
  {
    return false;
  }

  public virtual bool WantsPower()
  {
    return true;
  }

  public virtual bool IsPowered()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved8);
  }

  public bool IsConnectedTo(IOEntity entity, int slot, int depth, bool defaultReturn = false)
  {
    if (depth > 0 && slot < this.inputs.Length)
    {
      IOEntity.IOSlot input = this.inputs[slot];
      if (input.mainPowerSlot)
      {
        IOEntity ioEntity = input.connectedTo.Get(true);
        if (Object.op_Inequality((Object) ioEntity, (Object) null) && (Object.op_Equality((Object) ioEntity, (Object) entity) || ioEntity.IsConnectedTo(entity, depth - 1, defaultReturn)))
          return true;
      }
    }
    return false;
  }

  public bool IsConnectedTo(IOEntity entity, int depth, bool defaultReturn = false)
  {
    if (depth <= 0)
      return defaultReturn;
    for (int index = 0; index < this.inputs.Length; ++index)
    {
      IOEntity.IOSlot input = this.inputs[index];
      if (input.mainPowerSlot)
      {
        IOEntity ioEntity = input.connectedTo.Get(true);
        if (Object.op_Inequality((Object) ioEntity, (Object) null) && (Object.op_Equality((Object) ioEntity, (Object) entity) || ioEntity.IsConnectedTo(entity, depth - 1, defaultReturn)))
          return true;
      }
    }
    return false;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(6f)]
  private void Server_RequestData(BaseEntity.RPCMessage msg)
  {
    this.SendAdditionalData(msg.player);
  }

  public virtual void SendAdditionalData(BasePlayer player)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    this.ClientRPCPlayer<int, int, float, float>((Connection) null, player, "Client_ReceiveAdditionalData", this.currentEnergy, this.GetPassthroughAmount(0), num1, num2);
  }

  public static void ProcessQueue()
  {
    float realtimeSinceStartup = Time.get_realtimeSinceStartup();
    float num = IOEntity.framebudgetms / 1000f;
    while (IOEntity._processQueue.Count > 0 && (double) Time.get_realtimeSinceStartup() < (double) realtimeSinceStartup + (double) num)
    {
      IOEntity ioEntity = IOEntity._processQueue.Dequeue();
      if (Object.op_Inequality((Object) ioEntity, (Object) null))
        ioEntity.UpdateOutputs();
    }
  }

  public virtual void ResetIOState()
  {
  }

  public virtual void Init()
  {
    for (int index = 0; index < this.outputs.Length; ++index)
    {
      IOEntity.IOSlot output = this.outputs[index];
      output.connectedTo.Init();
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
      {
        int connectedToSlot = output.connectedToSlot;
        if (connectedToSlot < 0 || connectedToSlot >= output.connectedTo.Get(true).inputs.Length)
        {
          Debug.LogError((object) ("Slot IOR Error: " + ((Object) this).get_name() + " setting up inputs for " + ((Object) output.connectedTo.Get(true)).get_name() + " slot : " + (object) output.connectedToSlot));
        }
        else
        {
          output.connectedTo.Get(true).inputs[output.connectedToSlot].connectedTo.Set(this);
          output.connectedTo.Get(true).inputs[output.connectedToSlot].connectedToSlot = index;
          output.connectedTo.Get(true).inputs[output.connectedToSlot].connectedTo.Init();
        }
      }
    }
    if (!this.IsRootEntity())
      return;
    this.Invoke(new Action(this.MarkDirtyForceUpdateOutputs), Random.Range(1f, 1f));
  }

  internal override void DoServerDestroy()
  {
    if (this.isServer)
      this.Shutdown();
    base.DoServerDestroy();
  }

  public void ClearConnections()
  {
    List<IOEntity> ioEntityList = new List<IOEntity>();
    foreach (IOEntity.IOSlot input in this.inputs)
    {
      IOEntity ioEntity = (IOEntity) null;
      if (Object.op_Inequality((Object) input.connectedTo.Get(true), (Object) null))
      {
        ioEntity = input.connectedTo.Get(true);
        foreach (IOEntity.IOSlot output in input.connectedTo.Get(true).outputs)
        {
          if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null) && output.connectedTo.Get(true).EqualNetID((BaseNetworkable) this))
            output.Clear();
        }
      }
      input.Clear();
      if (Object.op_Implicit((Object) ioEntity))
        ioEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
      {
        ioEntityList.Add(output.connectedTo.Get(true));
        foreach (IOEntity.IOSlot input in output.connectedTo.Get(true).inputs)
        {
          if (Object.op_Inequality((Object) input.connectedTo.Get(true), (Object) null) && input.connectedTo.Get(true).EqualNetID((BaseNetworkable) this))
            input.Clear();
        }
      }
      if (Object.op_Implicit((Object) output.connectedTo.Get(true)))
        output.connectedTo.Get(true).UpdateFromInput(0, output.connectedToSlot);
      output.Clear();
    }
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    foreach (IOEntity ioEntity in ioEntityList)
    {
      if (Object.op_Inequality((Object) ioEntity, (Object) null))
      {
        ioEntity.MarkDirty();
        ioEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      }
    }
    for (int inputSlot = 0; inputSlot < this.inputs.Length; ++inputSlot)
      this.UpdateFromInput(0, inputSlot);
  }

  public void Shutdown()
  {
    this.ClearConnections();
  }

  public void MarkDirtyForceUpdateOutputs()
  {
    this.ensureOutputsUpdated = true;
    this.MarkDirty();
  }

  public virtual void MarkDirty()
  {
    if (this.isClient)
      return;
    this.cachedOutputsUsed = 0;
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
        ++this.cachedOutputsUsed;
    }
    this.TouchIOState();
  }

  public virtual int ConsumptionAmount()
  {
    return 1;
  }

  public virtual int GetCurrentEnergy()
  {
    return Mathf.Clamp(this.currentEnergy - this.ConsumptionAmount(), 0, this.currentEnergy);
  }

  public virtual int GetPassthroughAmount(int outputSlot = 0)
  {
    return Mathf.Clamp(this.GetCurrentEnergy(), 0, this.GetCurrentEnergy());
  }

  public virtual void UpdateHasPower(int inputAmount, int inputSlot)
  {
    this.SetFlag(BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount(), false, false);
  }

  public void TouchInternal()
  {
    int passthroughAmount = this.GetPassthroughAmount(0);
    int num = this.lastPassthroughEnergy != passthroughAmount ? 1 : 0;
    this.lastPassthroughEnergy = passthroughAmount;
    if (num != 0)
    {
      this.IOStateChanged(this.currentEnergy, 0);
      this.ensureOutputsUpdated = true;
    }
    IOEntity._processQueue.Enqueue(this);
  }

  public virtual void UpdateFromInput(int inputAmount, int inputSlot)
  {
    this.UpdateHasPower(inputAmount, inputSlot);
    this.lastEnergy = this.currentEnergy;
    this.currentEnergy = inputAmount;
    int passthroughAmount = this.GetPassthroughAmount(0);
    bool flag = this.lastPassthroughEnergy != passthroughAmount;
    this.lastPassthroughEnergy = passthroughAmount;
    if (this.currentEnergy != this.lastEnergy | flag)
    {
      this.IOStateChanged(inputAmount, inputSlot);
      this.ensureOutputsUpdated = true;
    }
    IOEntity._processQueue.Enqueue(this);
  }

  public virtual void TouchIOState()
  {
    if (this.isClient)
      return;
    this.TouchInternal();
  }

  public virtual void SendIONetworkUpdate()
  {
    this.SendNetworkUpdate_Flags();
  }

  public virtual void IOStateChanged(int inputAmount, int inputSlot)
  {
  }

  public virtual void UpdateOutputs()
  {
    if ((double) Time.get_realtimeSinceStartup() - (double) this.lastUpdateTime < (double) IOEntity.responsetime)
    {
      IOEntity._processQueue.Enqueue(this);
    }
    else
    {
      this.lastUpdateTime = Time.get_realtimeSinceStartup();
      this.SendIONetworkUpdate();
      if (this.outputs.Length == 0)
      {
        this.ensureOutputsUpdated = false;
      }
      else
      {
        if (!this.ensureOutputsUpdated)
          return;
        int inputAmount = Mathf.FloorToInt((float) this.GetPassthroughAmount(0) / (this.cachedOutputsUsed == 0 ? 1f : (float) this.cachedOutputsUsed));
        this.ensureOutputsUpdated = false;
        foreach (IOEntity.IOSlot output in this.outputs)
        {
          if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
            output.connectedTo.Get(true).UpdateFromInput(inputAmount, output.connectedToSlot);
        }
      }
    }
  }

  public override void Spawn()
  {
    base.Spawn();
    if (Application.isLoadingSave != null)
      return;
    this.Init();
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.Init();
  }

  public override void PostMapEntitySpawn()
  {
    base.PostMapEntitySpawn();
    this.Init();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.ioEntity = (__Null) Pool.Get<IOEntity>();
    ((IOEntity) info.msg.ioEntity).inputs = (__Null) Pool.GetList<IOEntity.IOConnection>();
    ((IOEntity) info.msg.ioEntity).outputs = (__Null) Pool.GetList<IOEntity.IOConnection>();
    foreach (IOEntity.IOSlot input in this.inputs)
    {
      IOEntity.IOConnection ioConnection = (IOEntity.IOConnection) Pool.Get<IOEntity.IOConnection>();
      ioConnection.connectedID = (__Null) (int) input.connectedTo.entityRef.uid;
      ioConnection.connectedToSlot = (__Null) input.connectedToSlot;
      ioConnection.niceName = (__Null) input.niceName;
      ioConnection.type = (__Null) input.type;
      ioConnection.inUse = (__Null) (ioConnection.connectedID > 0 ? 1 : 0);
      ((List<IOEntity.IOConnection>) ((IOEntity) info.msg.ioEntity).inputs).Add(ioConnection);
    }
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      IOEntity.IOConnection ioConnection = (IOEntity.IOConnection) Pool.Get<IOEntity.IOConnection>();
      ioConnection.connectedID = (__Null) (int) output.connectedTo.entityRef.uid;
      ioConnection.connectedToSlot = (__Null) output.connectedToSlot;
      ioConnection.niceName = (__Null) output.niceName;
      ioConnection.type = (__Null) output.type;
      ioConnection.inUse = (__Null) (ioConnection.connectedID > 0 ? 1 : 0);
      if (output.linePoints != null)
      {
        ioConnection.linePointList = (__Null) Pool.GetList<IOEntity.IOConnection.LineVec>();
        ((List<IOEntity.IOConnection.LineVec>) ioConnection.linePointList).Clear();
        foreach (Vector3 linePoint in output.linePoints)
        {
          IOEntity.IOConnection.LineVec lineVec = (IOEntity.IOConnection.LineVec) Pool.Get<IOEntity.IOConnection.LineVec>();
          lineVec.vec = (__Null) linePoint;
          ((List<IOEntity.IOConnection.LineVec>) ioConnection.linePointList).Add(lineVec);
        }
      }
      ((List<IOEntity.IOConnection>) ((IOEntity) info.msg.ioEntity).outputs).Add(ioConnection);
    }
  }

  public virtual float IOInput(
    IOEntity from,
    IOEntity.IOType inputType,
    float inputAmount,
    int slot = 0)
  {
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
        inputAmount = output.connectedTo.Get(true).IOInput(this, output.type, inputAmount, output.connectedToSlot);
    }
    return inputAmount;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    if (!info.fromDisk && ((IOEntity) info.msg.ioEntity).inputs != null)
    {
      int count = ((List<IOEntity.IOConnection>) ((IOEntity) info.msg.ioEntity).inputs).Count;
      if (this.inputs.Length != count)
        this.inputs = new IOEntity.IOSlot[count];
      for (int index = 0; index < count; ++index)
      {
        if (this.inputs[index] == null)
          this.inputs[index] = new IOEntity.IOSlot();
        IOEntity.IOConnection input = ((List<IOEntity.IOConnection>) ((IOEntity) info.msg.ioEntity).inputs)[index];
        this.inputs[index].connectedTo = new IOEntity.IORef();
        this.inputs[index].connectedTo.entityRef.uid = (uint) input.connectedID;
        if (this.isClient)
          this.inputs[index].connectedTo.InitClient();
        this.inputs[index].connectedToSlot = (int) input.connectedToSlot;
        this.inputs[index].niceName = (string) input.niceName;
        this.inputs[index].type = (IOEntity.IOType) input.type;
      }
    }
    if (((IOEntity) info.msg.ioEntity).outputs == null)
      return;
    if (!info.fromDisk && this.isClient)
    {
      foreach (IOEntity.IOSlot output in this.outputs)
        output.Clear();
    }
    int count1 = ((List<IOEntity.IOConnection>) ((IOEntity) info.msg.ioEntity).outputs).Count;
    if (this.outputs.Length != count1 && count1 > 0)
    {
      IOEntity.IOSlot[] outputs = this.outputs;
      this.outputs = new IOEntity.IOSlot[count1];
      for (int index = 0; index < outputs.Length; ++index)
      {
        if (index < count1)
          this.outputs[index] = outputs[index];
      }
    }
    for (int index1 = 0; index1 < count1; ++index1)
    {
      if (this.outputs[index1] == null)
        this.outputs[index1] = new IOEntity.IOSlot();
      IOEntity.IOConnection output = ((List<IOEntity.IOConnection>) ((IOEntity) info.msg.ioEntity).outputs)[index1];
      this.outputs[index1].connectedTo = new IOEntity.IORef();
      this.outputs[index1].connectedTo.entityRef.uid = (uint) output.connectedID;
      if (this.isClient)
        this.outputs[index1].connectedTo.InitClient();
      this.outputs[index1].connectedToSlot = (int) output.connectedToSlot;
      this.outputs[index1].niceName = (string) output.niceName;
      this.outputs[index1].type = (IOEntity.IOType) output.type;
      if (info.fromDisk || this.isClient)
      {
        List<IOEntity.IOConnection.LineVec> linePointList = (List<IOEntity.IOConnection.LineVec>) output.linePointList;
        if (this.outputs[index1].linePoints == null || this.outputs[index1].linePoints.Length != linePointList.Count)
          this.outputs[index1].linePoints = new Vector3[linePointList.Count];
        for (int index2 = 0; index2 < linePointList.Count; ++index2)
          this.outputs[index1].linePoints[index2] = (Vector3) linePointList[index2].vec;
      }
    }
  }

  public enum IOType
  {
    Electric,
    Fluidic,
    Kinetic,
    Generic,
  }

  [Serializable]
  public class IORef
  {
    public EntityRef entityRef;
    public IOEntity ioEnt;

    public void Init()
    {
      if (Object.op_Inequality((Object) this.ioEnt, (Object) null) && !this.entityRef.IsValid(true))
        this.entityRef.Set((BaseEntity) this.ioEnt);
      if (!this.entityRef.IsValid(true))
        return;
      this.ioEnt = (IOEntity) ((Component) this.entityRef.Get(true)).GetComponent<IOEntity>();
    }

    public void InitClient()
    {
      if (!this.entityRef.IsValid(false) || !Object.op_Equality((Object) this.ioEnt, (Object) null))
        return;
      this.ioEnt = (IOEntity) ((Component) this.entityRef.Get(false)).GetComponent<IOEntity>();
    }

    public IOEntity Get(bool isServer = true)
    {
      if (Object.op_Equality((Object) this.ioEnt, (Object) null) && this.entityRef.IsValid(isServer))
        this.ioEnt = (IOEntity) ((Component) this.entityRef.Get(isServer)).GetComponent<IOEntity>();
      return this.ioEnt;
    }

    public void Clear()
    {
      this.ioEnt = (IOEntity) null;
      this.entityRef.Set((BaseEntity) null);
    }

    public void Set(IOEntity newIOEnt)
    {
      this.entityRef.Set((BaseEntity) newIOEnt);
    }
  }

  [Serializable]
  public class IOSlot
  {
    public string niceName;
    public IOEntity.IOType type;
    public IOEntity.IORef connectedTo;
    public int connectedToSlot;
    public Vector3[] linePoints;
    public ClientIOLine line;
    public Vector3 handlePosition;
    public bool rootConnectionsOnly;
    public bool mainPowerSlot;

    public void Clear()
    {
      this.connectedTo.Clear();
      this.connectedToSlot = 0;
      this.linePoints = (Vector3[]) null;
    }
  }
}
