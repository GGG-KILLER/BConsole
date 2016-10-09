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

# License

MIT - https://gggkiller.mit-license.org/