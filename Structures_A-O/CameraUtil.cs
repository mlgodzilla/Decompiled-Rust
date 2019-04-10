// Decompiled with JetBrains decompiler
// Type: CameraUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class CameraUtil
{
  public static void NormalizePlane(ref Plane plane)
  {
    Vector3 normal = ((Plane) ref plane).get_normal();
    float num = (float) (1.0 / (double) ((Vector3) ref normal).get_magnitude());
    ref Plane local1 = ref plane;
    ((Plane) ref local1).set_normal(Vector3.op_Multiply(((Plane) ref local1).get_normal(), num));
    ref Plane local2 = ref plane;
    ((Plane) ref local2).set_distance(((Plane) ref local2).get_distance() * num);
  }

  public static void ExtractPlanes(Camera camera, ref Plane[] planes)
  {
    Matrix4x4 worldToCameraMatrix = camera.get_worldToCameraMatrix();
    CameraUtil.ExtractPlanes(Matrix4x4.op_Multiply(GL.GetGPUProjectionMatrix(camera.get_projectionMatrix(), false), worldToCameraMatrix), ref planes);
  }

  public static void ExtractPlanes(Matrix4x4 viewProjMatrix, ref Plane[] planes)
  {
    ((Plane) ref planes[0]).set_normal(new Vector3((float) (viewProjMatrix.m30 + viewProjMatrix.m00), (float) (viewProjMatrix.m31 + viewProjMatrix.m01), (float) (viewProjMatrix.m32 + viewProjMatrix.m02)));
    ((Plane) ref planes[0]).set_distance((float) (viewProjMatrix.m33 + viewProjMatrix.m03));
    CameraUtil.NormalizePlane(ref planes[0]);
    ((Plane) ref planes[1]).set_normal(new Vector3((float) (viewProjMatrix.m30 - viewProjMatrix.m00), (float) (viewProjMatrix.m31 - viewProjMatrix.m01), (float) (viewProjMatrix.m32 - viewProjMatrix.m02)));
    ((Plane) ref planes[1]).set_distance((float) (viewProjMatrix.m33 - viewProjMatrix.m03));
    CameraUtil.NormalizePlane(ref planes[1]);
    ((Plane) ref planes[2]).set_normal(new Vector3((float) (viewProjMatrix.m30 - viewProjMatrix.m10), (float) (viewProjMatrix.m31 - viewProjMatrix.m11), (float) (viewProjMatrix.m32 - viewProjMatrix.m12)));
    ((Plane) ref planes[2]).set_distance((float) (viewProjMatrix.m33 - viewProjMatrix.m13));
    CameraUtil.NormalizePlane(ref planes[2]);
    ((Plane) ref planes[3]).set_normal(new Vector3((float) (viewProjMatrix.m30 + viewProjMatrix.m10), (float) (viewProjMatrix.m31 + viewProjMatrix.m11), (float) (viewProjMatrix.m32 + viewProjMatrix.m12)));
    ((Plane) ref planes[3]).set_distance((float) (viewProjMatrix.m33 + viewProjMatrix.m13));
    CameraUtil.NormalizePlane(ref planes[3]);
    ((Plane) ref planes[4]).set_normal(new Vector3((float) viewProjMatrix.m20, (float) viewProjMatrix.m21, (float) viewProjMatrix.m22));
    ((Plane) ref planes[4]).set_distance((float) viewProjMatrix.m23);
    CameraUtil.NormalizePlane(ref planes[4]);
    ((Plane) ref planes[5]).set_normal(new Vector3((float) (viewProjMatrix.m30 - viewProjMatrix.m20), (float) (viewProjMatrix.m31 - viewProjMatrix.m21), (float) (viewProjMatrix.m32 - viewProjMatrix.m22)));
    ((Plane) ref planes[5]).set_distance((float) (viewProjMatrix.m33 - viewProjMatrix.m23));
    CameraUtil.NormalizePlane(ref planes[5]);
  }
}
