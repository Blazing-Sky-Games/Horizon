using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Horizon.Core.GL
{
	public struct GLSettings
	{
		public readonly Color color;
		public readonly BlendMode SrcBlend;
		public readonly BlendMode DstBlend;
		public readonly CullMode CullMode;
		public readonly bool ZWrite;
		public readonly CompareFunction ZTest;
		
		public GLSettings(
			Color color,
			BlendMode SrcBlend,
			BlendMode DstBlend,
			CullMode  CullMode,
			bool ZWrite,
			CompareFunction ZTest)
		{
			this.color = color;
			this.SrcBlend = SrcBlend;
			this.DstBlend = DstBlend;
			this.CullMode = CullMode;
			this.ZWrite = ZWrite;
			this.ZTest = ZTest;
		}
	}
}

