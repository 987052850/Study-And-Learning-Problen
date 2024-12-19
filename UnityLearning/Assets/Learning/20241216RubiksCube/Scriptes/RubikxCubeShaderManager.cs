using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.ENUM;

namespace TEN.MANAGER
{

	/// <summary>
	///项目 : TEN
	///日期：2024/12/18 21:32:43 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class RubikxCubeShaderManager : DESIGNMODEL.Singleton<RubikxCubeShaderManager>
	{
        private Texture _stateATex;
        private Texture _stateBTex;
        private RubikxCubeShaderManager()
        {
            _stateBTex = Resources.Load("Texture/RubiksCube/AA") as Texture;
            _stateATex = Resources.Load("Texture/RubiksCube/DD") as Texture;
        }
        public void SetState(GameObject pIn_RubiksCubeChild , ERubiksCubeInstanceState vIn_State)
        {
            ERubiksCubeInstanceState state = pIn_RubiksCubeChild.GetComponent<TEN.INSTANCE.CubeIntance>().MState;
            if (state == ERubiksCubeInstanceState.NORMAL)
            {
                return;
            }
            float alpha = vIn_State == ERubiksCubeInstanceState.TRANSPARENT_A ? 1 : 0.3f;
            Texture texture = vIn_State == ERubiksCubeInstanceState.TRANSPARENT_A ? _stateBTex : _stateATex;
            pIn_RubiksCubeChild.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
            pIn_RubiksCubeChild.GetComponent<MeshRenderer>().material.SetFloat("_AlphaScale", alpha);
        }
    }
}
