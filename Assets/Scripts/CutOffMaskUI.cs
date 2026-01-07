using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

// Script para hacer la transicion fade-in y fade-out de circunferencia 
public class CutOffMaskUI : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}
