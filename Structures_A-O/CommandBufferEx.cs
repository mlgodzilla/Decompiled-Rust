// Decompiled with JetBrains decompiler
// Type: CommandBufferEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

public static class CommandBufferEx
{
  public static void BlitArray(
    this CommandBuffer cb,
    Mesh blitMesh,
    RenderTargetIdentifier source,
    Material mat,
    int slice,
    int pass = 0)
  {
    cb.SetGlobalTexture("_Source", source);
    cb.SetGlobalFloat("_SourceMip", 0.0f);
    if (slice >= 0)
    {
      cb.SetGlobalFloat("_SourceSlice", (float) slice);
      cb.SetGlobalInt("_TargetSlice", slice);
    }
    cb.DrawMesh(blitMesh, Matrix4x4.get_identity(), mat, 0, pass);
  }

  public static void BlitArray(
    this CommandBuffer cb,
    Mesh blitMesh,
    RenderTargetIdentifier source,
    Texture target,
    Material mat,
    int slice,
    int pass = 0)
  {
    cb.SetRenderTarget(RenderTargetIdentifier.op_Implicit(target), 0, (CubemapFace) 0, -1);
    cb.SetGlobalTexture("_Source", source);
    cb.SetGlobalFloat("_SourceMip", 0.0f);
    if (slice >= 0)
    {
      cb.SetGlobalFloat("_SourceSlice", (float) slice);
      cb.SetGlobalInt("_TargetSlice", slice);
    }
    cb.DrawMesh(blitMesh, Matrix4x4.get_identity(), mat, 0, pass);
  }

  public static void BlitArrayMip(
    this CommandBuffer cb,
    Mesh blitMesh,
    Texture source,
    int sourceMip,
    int sourceSlice,
    Texture target,
    int targetMip,
    int targetSlice,
    Material mat,
    int pass = 0)
  {
    int num1 = source.get_width() >> sourceMip;
    int num2 = source.get_height() >> sourceMip;
    Vector4 vector4_1;
    ((Vector4) ref vector4_1).\u002Ector(1f / (float) num1, 1f / (float) num2, (float) num1, (float) num2);
    int num3 = target.get_width() >> targetMip;
    int num4 = target.get_height() >> targetMip;
    Vector4 vector4_2;
    ((Vector4) ref vector4_2).\u002Ector(1f / (float) num3, 1f / (float) num4, (float) num3, (float) num4);
    cb.SetGlobalTexture("_Source", RenderTargetIdentifier.op_Implicit(source));
    cb.SetGlobalVector("_Source_TexelSize", vector4_1);
    cb.SetGlobalVector("_Target_TexelSize", vector4_2);
    cb.SetGlobalFloat("_SourceMip", (float) sourceMip);
    if (sourceSlice >= 0)
    {
      cb.SetGlobalFloat("_SourceSlice", (float) sourceSlice);
      cb.SetGlobalInt("_TargetSlice", targetSlice);
    }
    cb.SetRenderTarget(RenderTargetIdentifier.op_Implicit(target), targetMip, (CubemapFace) 0, -1);
    cb.DrawMesh(blitMesh, Matrix4x4.get_identity(), mat, 0, pass);
  }

  public static void BlitMip(
    this CommandBuffer cb,
    Mesh blitMesh,
    Texture source,
    Texture target,
    int mip,
    int slice,
    Material mat,
    int pass = 0)
  {
    cb.BlitArrayMip(blitMesh, source, mip, slice, target, mip, slice, mat, pass);
  }
}
