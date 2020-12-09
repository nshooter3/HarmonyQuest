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
using System.Linq;
using UnityEngine;


namespace Articy.Harmonybarktest
{
    
    
    public class Dialogue : ArticyObject, IDialogue, IPropertyProvider, IInputPinsOwner, IOutputPinsOwner, IObjectWithColor, IObjectWithDisplayName, IObjectWithUnresolvedDisplayName, IObjectWithPreviewImage, IObjectWithText, IObjectWithUnresolvedText, IObjectWithAttachments, IObjectWithExternalId, IObjectWithShortId, IObjectWithPosition, IObjectWithZIndex, IObjectWithSize
    {
        
        [SerializeField()]
        private ArticyValueArticyModelList mAttachments = new ArticyValueArticyModelList();
        
        [SerializeField()]
        private String mDisplayName;
        
        [SerializeField()]
        private PreviewImage mPreviewImage = new PreviewImage();
        
        [SerializeField()]
        private Color mColor;
        
        [SerializeField()]
        private String mText;
        
        [SerializeField()]
        private String mExternalId;
        
        [SerializeField()]
        private Vector2 mPosition;
        
        [SerializeField()]
        private Single mZIndex;
        
        [SerializeField()]
        private Vector2 mSize;
        
        [SerializeField()]
        private UInt32 mShortId;
        
        [SerializeField()]
        private ArticyValueListInputPin mInputPins = new ArticyValueListInputPin();
        
        [SerializeField()]
        private ArticyValueListOutputPin mOutputPins = new ArticyValueListOutputPin();
        
        public List<ArticyObject> Attachments
        {
            get
            {
                return mAttachments.GetValue();
            }
            set
            {
                var oldValue = mAttachments;
                mAttachments.SetValue(value);
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "Attachments", oldValue.GetValue(), mAttachments.GetValue());
            }
        }
        
        public String Unresolved_DisplayName
        {
            get
            {
                return mDisplayName;
            }
        }
        
        public String DisplayName
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mDisplayName);
            }
            set
            {
                var oldValue = mDisplayName;
                mDisplayName = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "DisplayName", oldValue, mDisplayName);
            }
        }
        
        public PreviewImage PreviewImage
        {
            get
            {
                return mPreviewImage;
            }
            set
            {
                var oldValue = mPreviewImage;
                mPreviewImage = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "PreviewImage", oldValue, mPreviewImage);
            }
        }
        
        public Color Color
        {
            get
            {
                return mColor;
            }
            set
            {
                var oldValue = mColor;
                mColor = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "Color", oldValue, mColor);
            }
        }
        
        public String Unresolved_Text
        {
            get
            {
                return mText;
            }
        }
        
        public String Text
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mText);
            }
            set
            {
                var oldValue = mText;
                mText = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "Text", oldValue, mText);
            }
        }
        
        public String ExternalId
        {
            get
            {
                return mExternalId;
            }
            set
            {
                var oldValue = mExternalId;
                mExternalId = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "ExternalId", oldValue, mExternalId);
            }
        }
        
        public Vector2 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                var oldValue = mPosition;
                mPosition = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "Position", oldValue, mPosition);
            }
        }
        
        public Single ZIndex
        {
            get
            {
                return mZIndex;
            }
            set
            {
                var oldValue = mZIndex;
                mZIndex = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "ZIndex", oldValue, mZIndex);
            }
        }
        
        public Vector2 Size
        {
            get
            {
                return mSize;
            }
            set
            {
                var oldValue = mSize;
                mSize = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "Size", oldValue, mSize);
            }
        }
        
        public UInt32 ShortId
        {
            get
            {
                return mShortId;
            }
            set
            {
                var oldValue = mShortId;
                mShortId = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "ShortId", oldValue, mShortId);
            }
        }
        
        public List<InputPin> InputPins
        {
            get
            {
                return mInputPins.GetValue();
            }
            set
            {
                var oldValue = mInputPins;
                mInputPins.SetValue(value);
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "InputPins", oldValue.GetValue(), mInputPins.GetValue());
            }
        }
        
        public List<OutputPin> OutputPins
        {
            get
            {
                return mOutputPins.GetValue();
            }
            set
            {
                var oldValue = mOutputPins;
                mOutputPins.SetValue(value);
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(Id, InstanceId, "OutputPins", oldValue.GetValue(), mOutputPins.GetValue());
            }
        }
        
        public List<Articy.Unity.Interfaces.IInputPin> GetInputPins()
        {
            return InputPins.Cast<IInputPin>().ToList();
        }
        
        public List<Articy.Unity.Interfaces.IOutputPin> GetOutputPins()
        {
            return OutputPins.Cast<IOutputPin>().ToList();
        }
        
        protected override void CloneProperties(object aClone, Articy.Unity.ArticyObject aFirstClassParent)
        {
            Dialogue newClone = ((Dialogue)(aClone));
            mAttachments.CustomClone(newClone.mAttachments);
            newClone.DisplayName = Unresolved_DisplayName;
            newClone.PreviewImage = PreviewImage;
            newClone.Color = Color;
            newClone.Text = Unresolved_Text;
            newClone.ExternalId = ExternalId;
            newClone.Position = Position;
            newClone.ZIndex = ZIndex;
            newClone.Size = Size;
            newClone.ShortId = ShortId;
            List<InputPin> temp_InputPins = new List<InputPin>();
            int i = 0;
            for (i = 0; (i < InputPins.Count); i = (i + 1))
            {
                temp_InputPins.Add(((InputPin)(InputPins[i].CloneObject(newClone, aFirstClassParent))));
            }
            newClone.InputPins = temp_InputPins;
            List<OutputPin> temp_OutputPins = new List<OutputPin>();
            for (i = 0; (i < OutputPins.Count); i = (i + 1))
            {
                temp_OutputPins.Add(((OutputPin)(OutputPins[i].CloneObject(newClone, aFirstClassParent))));
            }
            newClone.OutputPins = temp_OutputPins;
            base.CloneProperties(newClone, aFirstClassParent);
        }
        
        public override bool IsLocalizedPropertyOverwritten(string aProperty)
        {
            return base.IsLocalizedPropertyOverwritten(aProperty);
        }
        
        #region property provider interface
        public override void setProp(string aProperty, object aValue)
        {
            if ((aProperty == "Attachments"))
            {
                Attachments = ((List<ArticyObject>)(aValue));
                return;
            }
            if ((aProperty == "DisplayName"))
            {
                DisplayName = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "PreviewImage"))
            {
                PreviewImage = ((PreviewImage)(aValue));
                return;
            }
            if ((aProperty == "Color"))
            {
                Color = ((Color)(aValue));
                return;
            }
            if ((aProperty == "Text"))
            {
                Text = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "ExternalId"))
            {
                ExternalId = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Position"))
            {
                Position = ((Vector2)(aValue));
                return;
            }
            if ((aProperty == "ZIndex"))
            {
                ZIndex = System.Convert.ToSingle(aValue);
                return;
            }
            if ((aProperty == "Size"))
            {
                Size = ((Vector2)(aValue));
                return;
            }
            if ((aProperty == "ShortId"))
            {
                ShortId = ((UInt32)(aValue));
                return;
            }
            if ((aProperty == "InputPins"))
            {
                InputPins = ((List<InputPin>)(aValue));
                return;
            }
            if ((aProperty == "OutputPins"))
            {
                OutputPins = ((List<OutputPin>)(aValue));
                return;
            }
            base.setProp(aProperty, aValue);
        }
        
        public override Articy.Unity.Interfaces.ScriptDataProxy getProp(string aProperty)
        {
            if ((aProperty == "Attachments"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Attachments);
            }
            if ((aProperty == "DisplayName"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(DisplayName);
            }
            if ((aProperty == "PreviewImage"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(PreviewImage);
            }
            if ((aProperty == "Color"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Color);
            }
            if ((aProperty == "Text"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Text);
            }
            if ((aProperty == "ExternalId"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(ExternalId);
            }
            if ((aProperty == "Position"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Position);
            }
            if ((aProperty == "ZIndex"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(ZIndex);
            }
            if ((aProperty == "Size"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Size);
            }
            if ((aProperty == "ShortId"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(ShortId);
            }
            if ((aProperty == "InputPins"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(InputPins);
            }
            if ((aProperty == "OutputPins"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(OutputPins);
            }
            return base.getProp(aProperty);
        }
        #endregion
    }
}
