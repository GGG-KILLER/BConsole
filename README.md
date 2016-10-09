# BConsole
BConsole is a small framework that I created for console writing and reading, it contains useful functions for those who make console-based programs.

# Features
- Safe cursor repositioning (no exceptions thrown when output is being redirected to a file)
- Safe window resizing (no exceptions thrown when output is being redirected to a file or no console window exists)
- Write multiple lines with a single function (easy one, I know, but gets tiring re-writing the loop many times and you can leave this to us for better organization)
- Line re-writing (write over a line with ease)
- Delete only a range of lines (overwrite a number of lines with \x0)
- UpdatableText class (write a pattern and then only update the parts of the text that actually change along the execution)
- Write a line or string with a certain color then return to old one promptly automatically
- Move cursor to a certain location then write a line or string then return to the previous location automatically
- ASCII textbox for nostalgic feeling (optional, use at your own risk)

# Licence

Copyright © 2016 GGG KILLER <gggkiller2@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.