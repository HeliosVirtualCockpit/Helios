using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Controls;
using GadrocsWorkshop.Helios.Util;
using SharpDX.IO;
using System.Text.RegularExpressions;
using static GadrocsWorkshop.Helios.NativeMethods;


namespace GadrocsWorkshop.Helios.Gauges.F15E.UFC
{
    /// <summary>
    /// Specialised Text display needed to handle colons, decimals, and degrees for the 14 segment display used by the F-15E 
    /// </summary>
    /// <remarks>
    /// The main character set is a 14 segment display, but there are two independent dots used for decimal, degree and colon or more usually nothing.
    /// The font has decimal, colon, degree and a blank of the same width (mapped to !) as 160 wide, and the 14 segment portion 640 wide.  Every 
    /// character needs to be formed from two characters - the first for the dots and the second for the main character.  Thus a normal A needs to be
    /// "!A" a space would need to be "! "
    /// </remarks>
    [HeliosControl("Helios.Base.F15E.UFCTextDisplay", "UFC Text Display", "Text Displays", typeof(TextDisplayRenderer),HeliosControlFlags.NotShownInUI)]
    public class UFCTextDisplay : TextDisplayRect
    {
        private readonly HeliosValue _value;
        private int _valueIndex = 0;
        private int _valueLength = 0;

        public UFCTextDisplay()
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
            string[] parts = Tokenizer.TokenizeAtLeast(e.Value.StringValue.Replace('!',':'), 4, ';');
            if (parts[0] == "5")
            {
                if (parts[4] == ".")
                {
                    parts[1] = Regex.Replace(parts[1], @"(\d{3})(\d{3})", "$1.$2");
                }
                if (parts[5] == ".")
                {
                    parts[3] = Regex.Replace(parts[3], @"(\d{3})(\d{3})", "$1.$2");
                }
            } else
            {
                for (int i = 1; i<= 3; i += 2)
                {
                    parts[i] = Regex.Replace(parts[i], @"([EW]\s{0,2})(\d{1,3})(\d{2})(\d{3})$", "$1$2°$3.$4");
                    parts[i] = Regex.Replace(parts[i], @"([NS]\s{0,1})(\d{1,2})(\d{2})(\d{3})$", "$1$2°$3.$4");
                    parts[i] = Regex.Replace(parts[i], @"(\d{3})(\d{3})[\D]+", "$1.$2");
                    parts[i] = Regex.Replace(parts[i], @"(\d{3})(\d{3})$", "$1.$2");
                }
        }

            string tmpBlankString = string.Concat(Enumerable.Repeat("! ", _valueLength));
            string pad1 = "";
            string pad2 = "";
            for(int i=1;i<=3; i++)
            {
                parts[i] = Regex.Replace(Regex.Replace(parts[i], @"ï¿{1}", @" "), @"½{1}", @"");  // This is specifically to remove odd data discovered in testing
                parts[i] = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(parts[i], @"\u00c2\u00b0{1}", @"°"), @"[\s\S-[\u00c2]]{1}", @"!$&"), @"(!°!){1}", @"°"), @"(!\.!){1}", @"."), @"(!\:!){1}", @":");

            }
            int lineMiddleLength = (_valueLength * 2) - parts[1].Length - parts[3].Length;
            if(lineMiddleLength > parts[2].Length)
            {
                int padLength = (lineMiddleLength - parts[2].Length) / 4;
                pad1 = tmpBlankString.Substring(0, parts[0] == "6" ? 12 - parts[1].Length : padLength * 2);
                pad2 = tmpBlankString.Substring(0, (lineMiddleLength - parts[2].Length) - pad1.Length);
            }
            TextValue = $"{parts[1]}{pad1}{parts[2]}{pad2}{parts[3]}"; ;
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
