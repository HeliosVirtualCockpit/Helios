//  Copyright 2014 Craig Courtney
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon
{
    public class FalconKeyFile
    {
        private readonly string _fileName;
        private bool _parsed;
        private readonly Dictionary<string, FalconKeyCallback> _callbacks = new Dictionary<string, FalconKeyCallback>();
        private List<FalconKeyCallback> _callbackList;
        private bool _keyFileExists = false;

        public FalconKeyFile(string keyFile)
        {
            _fileName = keyFile;
            _parsed = false;
        }

        #region Properties

        public bool KeyFileExists
        {
            get
            {
                return _keyFileExists;
            }
            set
            {
                _keyFileExists = value;
            }
        }

        public bool IsParsed
        {
            get
            {
                return _parsed;
            }
        }
        public string FileName
        {
            get { return _fileName; }
        }

        #endregion

        private void ParseKeys(string keyFile)
        {
            if (File.Exists(keyFile))
            {
                KeyFileExists = true;
                using (StreamReader reader = File.OpenText(keyFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length > 0 && line[0] != '#')
                        {
                            //string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                            string[] tokens = SplitArgument(line);

                            if (tokens.Length >= 2 && Regex.IsMatch(tokens[0].ToString(), "^[a-zA-Z0-9_]*$"))
                            {
                                string callbackName = tokens[0];
                                int isKey = int.Parse(tokens[2]);
                                if (isKey == 0)
                                {
                                    FalconKeyCallback callback = new FalconKeyCallback(callbackName)
                                    {
                                        KeyCode = ConvertString(tokens[3]),
                                        Modifiers = ConvertString(tokens[4]),
                                        ComboKeyCode = ConvertString(tokens[5]),
                                        ComboModifiers = ConvertString(tokens[6])
                                    };
                                    if (tokens.Length >= 9)
                                    {
                                        callback.Description = tokens[8];
                                    }
                                    if (!_callbacks.ContainsKey(callbackName) && callback.KeyCode > 0)
                                    {
                                        _callbacks.Add(callbackName, callback);
                                    }
                                }
                            }
                        }
                    }
                }
                
                _callbackList = new List<FalconKeyCallback>(_callbacks.Values);
                if(_callbackList.Count > 0)
                {
                    _parsed = true;
                    _callbackList.Sort();
                }
                else
                {
                    _parsed = false;
                }
            }
            else
            {
                KeyFileExists = false;
            }
        }

        public static string[] SplitArgument(String argumentString)
        {
            StringBuilder translatedArguments = new StringBuilder(argumentString).Replace("\\\"", "\r");
            bool InsideQuote = false;
            for (int i = 0; i < translatedArguments.Length; i++)
            {
                if (translatedArguments[i] == '"')
                {
                    InsideQuote = !InsideQuote;
                }
                if (translatedArguments[i] == ' ' && !InsideQuote)
                {
                    translatedArguments[i] = '\n';
                }
            }

            string[] toReturn = translatedArguments.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = RemoveMatchingQuotes(toReturn[i]);
                toReturn[i] = toReturn[i].Replace("\r", "\"");
            }
            return toReturn;
        }

        public static string RemoveMatchingQuotes(string stringToTrim)
        {
            int firstQuoteIndex = stringToTrim.IndexOf('"');
            int lastQuoteIndex = stringToTrim.LastIndexOf('"');
            while (firstQuoteIndex != lastQuoteIndex)
            {
                stringToTrim = stringToTrim.Remove(firstQuoteIndex, 1);
                stringToTrim = stringToTrim.Remove(lastQuoteIndex - 1, 1); //-1 because we've shifted the indicies left by one
                firstQuoteIndex = stringToTrim.IndexOf('"');
                lastQuoteIndex = stringToTrim.LastIndexOf('"');
            }
            return stringToTrim;
        }


        private int ConvertString(string numberString)
        {
            try
            {
                if (numberString.StartsWith("0X", StringComparison.CurrentCultureIgnoreCase))
                {
                    return int.Parse(numberString.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }
                return int.Parse(numberString);
            }
            catch (FormatException e)
            {
                ConfigManager.LogManager.LogError("Error parsing falcon key file.", e);
            }

            return 0;
        }

        public List<FalconKeyCallback> Callbacks
        {
            get
            {
                if (!_parsed)
                {
                    ParseKeys(_fileName);
                }

                return _callbackList;
            }
        }

        

        //public List<string> CallbackNames
        //{
        //    get
        //    {
        //        if (!_parsed)
        //        {
        //            ParseKeys(_fileName);
        //        }

        //        return new List<string>(_callbacks.Keys);
        //    }
        //}

        public bool HasCallback(string callbackName)
        {
            if (!_parsed)
            {
                ParseKeys(_fileName);
            } 
            
            return _callbacks.ContainsKey(callbackName);
        }

        public FalconKeyCallback this[string callbackName]
        {
            get
            {
                if (!_parsed)
                {
                    ParseKeys(_fileName);
                }

                if (_callbacks.ContainsKey(callbackName))
                {
                    return _callbacks[callbackName];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
