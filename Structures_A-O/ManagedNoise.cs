// Decompiled with JetBrains decompiler
// Type: ManagedNoise
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public static class ManagedNoise
{
  private static readonly int[] hash = new int[512]
  {
    151,
    160,
    137,
    91,
    90,
    15,
    131,
    13,
    201,
    95,
    96,
    53,
    194,
    233,
    7,
    225,
    140,
    36,
    103,
    30,
    69,
    142,
    8,
    99,
    37,
    240,
    21,
    10,
    23,
    190,
    6,
    148,
    247,
    120,
    234,
    75,
    0,
    26,
    197,
    62,
    94,
    252,
    219,
    203,
    117,
    35,
    11,
    32,
    57,
    177,
    33,
    88,
    237,
    149,
    56,
    87,
    174,
    20,
    125,
    136,
    171,
    168,
    68,
    175,
    74,
    165,
    71,
    134,
    139,
    48,
    27,
    166,
    77,
    146,
    158,
    231,
    83,
    111,
    229,
    122,
    60,
    211,
    133,
    230,
    220,
    105,
    92,
    41,
    55,
    46,
    245,
    40,
    244,
    102,
    143,
    54,
    65,
    25,
    63,
    161,
    1,
    216,
    80,
    73,
    209,
    76,
    132,
    187,
    208,
    89,
    18,
    169,
    200,
    196,
    135,
    130,
    116,
    188,
    159,
    86,
    164,
    100,
    109,
    198,
    173,
    186,
    3,
    64,
    52,
    217,
    226,
    250,
    124,
    123,
    5,
    202,
    38,
    147,
    118,
    126,
    (int) byte.MaxValue,
    82,
    85,
    212,
    207,
    206,
    59,
    227,
    47,
    16,
    58,
    17,
    182,
    189,
    28,
    42,
    223,
    183,
    170,
    213,
    119,
    248,
    152,
    2,
    44,
    154,
    163,
    70,
    221,
    153,
    101,
    155,
    167,
    43,
    172,
    9,
    129,
    22,
    39,
    253,
    19,
    98,
    108,
    110,
    79,
    113,
    224,
    232,
    178,
    185,
    112,
    104,
    218,
    246,
    97,
    228,
    251,
    34,
    242,
    193,
    238,
    210,
    144,
    12,
    191,
    179,
    162,
    241,
    81,
    51,
    145,
    235,
    249,
    14,
    239,
    107,
    49,
    192,
    214,
    31,
    181,
    199,
    106,
    157,
    184,
    84,
    204,
    176,
    115,
    121,
    50,
    45,
    (int) sbyte.MaxValue,
    4,
    150,
    254,
    138,
    236,
    205,
    93,
    222,
    114,
    67,
    29,
    24,
    72,
    243,
    141,
    128,
    195,
    78,
    66,
    215,
    61,
    156,
    180,
    151,
    160,
    137,
    91,
    90,
    15,
    131,
    13,
    201,
    95,
    96,
    53,
    194,
    233,
    7,
    225,
    140,
    36,
    103,
    30,
    69,
    142,
    8,
    99,
    37,
    240,
    21,
    10,
    23,
    190,
    6,
    148,
    247,
    120,
    234,
    75,
    0,
    26,
    197,
    62,
    94,
    252,
    219,
    203,
    117,
    35,
    11,
    32,
    57,
    177,
    33,
    88,
    237,
    149,
    56,
    87,
    174,
    20,
    125,
    136,
    171,
    168,
    68,
    175,
    74,
    165,
    71,
    134,
    139,
    48,
    27,
    166,
    77,
    146,
    158,
    231,
    83,
    111,
    229,
    122,
    60,
    211,
    133,
    230,
    220,
    105,
    92,
    41,
    55,
    46,
    245,
    40,
    244,
    102,
    143,
    54,
    65,
    25,
    63,
    161,
    1,
    216,
    80,
    73,
    209,
    76,
    132,
    187,
    208,
    89,
    18,
    169,
    200,
    196,
    135,
    130,
    116,
    188,
    159,
    86,
    164,
    100,
    109,
    198,
    173,
    186,
    3,
    64,
    52,
    217,
    226,
    250,
    124,
    123,
    5,
    202,
    38,
    147,
    118,
    126,
    (int) byte.MaxValue,
    82,
    85,
    212,
    207,
    206,
    59,
    227,
    47,
    16,
    58,
    17,
    182,
    189,
    28,
    42,
    223,
    183,
    170,
    213,
    119,
    248,
    152,
    2,
    44,
    154,
    163,
    70,
    221,
    153,
    101,
    155,
    167,
    43,
    172,
    9,
    129,
    22,
    39,
    253,
    19,
    98,
    108,
    110,
    79,
    113,
    224,
    232,
    178,
    185,
    112,
    104,
    218,
    246,
    97,
    228,
    251,
    34,
    242,
    193,
    238,
    210,
    144,
    12,
    191,
    179,
    162,
    241,
    81,
    51,
    145,
    235,
    249,
    14,
    239,
    107,
    49,
    192,
    214,
    31,
    181,
    199,
    106,
    157,
    184,
    84,
    204,
    176,
    115,
    121,
    50,
    45,
    (int) sbyte.MaxValue,
    4,
    150,
    254,
    138,
    236,
    205,
    93,
    222,
    114,
    67,
    29,
    24,
    72,
    243,
    141,
    128,
    195,
    78,
    66,
    215,
    61,
    156,
    180
  };
  private static double[] gradients1D = new double[2]
  {
    1.0,
    -1.0
  };
  private static double[] gradients2Dx = new double[8]
  {
    1.0,
    -1.0,
    0.0,
    0.0,
    0.707106781186548,
    -0.707106781186548,
    0.707106781186548,
    -0.707106781186548
  };
  private static double[] gradients2Dy = new double[8]
  {
    0.0,
    0.0,
    1.0,
    -1.0,
    0.707106781186548,
    0.707106781186548,
    -0.707106781186548,
    -0.707106781186548
  };
  private const int hashMask = 255;
  private const double sqrt2 = 1.4142135623731;
  private const double rsqrt2 = 0.707106781186548;
  private const double squaresToTriangles = 0.211324865405187;
  private const double trianglesToSquares = 0.366025403784439;
  private const double simplexScale1D = 2.40740740740741;
  private const double simplexScale2D = 32.9907739830396;
  private const double gradientScale2D = 4.0;
  private const int gradientsMask1D = 1;
  private const int gradientsMask2D = 7;

  public static double Simplex1D(double x)
  {
    double num1 = 0.0;
    int num2;
    int num3 = num2 = ManagedNoise.Floor(x);
    double num4 = x - (double) num3;
    double num5 = 1.0 - num4 * num4;
    if (num5 > 0.0)
    {
      double num6 = num5 * num5;
      double num7 = num5 * num6;
      int index = ManagedNoise.hash[num3 & (int) byte.MaxValue] & 1;
      double num8 = ManagedNoise.gradients1D[index] * num4;
      num1 += num8 * num7;
    }
    int num9 = num2 + 1;
    double num10 = x - (double) num9;
    double num11 = 1.0 - num10 * num10;
    if (num11 > 0.0)
    {
      double num6 = num11 * num11;
      double num7 = num11 * num6;
      int index = ManagedNoise.hash[num9 & (int) byte.MaxValue] & 1;
      double num8 = ManagedNoise.gradients1D[index] * num10;
      num1 += num8 * num7;
    }
    return num1 * (65.0 / 27.0);
  }

  public static double Simplex1D(double x, out double dx)
  {
    double num1 = 0.0;
    dx = 0.0;
    int num2;
    int num3 = num2 = ManagedNoise.Floor(x);
    double num4 = x - (double) num3;
    double num5 = 1.0 - num4 * num4;
    if (num5 > 0.0)
    {
      double num6 = num5 * num5;
      double num7 = num5 * num6;
      int index = ManagedNoise.hash[num3 & (int) byte.MaxValue] & 1;
      double num8 = ManagedNoise.gradients1D[index];
      double num9 = num8 * num4;
      double num10 = num9 * 6.0 * num6;
      dx += num8 * num7 - num10 * num4;
      num1 += num9 * num7;
    }
    int num11 = num2 + 1;
    double num12 = x - (double) num11;
    double num13 = 1.0 - num12 * num12;
    if (num13 > 0.0)
    {
      double num6 = num13 * num13;
      double num7 = num13 * num6;
      int index = ManagedNoise.hash[num11 & (int) byte.MaxValue] & 1;
      double num8 = ManagedNoise.gradients1D[index];
      double num9 = num8 * num12;
      double num10 = num9 * 6.0 * num6;
      dx += num8 * num7 - num10 * num12;
      num1 += num9 * num7;
    }
    return num1 * (65.0 / 27.0);
  }

  public static double Simplex2D(double x, double y)
  {
    double num1 = 0.0;
    double num2 = (x + y) * 0.366025403784439;
    double x1 = x + num2;
    double x2 = y + num2;
    int num3 = ManagedNoise.Floor(x1);
    int num4 = ManagedNoise.Floor(x2);
    int num5 = num3;
    int num6 = num4;
    double num7 = (double) (num5 + num6) * 0.211324865405187;
    double num8 = x - (double) num5 + num7;
    double num9 = y - (double) num6 + num7;
    double num10 = 0.5 - num8 * num8 - num9 * num9;
    if (num10 > 0.0)
    {
      double num11 = num10 * num10;
      double num12 = num10 * num11;
      int index = ManagedNoise.hash[ManagedNoise.hash[num5 & (int) byte.MaxValue] + num6 & (int) byte.MaxValue] & 7;
      double num13 = ManagedNoise.gradients2Dx[index];
      double num14 = ManagedNoise.gradients2Dy[index];
      double num15 = num8;
      double num16 = num13 * num15 + num14 * num9;
      num1 += num16 * num12;
    }
    int num17 = num3 + 1;
    int num18 = num4 + 1;
    double num19 = (double) (num17 + num18) * 0.211324865405187;
    double num20 = x - (double) num17 + num19;
    double num21 = y - (double) num18 + num19;
    double num22 = 0.5 - num20 * num20 - num21 * num21;
    if (num22 > 0.0)
    {
      double num11 = num22 * num22;
      double num12 = num22 * num11;
      int index = ManagedNoise.hash[ManagedNoise.hash[num17 & (int) byte.MaxValue] + num18 & (int) byte.MaxValue] & 7;
      double num13 = ManagedNoise.gradients2Dx[index];
      double num14 = ManagedNoise.gradients2Dy[index];
      double num15 = num20;
      double num16 = num13 * num15 + num14 * num21;
      num1 += num16 * num12;
    }
    if (x1 - (double) num3 >= x2 - (double) num4)
    {
      int num11 = num3 + 1;
      int num12 = num4;
      double num13 = (double) (num11 + num12) * 0.211324865405187;
      double num14 = x - (double) num11 + num13;
      double num15 = y - (double) num12 + num13;
      double num16 = 0.5 - num14 * num14 - num15 * num15;
      if (num16 > 0.0)
      {
        double num23 = num16 * num16;
        double num24 = num16 * num23;
        int index = ManagedNoise.hash[ManagedNoise.hash[num11 & (int) byte.MaxValue] + num12 & (int) byte.MaxValue] & 7;
        double num25 = ManagedNoise.gradients2Dx[index];
        double num26 = ManagedNoise.gradients2Dy[index];
        double num27 = num14;
        double num28 = num25 * num27 + num26 * num15;
        num1 += num28 * num24;
      }
    }
    else
    {
      int num11 = num3;
      int num12 = num4 + 1;
      double num13 = (double) (num11 + num12) * 0.211324865405187;
      double num14 = x - (double) num11 + num13;
      double num15 = y - (double) num12 + num13;
      double num16 = 0.5 - num14 * num14 - num15 * num15;
      if (num16 > 0.0)
      {
        double num23 = num16 * num16;
        double num24 = num16 * num23;
        int index = ManagedNoise.hash[ManagedNoise.hash[num11 & (int) byte.MaxValue] + num12 & (int) byte.MaxValue] & 7;
        double num25 = ManagedNoise.gradients2Dx[index];
        double num26 = ManagedNoise.gradients2Dy[index];
        double num27 = num14;
        double num28 = num25 * num27 + num26 * num15;
        num1 += num28 * num24;
      }
    }
    return num1 * 32.9907739830396;
  }

  public static double Simplex2D(double x, double y, out double dx, out double dy)
  {
    double num1 = 0.0;
    dx = 0.0;
    dy = 0.0;
    double num2 = (x + y) * 0.366025403784439;
    double x1 = x + num2;
    double x2 = y + num2;
    int num3 = ManagedNoise.Floor(x1);
    int num4 = ManagedNoise.Floor(x2);
    int num5 = num3;
    int num6 = num4;
    double num7 = (double) (num5 + num6) * 0.211324865405187;
    double num8 = x - (double) num5 + num7;
    double num9 = y - (double) num6 + num7;
    double num10 = 0.5 - num8 * num8 - num9 * num9;
    if (num10 > 0.0)
    {
      double num11 = num10 * num10;
      double num12 = num10 * num11;
      int index = ManagedNoise.hash[ManagedNoise.hash[num5 & (int) byte.MaxValue] + num6 & (int) byte.MaxValue] & 7;
      double num13 = ManagedNoise.gradients2Dx[index];
      double num14 = ManagedNoise.gradients2Dy[index];
      double num15 = num13 * num8 + num14 * num9;
      double num16 = num15 * 6.0 * num11;
      dx += num13 * num12 - num16 * num8;
      dy += num14 * num12 - num16 * num9;
      num1 += num15 * num12;
    }
    int num17 = num3 + 1;
    int num18 = num4 + 1;
    double num19 = (double) (num17 + num18) * 0.211324865405187;
    double num20 = x - (double) num17 + num19;
    double num21 = y - (double) num18 + num19;
    double num22 = 0.5 - num20 * num20 - num21 * num21;
    if (num22 > 0.0)
    {
      double num11 = num22 * num22;
      double num12 = num22 * num11;
      int index = ManagedNoise.hash[ManagedNoise.hash[num17 & (int) byte.MaxValue] + num18 & (int) byte.MaxValue] & 7;
      double num13 = ManagedNoise.gradients2Dx[index];
      double num14 = ManagedNoise.gradients2Dy[index];
      double num15 = num13 * num20 + num14 * num21;
      double num16 = num15 * 6.0 * num11;
      dx += num13 * num12 - num16 * num20;
      dy += num14 * num12 - num16 * num21;
      num1 += num15 * num12;
    }
    if (x1 - (double) num3 >= x2 - (double) num4)
    {
      int num11 = num3 + 1;
      int num12 = num4;
      double num13 = (double) (num11 + num12) * 0.211324865405187;
      double num14 = x - (double) num11 + num13;
      double num15 = y - (double) num12 + num13;
      double num16 = 0.5 - num14 * num14 - num15 * num15;
      if (num16 > 0.0)
      {
        double num23 = num16 * num16;
        double num24 = num16 * num23;
        int index = ManagedNoise.hash[ManagedNoise.hash[num11 & (int) byte.MaxValue] + num12 & (int) byte.MaxValue] & 7;
        double num25 = ManagedNoise.gradients2Dx[index];
        double num26 = ManagedNoise.gradients2Dy[index];
        double num27 = num25 * num14 + num26 * num15;
        double num28 = num27 * 6.0 * num23;
        dx += num25 * num24 - num28 * num14;
        dy += num26 * num24 - num28 * num15;
        num1 += num27 * num24;
      }
    }
    else
    {
      int num11 = num3;
      int num12 = num4 + 1;
      double num13 = (double) (num11 + num12) * 0.211324865405187;
      double num14 = x - (double) num11 + num13;
      double num15 = y - (double) num12 + num13;
      double num16 = 0.5 - num14 * num14 - num15 * num15;
      if (num16 > 0.0)
      {
        double num23 = num16 * num16;
        double num24 = num16 * num23;
        int index = ManagedNoise.hash[ManagedNoise.hash[num11 & (int) byte.MaxValue] + num12 & (int) byte.MaxValue] & 7;
        double num25 = ManagedNoise.gradients2Dx[index];
        double num26 = ManagedNoise.gradients2Dy[index];
        double num27 = num25 * num14 + num26 * num15;
        double num28 = num27 * 6.0 * num23;
        dx += num25 * num24 - num28 * num14;
        dy += num26 * num24 - num28 * num15;
        num1 += num27 * num24;
      }
    }
    dx *= 4.0;
    dy *= 4.0;
    return num1 * 32.9907739830396;
  }

  public static double Turbulence(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    for (int index = 0; index < octaves; ++index)
    {
      double num4 = ManagedNoise.Simplex2D(x * num2, y * num2);
      num1 += num3 * num4;
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double Billow(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    for (int index = 0; index < octaves; ++index)
    {
      double x1 = ManagedNoise.Simplex2D(x * num2, y * num2);
      num1 += num3 * ManagedNoise.Abs(x1);
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double Ridge(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    for (int index = 0; index < octaves; ++index)
    {
      double x1 = ManagedNoise.Simplex2D(x * num2, y * num2);
      num1 += num3 * (1.0 - ManagedNoise.Abs(x1));
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double Sharp(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    for (int index = 0; index < octaves; ++index)
    {
      double num4 = ManagedNoise.Simplex2D(x * num2, y * num2);
      num1 += num3 * (num4 * num4);
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double TurbulenceIQ(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    double num4 = 0.0;
    double num5 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double num6 = ManagedNoise.Simplex2D(x * num2, y * num2, out dx, out dy);
      num4 += dx;
      num5 += dy;
      num1 += num3 * num6 / (1.0 + (num4 * num4 + num5 * num5));
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double BillowIQ(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    double num4 = 0.0;
    double num5 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double x1 = ManagedNoise.Simplex2D(x * num2, y * num2, out dx, out dy);
      num4 += dx;
      num5 += dy;
      num1 += num3 * ManagedNoise.Abs(x1) / (1.0 + (num4 * num4 + num5 * num5));
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double RidgeIQ(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    double num4 = 0.0;
    double num5 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double x1 = ManagedNoise.Simplex2D(x * num2, y * num2, out dx, out dy);
      num4 += dx;
      num5 += dy;
      num1 += num3 * (1.0 - ManagedNoise.Abs(x1)) / (1.0 + (num4 * num4 + num5 * num5));
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double SharpIQ(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    double num4 = 0.0;
    double num5 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double num6 = ManagedNoise.Simplex2D(x * num2, y * num2, out dx, out dy);
      num4 += dx;
      num5 += dy;
      num1 += num3 * (num6 * num6) / (1.0 + (num4 * num4 + num5 * num5));
      num2 *= lacunarity;
      num3 *= gain;
    }
    return num1 * amplitude;
  }

  public static double TurbulenceWarp(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain,
    double warp)
  {
    x *= frequency;
    y *= frequency;
    double x1 = 0.0;
    double num1 = 1.0;
    double num2 = 1.0;
    double num3 = 0.0;
    double num4 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double num5 = ManagedNoise.Simplex2D((x + warp * num3) * num1, (y + warp * num4) * num1, out dx, out dy);
      x1 += num2 * num5;
      num3 += num2 * dx * -num5;
      num4 += num2 * dy * -num5;
      num1 *= lacunarity;
      num2 *= gain * ManagedNoise.Saturate(x1);
    }
    return x1 * amplitude;
  }

  public static double BillowWarp(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain,
    double warp)
  {
    x *= frequency;
    y *= frequency;
    double x1 = 0.0;
    double num1 = 1.0;
    double num2 = 1.0;
    double num3 = 0.0;
    double num4 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double x2 = ManagedNoise.Simplex2D((x + warp * num3) * num1, (y + warp * num4) * num1, out dx, out dy);
      x1 += num2 * ManagedNoise.Abs(x2);
      num3 += num2 * dx * -x2;
      num4 += num2 * dy * -x2;
      num1 *= lacunarity;
      num2 *= gain * ManagedNoise.Saturate(x1);
    }
    return x1 * amplitude;
  }

  public static double RidgeWarp(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain,
    double warp)
  {
    x *= frequency;
    y *= frequency;
    double x1 = 0.0;
    double num1 = 1.0;
    double num2 = 1.0;
    double num3 = 0.0;
    double num4 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double x2 = ManagedNoise.Simplex2D((x + warp * num3) * num1, (y + warp * num4) * num1, out dx, out dy);
      x1 += num2 * (1.0 - ManagedNoise.Abs(x2));
      num3 += num2 * dx * -x2;
      num4 += num2 * dy * -x2;
      num1 *= lacunarity;
      num2 *= gain * ManagedNoise.Saturate(x1);
    }
    return x1 * amplitude;
  }

  public static double SharpWarp(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain,
    double warp)
  {
    x *= frequency;
    y *= frequency;
    double x1 = 0.0;
    double num1 = 1.0;
    double num2 = 1.0;
    double num3 = 0.0;
    double num4 = 0.0;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double num5 = ManagedNoise.Simplex2D((x + warp * num3) * num1, (y + warp * num4) * num1, out dx, out dy);
      x1 += num2 * (num5 * num5);
      num3 += num2 * dx * -num5;
      num4 += num2 * dy * -num5;
      num1 *= lacunarity;
      num2 *= gain * ManagedNoise.Saturate(x1);
    }
    return x1 * amplitude;
  }

  public static double Jordan(
    double x,
    double y,
    int octaves,
    double frequency,
    double amplitude,
    double lacunarity,
    double gain,
    double warp,
    double damp,
    double damp_scale)
  {
    x *= frequency;
    y *= frequency;
    double num1 = 0.0;
    double num2 = 1.0;
    double num3 = 1.0;
    double num4 = 0.0;
    double num5 = 0.0;
    double num6 = 0.0;
    double num7 = 0.0;
    double num8 = num2 * gain;
    for (int index = 0; index < octaves; ++index)
    {
      double dx;
      double dy;
      double num9 = ManagedNoise.Simplex2D(x * num3 + num4, y * num3 + num5, out dx, out dy);
      double num10 = num9 * num9;
      double num11 = dx * num9;
      double num12 = dy * num9;
      num1 += num8 * num10;
      num4 += warp * num11;
      num5 += warp * num12;
      num6 += damp * num11;
      num7 += damp * num12;
      num3 *= lacunarity;
      num2 *= gain;
      num8 = num2 * (1.0 - damp_scale / (1.0 + (num6 * num6 + num7 * num7)));
    }
    return num1 * amplitude;
  }

  private static int Floor(double x)
  {
    if (x < 0.0)
      return (int) x - 1;
    return (int) x;
  }

  private static double Abs(double x)
  {
    if (x < 0.0)
      return -x;
    return x;
  }

  private static double Saturate(double x)
  {
    if (x > 1.0)
      return 1.0;
    if (x >= 0.0)
      return x;
    return 0.0;
  }
}
