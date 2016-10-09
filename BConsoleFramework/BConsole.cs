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
using BConsoleFramework.Components;

namespace BConsoleFramework
{
    public static class BConsole
    {
        /// <summary>
        /// Defines wether we're on a console window or not
        /// (Important for using cursor move operations or color changes)
        /// </summary>
        public static readonly Boolean HasConsole = true;

        /// <summary>
        /// Start X on Console
        /// </summary>
        public static readonly Int32 X0;

        /// <summary>
        /// Start Y on Console
        /// </summary>
        public static readonly Int32 Y0;

        /// <summary>
        /// Gets or sets the width of the console window if there's one
        /// </summary>
        public static Int32 WindowWidth
        {
            get
            {
                return Utils.TryReturn ( ( ) => Console.WindowWidth, 0 );
            }
            set
            {
                if ( HasConsole )
                    Console.WindowWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the console window
        /// if we are on a console window.
        /// </summary>
        public static Int32 WindowHeight
        {
            get
            {
                return Utils.TryReturn ( ( ) => Console.WindowHeight, 0 );
            }
            set
            {
                if ( HasConsole )
                    Console.WindowHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the column position of the cursor
        /// within the buffer area if we're on a console window.
        /// </summary>
        public static Int32 CursorLeft
        {
            get
            {
                return Utils.TryReturn ( ( ) => Console.CursorLeft, 0 );
            }
            set
            {
                if ( HasConsole )
                    Console.CursorLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the row position of the cursor
        /// within the buffer area if we're on a console window.
        /// </summary>
        public static Int32 CursorTop
        {
            get
            {
                return Utils.TryReturn ( ( ) => Console.CursorTop, 0 );
            }
            set
            {
                if ( HasConsole )
                    Console.CursorTop = value;
            }
        }

        /// <summary>
        /// Setups base variables and gathers information about
        /// the current environment
        /// </summary>
        static BConsole ( )
        {
            try
            {
                using ( new TemporaryCursorMove ( 0, 0 ) )
                {
                    var a = Console.BufferHeight;
                }
                HasConsole = true;
            }
            catch ( Exception )
            {
                HasConsole = false;
            }

            X0 = CursorTop;
            Y0 = CursorLeft;
        }

        /// <summary>
        /// The characters to build a box
        /// </summary>
        internal struct BoxChars
        {
            public const Char TopLeft = '╔';
            public const Char TopRight = '╗';
            public const Char BottomLeft = '╚';
            public const Char BottomRight = '╝';
            public const Char Vertical = '═';
            public const Char Horizontal = '║';
        };

        /// <summary>
        /// Clears all lines starting on the provided index
        /// </summary>
        /// <param name="StartLine">The line where to start from</param>
        public static void ClearLines ( Int32 StartLine )
        {
            var Top = Console.CursorTop;
            ClearLines ( StartLine, Top - StartLine + 1 );
        }

        /// <summary>
        /// Writes all lines passed to it
        /// </summary>
        /// <param name="Lines">Lines to write</param>
        public static void WriteLines ( IEnumerable<String> Lines )
        {
            foreach ( var Line in Lines )
                WriteLine ( Line );
        }

        /// <summary>
        /// Clears a number of lines starting on the provided index
        /// </summary>
        /// <param name="StartLine">The line where to start from</param>
        /// <param name="Count">The amount of lines to clear</param>
        public static void ClearLines ( Int32 StartLine, Int32 Count )
        {
            // Gets the last line Y
            var EndLine = StartLine + Count + 1;

            using ( new TemporaryCursorMove ( 0, StartLine ) )
            {
                // Loops through each line until the last line
                for ( var Line = StartLine ; Line < EndLine ; Line++ )
                {
                    // Moves the cursor to the Y
                    SetCursorPosition ( 0, Line );

                    // Fills the line with empty characters
                    WriteLine ( new String ( '\0', WindowWidth ) );
                }
            }
        }

        /// <summary>
        /// Re-Writes the text on the current line by the offset
        /// Re-writes from the start if no offset is provided
        /// </summary>
        /// <param name="Value">The value to write</param>
        /// <param name="Offset">The offset to write from (default = 0)</param>
        public static void ReWrite ( Object Value, Int32 Offset = 0 )
        {
            // Saves the current cursor position
            var Top = CursorTop;

            using ( new TemporaryCursorMove ( Offset, Top ) )
            {
                // Clears out the line starting on the offset
                Write ( new String ( '\0', WindowWidth ) );

                // Moves the cursor back to the offset
                SetCursorPosition ( Offset, Top );
                // Writes out the value
                Write ( Value );
            }
        }

        /// <summary>
        /// Re-Writes a line
        /// Re-writes the last line if no offset is provided
        /// The offset is based on the current line
        /// An offset of 1 will rewrite the last line, 2 will rewrite the line before the last, etc.
        /// </summary>
        /// <param name="Value">The value to write</param>
        /// <param name="Offset">The offset to write at</param>
        public static void ReWriteLine ( Object Value, Int32 Offset = 1 )
        {
            // Gets the current Y
            var Top = CursorTop;

            // Moves to the start of the line
            using ( new TemporaryCursorMove ( 0, Top - Offset ) )
            {
                // Clears out the line
                Write ( new String ( ' ', WindowWidth ) );

                // Moves the cursor back to the start of the line
                SetCursorPosition ( 0, Top - Offset );
                // Writes the line with the value
                WriteLine ( Value );
            }
        }

        /// <summary>
        /// Re-writes a line with a certain color
        /// Re-writes the last line if no offset is provided
        /// The offset is based on the current line
        /// An offset of 1 will rewrite the last line, 2 will rewrite the line before the last, etc.
        /// </summary>
        /// <param name="Value">The value to write</param>
        /// <param name="Color">The color to write with</param>
        /// <param name="Offset">The offset to write from</param>
        public static void ReWriteLine ( Object Value, ConsoleColor Color, Int32 Offset = 1 )
        {
            using ( new TemporaryForegroundColorChange ( Color ) )
                ReWriteLine ( Value, Offset );
        }

        /// <summary>
        /// Re-writes the line at the specified index
        /// </summary>
        /// <param name="Line">The index where to write at</param>
        /// <param name="Value">The value to write</param>
        public static void ReWriteLine ( Int32 Line, Object Value )
        {
            // Moves the cursor temporarely
            using ( new TemporaryCursorMove ( 0, Line ) )
            {
                // Fills the line with blanks
                Write ( new String ( '\0', WindowWidth ) );

                // Moves the cursor back to the start of the line
                SetCursorPosition ( 0, Line );
                // Writes out the line
                WriteLine ( Value );
            }
        }

        /// <summary>
        /// Re-writes the line at the specified index with the provided color
        /// </summary>
        /// <param name="Line">The index where to write at</param>
        /// <param name="Value">The value to write</param>
        /// <param name="Color">The color to write with</param>
        public static void ReWriteLine ( Int32 Line, Object Value, ConsoleColor Color )
        {
            using ( new TemporaryForegroundColorChange ( Color ) )
                ReWriteLine ( Line, Value );
        }

        /// <summary>
        /// Writes a value to the output
        /// </summary>
        /// <param name="value">The value to write</param>
        public static void Write ( Object value ) => Console.Write ( value );

        /// <summary>
        /// Writes a value to the output with a certain color then resets the color
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="Color">The color to write with</param>
        public static void Write ( Object value, ConsoleColor Color )
        {
            using ( new TemporaryForegroundColorChange ( Color ) )
                Write ( value );
        }

        /// <summary>
        /// Writes a value to the specified point
        /// </summary>
        /// <param name="Value">Value to write</param>
        /// <param name="Left">Left distance</param>
        /// <param name="Top">Top distance</param>
        public static void Write ( Object Value, Int32 Left, Int32 Top )
        {
            using ( new TemporaryCursorMove ( Left, Top ) )
                Console.Write ( Value );
        }

        /// <summary>
        /// Writes to a specified location
        /// </summary>
        /// <param name="Value">Value to write</param>
        /// <param name="Location">Location to write to</param>
        public static void Write ( Object Value, Point Location )
        {
            Write ( Value, Location.X, Location.Y );
        }

        /// <summary>
        /// Writes to a specified location with the provided color
        /// </summary>
        /// <param name="value">Value to write to</param>
        /// <param name="Left">Left distance</param>
        /// <param name="Top">Top distance</param>
        /// <param name="Color">Color to write with</param>
        public static void Write ( Object value, Int32 Left, Int32 Top, ConsoleColor Color )
        {
            using ( new TemporaryForegroundColorChange ( Color ) )
                Write ( value, Left, Top );
        }

        /// <summary>
        /// Writes to a specified location with a provided color
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="Location">The location to write to</param>
        /// <param name="Color">The color to write with</param>
        public static void Write ( Object value, Point Location, ConsoleColor Color )
        {
            using ( new TemporaryForegroundColorChange ( Color ) )
                Write ( value, Location );
        }

        /// <summary>
        /// Writes a line terminator to the output
        /// </summary>
        public static void WriteLine ( ) => Console.WriteLine ( );

        /// <summary>
        /// Writes a line to the output
        /// </summary>
        /// <param name="value">The value to write</param>
        public static void WriteLine ( Object value ) => Console.WriteLine ( value );

        /// <summary>
        /// Writes a line with a certain color to the output
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="Color">The color to write with</param>
        public static void WriteLine ( Object value, ConsoleColor Color )
        {
            using ( new TemporaryForegroundColorChange ( Color ) )
                WriteLine ( value );
        }

        /// <summary>
        /// Draws a little textbox
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static String UseTextBox ( Int32 Width, ConsoleColor Color = ConsoleColor.White )
        {
            return new TextBox ( Width, Color )
                .ReadLine ( );
        }

        /// <summary>
        /// Creates a new updatable text
        /// </summary>
        /// <param name="Text">The text pattern</param>
        /// <param name="Values">The values to use on rendering</param>
        /// <returns></returns>
        public static UpdatableText CreateText ( String Text, IDictionary<String, Object> Values )
        {
            return new UpdatableText ( Text, Values );
        }

        /// <summary>
        /// Moves the cursor on the console only if we're on a console
        /// (Avoids invalid handle exceptions)
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public static void SetCursorPosition ( Int32 X, Int32 Y )
        {
            if ( HasConsole )
                Console.SetCursorPosition ( X, Y );
        }

        private static Int32 Y;

        public static void TL ( Object a )
        {
            var s = a.ToString ( ); var l = CursorLeft; var t = CursorTop;

            SetCursorPosition ( WindowWidth - Math.Max ( s.Length, 1 ), Y0 + Y % 20 );
            Write ( new string ( ' ', s.Length ) );
            SetCursorPosition ( WindowWidth - Math.Max ( s.Length, 1 ), Y0+ Y++ % 20 );
            Write ( s );
            SetCursorPosition ( l, t );
        }
    }
}