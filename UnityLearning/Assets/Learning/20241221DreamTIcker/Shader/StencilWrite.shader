Shader"Learning/HW/DreamTicker/StencilWrite"
{
	Properties
	{
		_StencilRef("Stencil Ref",Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp" , Float) = 8
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilPass("Stencil Pass" , Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilFail("Stencil Fail" , Float) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Geometry-100"
		}
		Pass
		{
	
			Stencil
			{
				Ref[_StencilRef]
				Comp[_StencilComp]
				Pass[_StencilPass]
				Fail[_StencilFail]
			}

			ZWrite off
			Cull off
			ColorMask 0
		}
	}
}