// Decompiled with JetBrains decompiler
// Type: BaseCommandBuffer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseCommandBuffer : MonoBehaviour
{
  private Dictionary<Camera, Dictionary<int, CommandBuffer>> cameras;

  protected CommandBuffer GetCommandBuffer(
    string name,
    Camera camera,
    CameraEvent cameraEvent)
  {
    Dictionary<int, CommandBuffer> dictionary;
    if (!this.cameras.TryGetValue(camera, out dictionary))
    {
      dictionary = new Dictionary<int, CommandBuffer>();
      this.cameras.Add(camera, dictionary);
    }
    CommandBuffer commandBuffer;
    if (dictionary.TryGetValue((int) cameraEvent, out commandBuffer))
    {
      commandBuffer.Clear();
    }
    else
    {
      commandBuffer = new CommandBuffer();
      commandBuffer.set_name(name);
      dictionary.Add((int) cameraEvent, commandBuffer);
      this.CleanupCamera(name, camera, cameraEvent);
      camera.AddCommandBuffer(cameraEvent, commandBuffer);
    }
    return commandBuffer;
  }

  protected void CleanupCamera(string name, Camera camera, CameraEvent cameraEvent)
  {
    foreach (CommandBuffer commandBuffer in camera.GetCommandBuffers(cameraEvent))
    {
      if (commandBuffer.get_name() == name)
        camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
    }
  }

  protected void CleanupCommandBuffer(Camera camera, CameraEvent cameraEvent)
  {
    Dictionary<int, CommandBuffer> dictionary;
    CommandBuffer commandBuffer;
    if (!this.cameras.TryGetValue(camera, out dictionary) || !dictionary.TryGetValue((int) cameraEvent, out commandBuffer))
      return;
    camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
  }

  protected void Cleanup()
  {
    using (Dictionary<Camera, Dictionary<int, CommandBuffer>>.Enumerator enumerator1 = this.cameras.GetEnumerator())
    {
      while (enumerator1.MoveNext())
      {
        KeyValuePair<Camera, Dictionary<int, CommandBuffer>> current1 = enumerator1.Current;
        Camera key1 = current1.Key;
        Dictionary<int, CommandBuffer> dictionary = current1.Value;
        if (Object.op_Implicit((Object) key1))
        {
          using (Dictionary<int, CommandBuffer>.Enumerator enumerator2 = dictionary.GetEnumerator())
          {
            while (enumerator2.MoveNext())
            {
              KeyValuePair<int, CommandBuffer> current2 = enumerator2.Current;
              int key2 = current2.Key;
              CommandBuffer commandBuffer = current2.Value;
              key1.RemoveCommandBuffer((CameraEvent) key2, commandBuffer);
            }
          }
        }
      }
    }
  }

  public BaseCommandBuffer()
  {
    base.\u002Ector();
  }
}
