using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.MANAGER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/18 19:09:38 
	///创建者：Michael Corleone
	///类用途：管理魔方6个面，以及8个节点的相对关系
	/// </summary>
	public class ReletivePosition
	{
        private GameObject _flu;
        private GameObject _fru;
        private GameObject _fld;
        private GameObject _frd;

        private GameObject _blu;
        private GameObject _bru;
        private GameObject _bld;
        private GameObject _brd;
        private void Init(GameObject pIn_FLU , GameObject pIn_FRU, GameObject pIn_FLD, GameObject pIn_FRD,
            GameObject pIn_BLU, GameObject pIn_BRU, GameObject pIn_BLD, GameObject pIn_BRD)
        {
            _flu = pIn_FLU;
            _fru = pIn_FRU;
            _fld = pIn_FLD;
            _frd = pIn_FRD;

            _blu = pIn_BLU;
            _bru = pIn_BRU;
            _bld = pIn_BLD;
            _brd = pIn_BRD;
        }
    }
}
