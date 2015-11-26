using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsInput;

namespace CodedUI.Virtusa.Utils
{
    class KeyCodes
    {
        Dictionary<string, VirtualKeyCode> keyCodes2;

        Dictionary<string, VirtualKeyCode[]> keyCodes;

        public KeyCodes() 
        {
            initKeys();
        }


        public VirtualKeyCode[] GetKey(String key)
        {
            return keyCodes[key];
        }

        private void initKeys()
        {
            keyCodes = new Dictionary<string, VirtualKeyCode[]>();
            // key code for key event "a"
            keyCodes.Add("a", new VirtualKeyCode[] { VirtualKeyCode.VK_A });
            // key code for key event "b"
            keyCodes.Add("b", new VirtualKeyCode[]  { VirtualKeyCode.VK_B });
            // key code for key event "c"
            keyCodes.Add("c", new VirtualKeyCode[]  { VirtualKeyCode.VK_C });
            // key code for key event "d"
            keyCodes.Add("d", new VirtualKeyCode[]  { VirtualKeyCode.VK_D });
            // key code for key event "e"
            keyCodes.Add("e", new VirtualKeyCode[]  { VirtualKeyCode.VK_E });
            // key code for key event "f"
            keyCodes.Add("f", new VirtualKeyCode[]  { VirtualKeyCode.VK_F });
            // key code for key event "g"
            keyCodes.Add("g", new VirtualKeyCode[]  { VirtualKeyCode.VK_G });
            // key code for key event "h"
            keyCodes.Add("h", new VirtualKeyCode[]  { VirtualKeyCode.VK_H });
            // key code for key event "i"
            keyCodes.Add("i", new VirtualKeyCode[]  { VirtualKeyCode.VK_I });
            // key code for key event "j"
            keyCodes.Add("j", new VirtualKeyCode[]  { VirtualKeyCode.VK_J });
            // key code for key event "k"
            keyCodes.Add("k", new VirtualKeyCode[]  { VirtualKeyCode.VK_K });
            // key code for key event "l"
            keyCodes.Add("l", new VirtualKeyCode[]  { VirtualKeyCode.VK_L });
            // key code for key event "m"
            keyCodes.Add("m", new VirtualKeyCode[]  { VirtualKeyCode.VK_M });
            // key code for key event "n"
            keyCodes.Add("n", new VirtualKeyCode[]  { VirtualKeyCode.VK_N });
            // key code for key event "o"
            keyCodes.Add("o", new VirtualKeyCode[]  { VirtualKeyCode.VK_O });
            // key code for key event "p"
            keyCodes.Add("p", new VirtualKeyCode[]  { VirtualKeyCode.VK_P });
            // key code for key event "q"
            keyCodes.Add("q", new VirtualKeyCode[]  { VirtualKeyCode.VK_Q });
            // key code for key event "r"
            keyCodes.Add("r", new VirtualKeyCode[]  { VirtualKeyCode.VK_R });
            keyCodes.Add("s", new VirtualKeyCode[]  { VirtualKeyCode.VK_S });
            keyCodes.Add("t", new VirtualKeyCode[]  { VirtualKeyCode.VK_T });
            keyCodes.Add("u", new VirtualKeyCode[]  { VirtualKeyCode.VK_U });
            keyCodes.Add("v", new VirtualKeyCode[]  { VirtualKeyCode.VK_V });
            keyCodes.Add("w", new VirtualKeyCode[]  { VirtualKeyCode.VK_W });
            keyCodes.Add("x", new VirtualKeyCode[]  { VirtualKeyCode.VK_X });
            keyCodes.Add("y", new VirtualKeyCode[]  { VirtualKeyCode.VK_Y });
            keyCodes.Add("z", new VirtualKeyCode[]  { VirtualKeyCode.VK_Z });
            keyCodes.Add("A", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_A });
            keyCodes.Add("B", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_B });
            keyCodes.Add("C", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_C });
            keyCodes.Add("D", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_D });
            keyCodes.Add("E", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_E });
            keyCodes.Add("F", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_F });
            keyCodes.Add("G", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_G });
            keyCodes.Add("H", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_H });
            keyCodes.Add("I", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_I });
            keyCodes.Add("J", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_J });
            keyCodes.Add("K", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_K });
            keyCodes.Add("L", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_L });
            keyCodes.Add("M", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_M });
            keyCodes.Add("N", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_N });
            keyCodes.Add("O", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_O });
            keyCodes.Add("P", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_P });
            keyCodes.Add("Q", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Q });
            keyCodes.Add("R", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_R });
            keyCodes.Add("S", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_S });
            keyCodes.Add("T", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_T });
            keyCodes.Add("U", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_U });
            keyCodes.Add("V", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_V });
            keyCodes.Add("W", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_W });
            keyCodes.Add("X", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_X });
            keyCodes.Add("Y", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Y });
            keyCodes.Add("Z", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Z });
            keyCodes.Add("`", new VirtualKeyCode[]  { VirtualKeyCode.OEM_TILDE });
            keyCodes.Add("0", new VirtualKeyCode[]  { VirtualKeyCode.VK_0 });
            keyCodes.Add("1", new VirtualKeyCode[]  { VirtualKeyCode.VK_1 });
            keyCodes.Add("2", new VirtualKeyCode[]  { VirtualKeyCode.VK_2 });
            keyCodes.Add("3", new VirtualKeyCode[]  { VirtualKeyCode.VK_3 });
            keyCodes.Add("4", new VirtualKeyCode[]  { VirtualKeyCode.VK_4 });
            keyCodes.Add("5", new VirtualKeyCode[]  { VirtualKeyCode.VK_5 });
            keyCodes.Add("6", new VirtualKeyCode[]  { VirtualKeyCode.VK_6 });
            keyCodes.Add("7", new VirtualKeyCode[]  { VirtualKeyCode.VK_7 });
            keyCodes.Add("8", new VirtualKeyCode[]  { VirtualKeyCode.VK_8 });
            keyCodes.Add("9", new VirtualKeyCode[]  { VirtualKeyCode.VK_9 });
            keyCodes.Add("-", new VirtualKeyCode[]  { VirtualKeyCode.OEM_MINUS });
            keyCodes.Add("=", new VirtualKeyCode[]  { VirtualKeyCode.OEM_PLUS });
            keyCodes.Add("~", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_TILDE });
            keyCodes.Add("!", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_1 });
            keyCodes.Add("@", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_2 });
            keyCodes.Add("#", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_3 });
            keyCodes.Add("$", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_4 });
            keyCodes.Add("%", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_5 });
            keyCodes.Add("^", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_6 });
            keyCodes.Add("&", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_7 });
            keyCodes.Add("*", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_8 });
            keyCodes.Add("(", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_9 });
            keyCodes.Add(")", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_0 });
            keyCodes.Add("_", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_MINUS });
            keyCodes.Add("+", new VirtualKeyCode[]  { VirtualKeyCode.OEM_PLUS });
            keyCodes.Add("\t", new VirtualKeyCode[]  { VirtualKeyCode.TAB });
            keyCodes.Add("\n", new VirtualKeyCode[]  { VirtualKeyCode.ACCEPT });
            keyCodes.Add("[", new VirtualKeyCode[]  { VirtualKeyCode.OEM_OPEN_BRACKETS });
            keyCodes.Add("]", new VirtualKeyCode[]  { VirtualKeyCode.OEM_CLOSE_BRACKETS });
            keyCodes.Add("\\", new VirtualKeyCode[]  { VirtualKeyCode.OEM_BACK_SLASH });
            keyCodes.Add("{", new VirtualKeyCode[]  {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_OPEN_BRACKETS});
            keyCodes.Add("}", new VirtualKeyCode[] { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_CLOSE_BRACKETS });
            keyCodes.Add("|", new VirtualKeyCode[] { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_BACK_SLASH });
            keyCodes.Add(";", new VirtualKeyCode[]  { VirtualKeyCode.OEM_SEMICOLON });
            keyCodes.Add(":", new VirtualKeyCode[] { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_SEMICOLON });
            keyCodes.Add("'", new VirtualKeyCode[]  { VirtualKeyCode.OEM_QUOTES });
            keyCodes.Add("\"", new VirtualKeyCode[]  { VirtualKeyCode.OEM_BACK_SLASH });
            keyCodes.Add(",", new VirtualKeyCode[]  { VirtualKeyCode.OEM_COMMA });
            keyCodes.Add("<", new VirtualKeyCode[] { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_COMMA });
            keyCodes.Add(".", new VirtualKeyCode[]  { VirtualKeyCode.OEM_PERIOD });
            keyCodes.Add(">", new VirtualKeyCode[] { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_PERIOD });
            keyCodes.Add("/", new VirtualKeyCode[]  { VirtualKeyCode.OEM_QUESTION });
            keyCodes.Add("?", new VirtualKeyCode[] { VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_QUESTION });
            keyCodes.Add(" ", new VirtualKeyCode[]  { VirtualKeyCode.SPACE });
            keyCodes.Add("alt", new VirtualKeyCode[]  { VirtualKeyCode.MENU });
            keyCodes.Add("ctrl", new VirtualKeyCode[]  { VirtualKeyCode.CONTROL });
            keyCodes.Add("esc", new VirtualKeyCode[]  { VirtualKeyCode.ESCAPE });
            keyCodes.Add("down", new VirtualKeyCode[]  { VirtualKeyCode.DOWN });
            keyCodes.Add("up", new VirtualKeyCode[]  { VirtualKeyCode.UP });
            keyCodes.Add("left", new VirtualKeyCode[]  { VirtualKeyCode.LEFT });
            keyCodes.Add("right", new VirtualKeyCode[]  { VirtualKeyCode.RIGHT });
            keyCodes.Add("F1", new VirtualKeyCode[] { VirtualKeyCode.F1 });
            keyCodes.Add("F2", new VirtualKeyCode[] { VirtualKeyCode.F2 });
            keyCodes.Add("F3", new VirtualKeyCode[] { VirtualKeyCode.F3 });
            keyCodes.Add("F4", new VirtualKeyCode[] { VirtualKeyCode.F4 });
            keyCodes.Add("F5", new VirtualKeyCode[] { VirtualKeyCode.F5 });
            keyCodes.Add("F6", new VirtualKeyCode[] { VirtualKeyCode.F6 });
            keyCodes.Add("F7", new VirtualKeyCode[] { VirtualKeyCode.F7 });
            keyCodes.Add("F8", new VirtualKeyCode[] { VirtualKeyCode.F8 });
            keyCodes.Add("F9", new VirtualKeyCode[] { VirtualKeyCode.F9 });
            keyCodes.Add("F10", new VirtualKeyCode[] { VirtualKeyCode.F10 });
            keyCodes.Add("F11", new VirtualKeyCode[] { VirtualKeyCode.F11 });
            keyCodes.Add("F12", new VirtualKeyCode[] { VirtualKeyCode.F12 });
            //keyCodes.Add("alt+F4", new VirtualKeyCode[]  { VirtualKeyCode.VK_ALT, VirtualKeyCode.VK_F4 });
            //keyCodes.Add("alt+\t", new VirtualKeyCode[]  { VirtualKeyCode.VK_ALT, VirtualKeyCode.VK_TAB });
            keyCodes.Add("insert", new VirtualKeyCode[]  { VirtualKeyCode.INSERT });
            keyCodes.Add("home", new VirtualKeyCode[]  { VirtualKeyCode.HOME });
            //keyCodes.Add("pageup", new VirtualKeyCode[]  { VirtualKeyCode.page });
            keyCodes.Add("backspace", new VirtualKeyCode[]  { VirtualKeyCode.CLEAR });
            keyCodes.Add("delete", new VirtualKeyCode[]  { VirtualKeyCode.DELETE });
            keyCodes.Add("end", new VirtualKeyCode[]  { VirtualKeyCode.END });
            //keyCodes.Add("pagedown", new VirtualKeyCode[]  { VirtualKeyCode.page });
            //keyCodes.Add("shift+\t", new VirtualKeyCode[]  { VirtualKeyCode.SHIFT, VirtualKeyCode.VK_TAB });
            //keyCodes.Add("ctrl+o", new VirtualKeyCode[]  { VirtualKeyCode.VK_CONTROL, VirtualKeyCode.VK_O });
        }

        //private void initKeys2()
        //{
        //    keyCodes = new Dictionary<string, VirtualKeyCode>();

        //    // key code for key event "a"
        //    keyCodes.Add("a",  VirtualKeyCode.VK_A );
        //    // key code for key event "b"
        //    keyCodes.Add("b",  VirtualKeyCode.VK_B );
        //    // key code for key event "c"
        //    keyCodes.Add("c",  VirtualKeyCode.VK_C );
        //    // key code for key event "d"
        //    keyCodes.Add("d",  VirtualKeyCode.VK_D );
        //    // key code for key event "e"
        //    keyCodes.Add("e",  VirtualKeyCode.VK_E );
        //    // key code for key event "f"
        //    keyCodes.Add("f",  VirtualKeyCode.VK_F );
        //    // key code for key event "g"
        //    keyCodes.Add("g",  VirtualKeyCode.VK_G );
        //    // key code for key event "h"
        //    keyCodes.Add("h",  VirtualKeyCode.VK_H );
        //    // key code for key event "i"
        //    keyCodes.Add("i",  VirtualKeyCode.VK_I );
        //    // key code for key event "j"
        //    keyCodes.Add("j",  VirtualKeyCode.VK_J );
        //    // key code for key event "k"
        //    keyCodes.Add("k",  VirtualKeyCode.VK_K );
        //    // key code for key event "l"
        //    keyCodes.Add("l",  VirtualKeyCode.VK_L );
        //    // key code for key event "m"
        //    keyCodes.Add("m",  VirtualKeyCode.VK_M );
        //    // key code for key event "n"
        //    keyCodes.Add("n",  VirtualKeyCode.VK_N );
        //    // key code for key event "o"
        //    keyCodes.Add("o",  VirtualKeyCode.VK_O );
        //    // key code for key event "p"
        //    keyCodes.Add("p",  VirtualKeyCode.VK_P );
        //    // key code for key event "q"
        //    keyCodes.Add("q",  VirtualKeyCode.VK_Q );
        //    // key code for key event "r"
        //    keyCodes.Add("r",  VirtualKeyCode.VK_R );
        //    // key code for key event "s"
        //    keyCodes.Add("s",  VirtualKeyCode.VK_S );
        //    // key code for key event "t"
        //    keyCodes.Add("t",  VirtualKeyCode.VK_T );
        //    // key code for key event "u"
        //    keyCodes.Add("u",  VirtualKeyCode.VK_U );
        //    // key code for key event "v"
        //    keyCodes.Add("v",  VirtualKeyCode.VK_V );
        //    // key code for key event "w"
        //    keyCodes.Add("w",  VirtualKeyCode.VK_W );
        //    // key code for key event "x"
        //    keyCodes.Add("x",  VirtualKeyCode.VK_X );
        //    // key code for key event "y"
        //    keyCodes.Add("y",  VirtualKeyCode.VK_Y );
        //    // key code for key event "z"
        //    keyCodes.Add("z",  VirtualKeyCode.VK_Z );

        //    //keyCodes.Add("A",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_A );
        //    //keyCodes.Add("B",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_B );
        //    //keyCodes.Add("C",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_C );
        //    //keyCodes.Add("D",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_D );
        //    //keyCodes.Add("E",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_E );
        //    //keyCodes.Add("F",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_F );
        //    //keyCodes.Add("G",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_G );
        //    //keyCodes.Add("H",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_H );
        //    //keyCodes.Add("I",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_I );
        //    //keyCodes.Add("J",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_J );
        //    //keyCodes.Add("K",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_K );
        //    //keyCodes.Add("L",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_L );
        //    //keyCodes.Add("M",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_M );
        //    //keyCodes.Add("N",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_N );
        //    //keyCodes.Add("O",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_O );
        //    //keyCodes.Add("P",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_P );
        //    //keyCodes.Add("Q",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Q );
        //    //keyCodes.Add("R",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_R );
        //    //keyCodes.Add("S",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_S );
        //    //keyCodes.Add("T",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_T );
        //    //keyCodes.Add("U",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_U );
        //    //keyCodes.Add("V",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_V );
        //    //keyCodes.Add("W",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_W );
        //    //keyCodes.Add("X",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_X );
        //    //keyCodes.Add("Y",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Y );
        //    //keyCodes.Add("Z",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Z );
        //    //keyCodes.Add("`",  VirtualKeyCode.VK_BACK_QUOTE );
        //    keyCodes.Add("0",  VirtualKeyCode.VK_0 );
        //    keyCodes.Add("1",  VirtualKeyCode.VK_1 );
        //    keyCodes.Add("2",  VirtualKeyCode.VK_2 );
        //    keyCodes.Add("3",  VirtualKeyCode.VK_3 );
        //    keyCodes.Add("4",  VirtualKeyCode.VK_4 );
        //    keyCodes.Add("5",  VirtualKeyCode.VK_5 );
        //    keyCodes.Add("6",  VirtualKeyCode.VK_6 );
        //    keyCodes.Add("7",  VirtualKeyCode.VK_7 );
        //    keyCodes.Add("8",  VirtualKeyCode.VK_8 );
        //    keyCodes.Add("9",  VirtualKeyCode.VK_9 );
        //    keyCodes.Add("-",  VirtualKeyCode.OEM_MINUS );
        //    //keyCodes.Add("=",  VirtualKeyCode.VK_EQUALS );
        //    //keyCodes.Add("~",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_BACK_QUOTE );
        //    //keyCodes.Add("!",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_1 );
        //    //keyCodes.Add("@",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_2 );
        //    ///keyCodes.Add("#",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_3 );
        //    //keyCodes.Add("$",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_4 );
        //    //keyCodes.Add("%",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_5 );
        //   //keyCodes.Add("^",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_6 );
        //    //keyCodes.Add("&",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_7 );
        //    //keyCodes.Add("*",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_8 );
        //    //keyCodes.Add("(",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_9 );
        //    //keyCodes.Add(")",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_0 );
        //    //keyCodes.Add("_",  VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_MINUS );
        //    //keyCodes.Add("+",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_EQUALS );
        //    keyCodes.Add("\t",  VirtualKeyCode.TAB );
        //    keyCodes.Add("\n",  VirtualKeyCode.ACCEPT);
        //    //keyCodes.Add("[",  VirtualKeyCode. );
        //    //keyCodes.Add("]",  VirtualKeyCode.VK_CLOSE_BRACKET );
        //    //keyCodes.Add("\\",  VirtualKeyCode.VK_BACK_SLASH );
        //   // keyCodes.Add("{", VirtualKeyCode.SHIFT, VirtualKeyCode.VK_OPEN_BRACKET);
        //   //keyCodes.Add("}", VirtualKeyCode.SHIFT, VirtualKeyCode.VK_CLOSE_BRACKET);
        //    //keyCodes.Add("|",  VirtualKeyCode.SHIFT, VirtualKeyCode.VK_BACK_SLASH );
        //    //keyCodes.Add(";",  VirtualKeyCode. );
        //   // keyCodes.Add(":",  VirtualKeyCode.VK_COLON );
        //   // keyCodes.Add("'",  VirtualKeyCode.VK_QUOTE );
        //   // keyCodes.Add("\"",  VirtualKeyCode.VK_QUOTEDBL );
        //    keyCodes.Add(",",  VirtualKeyCode.OEM_COMMA );
        //   // keyCodes.Add("<",  VirtualKeyCode.SHIFT, VirtualKeyCode. );
        //    keyCodes.Add(".",  VirtualKeyCode.OEM_PERIOD );
        //    //keyCodes.Add(">",  VirtualKeyCode.SHIFT, VirtualKeyCode. );
        //    keyCodes.Add("/",  VirtualKeyCode.DIVIDE );
        //    //keyCodes.Add("?",  VirtualKeyCode.SHIFT, VirtualKeyCode. );
        //    keyCodes.Add(" ",  VirtualKeyCode.SPACE );
        //    keyCodes.Add("shift", VirtualKeyCode.SHIFT);
        //    keyCodes.Add("alt",  VirtualKeyCode.MENU );
        //    keyCodes.Add("ctrl",  VirtualKeyCode.CONTROL );
        //    keyCodes.Add("esc",  VirtualKeyCode.ESCAPE );
        //    keyCodes.Add("down",  VirtualKeyCode.DOWN );
        //    keyCodes.Add("up",  VirtualKeyCode.UP );
        //    keyCodes.Add("left",  VirtualKeyCode.LEFT );
        //    keyCodes.Add("right",  VirtualKeyCode.RIGHT );
        //    keyCodes.Add("F1",  VirtualKeyCode.F1 );
        //    keyCodes.Add("F2",  VirtualKeyCode.F2 );
        //    keyCodes.Add("F3",  VirtualKeyCode.F3 );
        //    keyCodes.Add("F4",  VirtualKeyCode.F4 );
        //    keyCodes.Add("F5",  VirtualKeyCode.F5 );
        //    keyCodes.Add("F6",  VirtualKeyCode.F6 );
        //    keyCodes.Add("F7",  VirtualKeyCode.F7 );
        //    keyCodes.Add("F8",  VirtualKeyCode.F8 );
        //    keyCodes.Add("F9",  VirtualKeyCode.F9 );
        //    keyCodes.Add("F10",  VirtualKeyCode.F10 );
        //    keyCodes.Add("F11",  VirtualKeyCode.F11 );
        //    keyCodes.Add("F12",  VirtualKeyCode.F12 );

        //}
    }
}
