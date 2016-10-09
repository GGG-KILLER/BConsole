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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BConsoleFramework.Components
{
    public class TextBox
    {
        private Int32 Width;
        private ConsoleColor Color;
        private Point Location;
        private Point Cursor;
        private StringBuilder Str;

        public TextBox ( Int32 Width, ConsoleColor Color )
        {
            this.Width = Width;
            this.Color = Color;
            Location = new Point ( Console.CursorLeft, Console.CursorTop );
            Str = new StringBuilder ( );

            Cursor = new Point ( 0, Location.Y + 1 );
        }

        private void DrawBox ( )
        {
            var c = Console.ForegroundColor;

            Console.ForegroundColor = Color;
            Console.SetCursorPosition ( Location.X, Location.Y );

            #region Upper Line

            Console.Write ( BConsole.BoxChars.TopLeft );
            Console.Write ( new String ( BConsole.BoxChars.Vertical, Width - 2 ) );
            Console.WriteLine ( BConsole.BoxChars.TopRight );

            #endregion Upper Line

            #region Middle Line

            Console.Write ( BConsole.BoxChars.Horizontal );
            Console.Write ( new String ( ' ', Width - 2 ) );
            Console.WriteLine ( BConsole.BoxChars.Horizontal );

            #endregion Middle Line

            #region Bottom Line

            Console.Write ( BConsole.BoxChars.BottomLeft );
            Console.Write ( new String ( BConsole.BoxChars.Vertical, Width - 2 ) );
            Console.WriteLine ( BConsole.BoxChars.BottomRight );

            #endregion Bottom Line

            Console.SetCursorPosition ( Cursor.X + 1, Cursor.Y );
        }

        public String ReadLine ( )
        {
            DrawBox ( );

            ConsoleKeyInfo Key;
            do
            {
                Key = Console.ReadKey ( true );
                ProcessKey ( Key );
                DrawText ( );
            }
            while ( Key.Key != ConsoleKey.Enter );

            Console.SetCursorPosition ( Location.X, Location.Y + 3 );

            return Str.ToString ( );
        }

        private void DrawText ( )
        {
            Console.SetCursorPosition ( 1, Cursor.Y );
            Console.Write ( Str.ToString ( ) );
            Console.Write ( new String ( ' ', Width - Str.Length - 2 ) );
            Console.SetCursorPosition ( Cursor.X + 1, Cursor.Y );
        }

        private static Boolean TextKey ( ConsoleKeyInfo Key ) => Key.KeyChar != 0x0 && Key.KeyChar != 0x1B;

        private void ProcessKey ( ConsoleKeyInfo Key )
        {
            switch ( Key.Key )
            {
                case ConsoleKey.Backspace:
                    if ( Cursor.X > 0 )
                    {
                        Str.Remove ( Cursor.X - 1, 1 );
                        Cursor.X--;
                    }
                    break;

                // Ctrl+V parser
                case ConsoleKey.V:
                    ProcessClipboard ( Key );
                    Cursor.X = Str.Length;
                    break;

                case ConsoleKey.Delete:
                    if ( Console.CursorLeft - 1 < Str.Length )
                        Str.Remove ( Cursor.X, 1 );
                    break;

                case ConsoleKey.Home:
                    Cursor.X = 0;
                    break;

                case ConsoleKey.End:
                    Cursor.X = Str.Length;
                    break;

                case ConsoleKey.LeftArrow:
                    if ( Cursor.X > 0 )
                        Cursor.X--;
                    break;

                case ConsoleKey.RightArrow:
                    if ( Cursor.X < Str.Length )
                        Cursor.X++;
                    break;

                default:
                    if ( Str.Length < ( Width - 2 ) && TextKey ( Key ) )
                    {
                        Cursor.X++;

                        Str.Insert ( Cursor.X - 1, Key.KeyChar );

                        BConsole.TL ( ( ( Int32 ) Key.KeyChar ).ToString ( "X2" ) );
                    }
                    break;
            }
        }

        private void ProcessClipboard ( ConsoleKeyInfo Key )
        {
            if ( Key.Modifiers == ConsoleModifiers.Control && Clipboard.ContainsText ( ) )
            {
                //*
                var clip = Clipboard.GetText ( );
                clip = clip.Length > Width - 2 ? clip.Substring ( 0, Str.Length - Width - 2 ) : clip;
                Str.Insert ( Cursor.X, clip );
                //*/
            }
            else if ( Str.Length < Width - 2 )
                Str.Insert ( Cursor.X, Key.KeyChar );
        }
    }
}