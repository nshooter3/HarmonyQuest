//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Articy.Harmonybarktest;
using Articy.Unity;
using Articy.Unity.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Articy.Harmonybarktest.Features
{
    
    
    [Serializable()]
    public class DefaultExtendedCharacterFeatureFeature : IArticyBaseObject, IPropertyProvider
    {
        
        [SerializeField()]
        private String mMotivation;
        
        [SerializeField()]
        private String mInnerConflict;
        
        [SerializeField()]
        private String mSkills;
        
        [SerializeField()]
        private String mFears;
        
        [SerializeField()]
        private String mHabits;
        
        [SerializeField()]
        private String mFurtherDetails;
        
        [SerializeField()]
        private UInt64 mOwnerId;
        
        [SerializeField()]
        private UInt32 mOwnerInstanceId;
        
        public String Unresolved_Motivation
        {
            get
            {
                return mMotivation;
            }
        }
        
        public String Motivation
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mMotivation);
            }
            set
            {
                var oldValue = mMotivation;
                mMotivation = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultExtendedCharacterFeature.Motivation", oldValue, mMotivation);
            }
        }
        
        public String Unresolved_InnerConflict
        {
            get
            {
                return mInnerConflict;
            }
        }
        
        public String InnerConflict
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mInnerConflict);
            }
            set
            {
                var oldValue = mInnerConflict;
                mInnerConflict = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultExtendedCharacterFeature.InnerConflict", oldValue, mInnerConflict);
            }
        }
        
        public String Unresolved_Skills
        {
            get
            {
                return mSkills;
            }
        }
        
        public String Skills
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mSkills);
            }
            set
            {
                var oldValue = mSkills;
                mSkills = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultExtendedCharacterFeature.Skills", oldValue, mSkills);
            }
        }
        
        public String Unresolved_Fears
        {
            get
            {
                return mFears;
            }
        }
        
        public String Fears
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mFears);
            }
            set
            {
                var oldValue = mFears;
                mFears = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultExtendedCharacterFeature.Fears", oldValue, mFears);
            }
        }
        
        public String Unresolved_Habits
        {
            get
            {
                return mHabits;
            }
        }
        
        public String Habits
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mHabits);
            }
            set
            {
                var oldValue = mHabits;
                mHabits = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultExtendedCharacterFeature.Habits", oldValue, mHabits);
            }
        }
        
        public String Unresolved_FurtherDetails
        {
            get
            {
                return mFurtherDetails;
            }
        }
        
        public String FurtherDetails
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mFurtherDetails);
            }
            set
            {
                var oldValue = mFurtherDetails;
                mFurtherDetails = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultExtendedCharacterFeature.FurtherDetails", oldValue, mFurtherDetails);
            }
        }
        
        public UInt64 OwnerId
        {
            get
            {
                return mOwnerId;
            }
            set
            {
                mOwnerId = value;
            }
        }
        
        public UInt32 OwnerInstanceId
        {
            get
            {
                return mOwnerInstanceId;
            }
            set
            {
                mOwnerInstanceId = value;
            }
        }
        
        private void CloneProperties(object aClone, Articy.Unity.ArticyObject aFirstClassParent)
        {
            Articy.Harmonybarktest.Features.DefaultExtendedCharacterFeatureFeature newClone = ((Articy.Harmonybarktest.Features.DefaultExtendedCharacterFeatureFeature)(aClone));
            newClone.Motivation = Unresolved_Motivation;
            newClone.InnerConflict = Unresolved_InnerConflict;
            newClone.Skills = Unresolved_Skills;
            newClone.Fears = Unresolved_Fears;
            newClone.Habits = Unresolved_Habits;
            newClone.FurtherDetails = Unresolved_FurtherDetails;
            newClone.OwnerId = OwnerId;
        }
        
        public object CloneObject(object aParent, Articy.Unity.ArticyObject aFirstClassParent)
        {
            Articy.Harmonybarktest.Features.DefaultExtendedCharacterFeatureFeature clone = new Articy.Harmonybarktest.Features.DefaultExtendedCharacterFeatureFeature();
            CloneProperties(clone, aFirstClassParent);
            return clone;
        }
        
        public virtual bool IsLocalizedPropertyOverwritten(string aProperty)
        {
            return false;
        }
        
        #region property provider interface
        public void setProp(string aProperty, object aValue)
        {
            if ((aProperty == "Motivation"))
            {
                Motivation = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "InnerConflict"))
            {
                InnerConflict = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Skills"))
            {
                Skills = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Fears"))
            {
                Fears = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Habits"))
            {
                Habits = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "FurtherDetails"))
            {
                FurtherDetails = System.Convert.ToString(aValue);
                return;
            }
        }
        
        public Articy.Unity.Interfaces.ScriptDataProxy getProp(string aProperty)
        {
            if ((aProperty == "Motivation"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Motivation);
            }
            if ((aProperty == "InnerConflict"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(InnerConflict);
            }
            if ((aProperty == "Skills"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Skills);
            }
            if ((aProperty == "Fears"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Fears);
            }
            if ((aProperty == "Habits"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Habits);
            }
            if ((aProperty == "FurtherDetails"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(FurtherDetails);
            }
            return null;
        }
        #endregion
    }
}
