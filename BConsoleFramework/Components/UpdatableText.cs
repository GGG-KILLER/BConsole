/*
 * Copyright © 2016 GGG KILLER <gggkiller2@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace BConsoleFramework.Components
{
    public class UpdatableTextFieldInfo
    {
        public String Name;
        public Int32 Size;
        public Object Value;
        public ConsoleColor? Color;

        public override String ToString ( )
        {
            return $"{Name} ({Size}:{Value}) - {Color}";
        }
    }

    public struct FieldInfoReference
    {
        public String Name;
    }

    public class UpdatableText
    {
        private readonly Dictionary<String, UpdatableTextFieldInfo> Fields = new Dictionary<String, UpdatableTextFieldInfo> ( );
        private readonly List<Object> Parts = new List<Object> ( );
        private readonly Point Location;

        /// <summary>
        /// Parses the text
        /// </summary>
        /// <param name="Text">The text to parse</param>
        private void Parse ( String Text )
        {
            // ABetterParser™
            var inField = false;
            var pastColon = false;
            var curField = new UpdatableTextFieldInfo
            {
                Value = "",
                Color = Console.ForegroundColor
            };
            var Size = new StringBuilder ( );
            var Name = new StringBuilder ( );
            var Part = new StringBuilder ( );

            // Parse every character individually (faster than split and foreach which were also prone to errors)
            for ( var i = 0 ; i < Text.Length ; i++ )
            {
                // Last char
                var lc = i - 1 > 0 ? Text[i - 1] : '\x0';
                // Current Char
                var cc = Text[i];

                // Test for each character
                switch ( cc )
                {
                    // Field start
                    case '{':
                        // Throw exception if we're inside a field already
                        if ( inField )
                            throw new Exception ( $"Cannot use '{{' inside a field name ({i})." );

                        // Check if last character wasn't escape sequence
                        if ( lc != '\\' )
                        {
                            // Add the part to the text if there's anything
                            if ( Part.Length > 0 )
                            {
                                Parts.Add ( Part.ToString ( ) );

                                // And clears the builder
                                Part.Clear ( );
                            }

                            // Mark the field flag
                            inField = true;
                        }
                        // Else just append the curly brace
                        else
                        {
                            // Removes the escape character as it's not intentional
                            Part.Remove ( Part.Length - 1, 1 );
                            Part.Append ( '{' );
                        }
                        break;

                    // End of a field
                    case '}':
                        var size = 0;

                        // Unmark the field flag
                        if ( !inField )
                        {
                            // And append to the text
                            Part.Append ( '}' );
                            break;
                        }

                        // If there's no name, throw an error
                        if ( Name.Length < 1 )
                            throw new Exception ( $"Name of section cannot be empty. ({i})" );

                        // Throw an error if the size of the field is invalid
                        if ( Size.Length > 1 && !Int32.TryParse ( Size.ToString ( ), out size ) )
                            throw new Exception ( $"Invalid field name ({i})" );

                        // Add the field to the list
                        curField.Name = Name.ToString ( );
                        curField.Size = size;
                        Fields.Add ( curField.Name, curField );
                        Parts.Add ( new FieldInfoReference { Name = curField.Name } );

                        // Clearing data
                        pastColon = inField = false;
                        Name.Clear ( );
                        Size.Clear ( );
                        curField = new UpdatableTextFieldInfo
                        {
                            Value = null,
                            Color = Console.ForegroundColor
                        };
                        break;

                    // Check for the field size start
                    case ':':
                        // Mark the field size flag if we're in a field
                        if ( inField )
                            pastColon = true;
                        // Else just append to the text
                        else
                            Part.Append ( ':' );
                        break;

                    // All other characters
                    default:
                        // If we're in a field, parse names and sizes
                        if ( inField )
                        {
                            if ( pastColon )
                                Size.Append ( cc );
                            else
                                Name.Append ( cc );
                        }
                        // Else just append to the part
                        else
                            Part.Append ( cc );
                        break;
                }
            }

            // Add the remaining text if there's any
            if ( Part.Length > 0 )
            {
                Parts.Add ( Part.ToString ( ) );
                Part.Clear ( );
            }
        }

        /// <summary>
        /// Creates an updatable text
        /// </summary>
        /// <param name="Text">The line pattern</param>
        public UpdatableText ( String Text )
        {
            // Parses the line
            this.Parse ( Text );
            // Saves the location the line was first inserted at
            this.Location = new Point ( Console.CursorLeft, Console.CursorTop );
        }

        /// <summary>
        /// Creates an updatable text
        /// </summary>
        /// <param name="Text">The line pattern</param>
        /// <param name="Values">The initial values to render</param>
        public UpdatableText ( String Text, IDictionary<String, Object> Values ) : this ( Text )
        {
            // Inserts the values into the fields
            foreach ( var kv in Values )
                this.Fields[kv.Key].Value = kv.Value;

            // Renders the text
            this.Render ( );
        }

        /// <summary>
        /// Updates a field from the text and re-renders it
        /// </summary>
        /// <param name="Name">The name of the field</param>
        /// <param name="Value">The value of the field</param>
        /// <param name="ReRender">If the text should be re-rendered</param>
        public void Update ( String Name, Object Value, Boolean ReRender = false )
        {
            // Break if the value contains a line break
            if ( Value.ToString ( ).IndexOf ( '\n' ) != -1 )
                throw new ArgumentException ( "The value cannot contain a line break!", nameof ( Value ) );

            // Updates the field value
            this.Fields[Name].Value = Value;

            // Re-renders the text
            if ( ReRender )
                this.Render ( );
        }

        /// <summary>
        /// Updates the field and the text
        /// </summary>
        /// <param name="Name">The name of the field</param>
        /// <param name="Value">The value of the field</param>
        /// <param name="Color">The color of the field value</param>
        /// <param name="ReRender">Wether the text should be re-rendered</param>
        public void Update ( String Name, Object Value, ConsoleColor Color, Boolean ReRender = false )
        {
            // Can't have any line breaks for the sake of rendering
            if ( Value.ToString ( ).IndexOf ( '\n' ) != -1 )
                throw new ArgumentException ( "The value cannot contain a line break!", nameof ( Value ) );

            // Updates the value and color
            this.Fields[Name].Value = Value;
            this.Fields[Name].Color = Color;

            // Re-renders the text
            if ( ReRender )
                this.Render ( );
        }

        /// <summary>
        /// Renders the text
        /// </summary>
        public void Render ( )
        {
            // Loops through all parts of the text
            using ( new TemporaryCursorMove ( Location.X, Location.Y ) )
            {
                foreach ( var Part in Parts )
                {
                    // Handles the part types
                    if ( Part is String )
                    {
                        // Writes out if it's simply a string
                        BConsole.Write ( Part.ToString ( ) );
                    }
                    else if ( Part is FieldInfoReference )
                    {
                        // Writes the value with color if it's a field
                        var fieldName = ( ( FieldInfoReference ) Part ).Name;
                        UpdatableTextFieldInfo info;

                        // If it exists, that is
                        if ( Fields.TryGetValue ( fieldName, out info ) )
                        {
                            var value = ( info.Value ?? "" ).ToString ( );
                            var pad = new String ( ' ', Math.Max ( info.Size - value.Length, 0 ) );

                            // Writes out the field with the color and value
                            BConsole.Write ( value + pad, info.Color ?? Console.ForegroundColor );
                        }
                        else
                            throw new Exception ( $"Unprovided field value {fieldName}" );
                    }
                    else
                    {
                        // Throws exception on unknown part types
                        throw new Exception ( $"Unknown part ({Part}) type: {Part.GetType ( ).Name}" );
                    }
                }

                // Writes the line terminator
                BConsole.WriteLine ( );
            }
        }
    }
}