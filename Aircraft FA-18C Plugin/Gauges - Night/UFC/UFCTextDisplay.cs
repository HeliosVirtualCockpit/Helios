using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    /// <summary>
    /// Specialised Text display which only processes a single character from the 
    /// triggered value.
    /// </summary>
    /// <remarks>
    /// The oroginal use for this was to allow two halves of a character to be displayed, one
    /// in one textdisplay, and the other in a second textdisplay.  With transparency, this means
    /// a composite character can be rendered by the overlapping textdisplays.
    /// </remarks>
    [HeliosControl("Helios.Base.FA_18C.TextDisplay2", "UFC Text Display - Night", "Text Displays - Night", typeof(TextDisplayRenderer),HeliosControlFlags.NotShownInUI)]
    public class UFCTextDisplay2 : TextDisplayRect
    {
            private readonly HeliosValue _value;
            private int _valueIndex = 0;
            private int _valueLength = 0;

            public UFCTextDisplay2()
                : base("TextDisplay", new System.Windows.Size(100, 50))
            {
                _value = new HeliosValue(this, new BindingValue(false), "", "TextDisplay", "Value of this Text Display", "A text string.", BindingValueUnits.Text);
                _value.Execute += On_Execute;
                Values.Add(_value);
                Actions.Add(_value);
            }

            protected override void OnTextValueChange()
            {
                _value.SetValue(new BindingValue(_textValue), BypassTriggers);
            }

            void On_Execute(object action, HeliosActionEventArgs e)
            {
                BeginTriggerBypass(e.BypassCascadingTriggers);
                string tmpValue = e.Value.StringValue;
                if (tmpValue.Length == 2)
                {
                    tmpValue = tmpValue.Substring(_valueIndex, _valueLength);
                }
                TextValue = tmpValue;
                EndTriggerBypass(e.BypassCascadingTriggers);
            }
            public int TextIndex
            {
                get { return _valueIndex; } 
                set { _valueIndex = value; } 
            }
        public int TextLength
        {
            get { return _valueLength; }    
            set { _valueLength = value; }
        }
    }
}
