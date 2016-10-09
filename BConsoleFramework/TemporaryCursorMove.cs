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

namespace BConsoleFramework
{
    /// <summary>
    /// Moves the cursor temporarely to another position
    /// then restores the position once disposed
    /// </summary>
    internal class TemporaryCursorMove : IDisposable
    {
        private Int32 Left, Top;

        /// <summary>
        /// Moves the cursor temporarely to another position
        /// then restores the position once disposed
        /// </summary>
        /// <param name="Left">The Left value to move to</param>
        /// <param name="Top">The Top value to move to</param>
        public TemporaryCursorMove ( Int32 Left, Int32 Top )
        {
            // Saves the current position
            this.Left = BConsole.CursorLeft;
            this.Top = BConsole.CursorTop;

            // Moves the cursor to the provided position if we are on a console
            BConsole.SetCursorPosition ( Left, Top );
        }

        public void Dispose ( )
        {
            // Moves the cursor back to where it was
            BConsole.SetCursorPosition ( Left, Top );
            GC.SuppressFinalize ( this );
        }
    }
}