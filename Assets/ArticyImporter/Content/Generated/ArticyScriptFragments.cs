//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Articy.Unity;
using Articy.Unity.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Articy.Harmonybarktest.GlobalVariables
{
    
    
    [Articy.Unity.ArticyCodeGenerationHashAttribute(637317258347371565)]
    public class ArticyScriptFragments : BaseScriptFragments, ISerializationCallbackReceiver
    {
        
        #region Fields
        private System.Collections.Generic.Dictionary<int, System.Func<ArticyGlobalVariables, Articy.Unity.IBaseScriptMethodProvider, bool>> Conditions = new System.Collections.Generic.Dictionary<int, System.Func<ArticyGlobalVariables, Articy.Unity.IBaseScriptMethodProvider, bool>>();
        
        private System.Collections.Generic.Dictionary<int, System.Action<ArticyGlobalVariables, Articy.Unity.IBaseScriptMethodProvider>> Instructions = new System.Collections.Generic.Dictionary<int, System.Action<ArticyGlobalVariables, Articy.Unity.IBaseScriptMethodProvider>>();
        #endregion
        
        #region Unity serialization
        public override void OnBeforeSerialize()
        {
        }
        
        public override void OnAfterDeserialize()
        {
            Conditions = new System.Collections.Generic.Dictionary<int, System.Func<ArticyGlobalVariables, Articy.Unity.IBaseScriptMethodProvider, bool>>();
            Instructions = new System.Collections.Generic.Dictionary<int, System.Action<ArticyGlobalVariables, Articy.Unity.IBaseScriptMethodProvider>>();
        }
        #endregion
        
        #region Script execution
        public override void CallInstruction(Articy.Unity.Interfaces.IGlobalVariables aGlobalVariables, int aHash, Articy.Unity.IBaseScriptMethodProvider aMethodProvider)
        {
            if ((Instructions.ContainsKey(aHash) == false))
            {
                return;
            }
            if ((aMethodProvider != null))
            {
                aMethodProvider.IsCalledInForecast = aGlobalVariables.IsInShadowState;
            }
            Instructions[aHash].Invoke(((ArticyGlobalVariables)(aGlobalVariables)), aMethodProvider);
        }
        
        public override bool CallCondition(Articy.Unity.Interfaces.IGlobalVariables aGlobalVariables, int aHash, Articy.Unity.IBaseScriptMethodProvider aMethodProvider)
        {
            if ((Conditions.ContainsKey(aHash) == false))
            {
                return true;
            }
            if ((aMethodProvider != null))
            {
                aMethodProvider.IsCalledInForecast = aGlobalVariables.IsInShadowState;
            }
            return Conditions[aHash].Invoke(((ArticyGlobalVariables)(aGlobalVariables)), aMethodProvider);
        }
        #endregion
    }
}
