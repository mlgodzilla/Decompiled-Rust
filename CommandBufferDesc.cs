// Decompiled with JetBrains decompiler
// Type: CommandBufferDesc
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine.Rendering;

public class CommandBufferDesc
{
  public CameraEvent CameraEvent { get; private set; }

  public int OrderId { get; private set; }

  public Action<CommandBuffer> FillDelegate { get; private set; }

  public CommandBufferDesc(
    CameraEvent cameraEvent,
    int orderId,
    CommandBufferDesc.FillCommandBuffer fill)
  {
    this.CameraEvent = cameraEvent;
    this.OrderId = orderId;
    this.FillDelegate = new Action<CommandBuffer>(fill.Invoke);
  }

  public delegate void FillCommandBuffer(CommandBuffer cb);
}
