﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsWpfControls
{

    /// <summary>
    /// This interfaces is used to parse a string to any object. 
    /// </summary>
    public interface IParseStringToObject
    {
        /// <summary>
        /// Parses the given input to an object of the given type.
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <param name="result">The object if successful, otherwise null</param>
        /// <param name="culture">The culture which should be used to parse. This parameter is optional</param>
        /// <param name="stringFormat">The string format to apply. This parameter is optional</param>
        /// <param name="targetType">the <see cref="Type"/> to which the input should be converted to. This parameter is optional</param>
        /// <returns><see langword="true"/> if converting successful, otherwise <see langword="false"/></returns>
        bool TryCreateObjectFromString(string input, out object result, CultureInfo culture = null, string stringFormat = null, Type targetType = null);
    }
}