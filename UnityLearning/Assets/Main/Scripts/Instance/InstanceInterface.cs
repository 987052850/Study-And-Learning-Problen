using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.INTERFACE
{
    interface IInit
    {
        void Init(GLOBAL.STRUCT.SInterface vIn_InitData);
    }
    interface IReset
    {
        void Reset(GLOBAL.STRUCT.SInterface vIn_InitData);
    }
}
