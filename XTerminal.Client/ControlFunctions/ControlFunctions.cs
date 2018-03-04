﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.ControlFunctions
{
    #region 弃用注释，请不要相信这里所说的，没删掉是因为要做一个记录

    ///     由01/11开头，后面跟一个Fe，Fe可以看作成命令类型，7位编码的ASCII码和8位编码的ASCII码使用的Fe码的范围不同，但是每个Fe所表示的意义都是相同的
    ///     对于7位编码的ASCII码：
    ///         1.Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位编码的ASCII码：
    ///         1.Fe的范围在08/00（128） - 09/15（159）之间
    /// 

    #endregion

    /// <summary>
    /// 根据Control Functions for Coded Character Sets Ecma-048.pdf标准，
    /// 这是一个ECMA定义的标准，所有的终端模拟器都会实现这个标准
    /// 参考：
    ///     Dependencies/Control Functions for Coded Character Sets Ecma-048.pdf
    ///     
    /// 控制序列：
    ///     对于7位编码的ascii码：
    ///         由两个字符开头。第一个字符是01/11，第二个是Fe。Fe可以看作成命令类型，Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位编码的ascii码：
    ///         由一个字符开头。范围在08/00 - 09/15之间
    /// 
    /// 完整的控制序列格式是：
    ///     对于7位编码的ascii码：ControlFunction Fe [ [Fe参数] [Fe结束符] ]
    ///     对于8位编码的ascii码：ControlFunction [ [参数] [结束符] ]
    /// 
    /// 一般情况下，使用7位ascii编码的主机返回的ControlFunctions和Fe百分之九十都是ESC和CSI，这里解释一下CSI的含义，这个解释同样可以用作于8位ascii编码的主机
    /// 
    /// 一些Fe的功能说明：
    ///     CSI：
    ///         名称：控制序列
    ///         格式：CSI P..P I..I F
    ///         参数说明：
    ///             CSI：控制序列字符，在7位编码中，由01/11 05/11两个字符组成；在8位编码中，由09/11单个字符组成
    ///             P..P：参数字符串，由03/00（48） - 03/15（63）之间的字符组成
    ///             I..I：中间字符串，由02/00（32） - 02/15（47）之间的字符组成，后面会跟一个字符串终结字符（F）
    ///             F：结束字符，由04/00（64） - 07/14（126）之间的某个字符表示。07/00 - 07/14之间的字符也是结束符，但是这是留给厂商做实验使用的。注意，带有中间字符串和不带有中间字符串的结束符的含义不一样
    /// </summary>
    public static class ControlFunctions
    {
        /// <summary>
        /// ControlFunctionCode -> ControlFunctionParser
        /// </summary>
        private static Dictionary<byte, ControlFunctionParser> ControlFunctionMap = new Dictionary<byte, ControlFunctionParser>()
        {
            { ControlFunctions.NUL, null },
            { ControlFunctions.SOH, null },
            { ControlFunctions.STX, null },
            { ControlFunctions.ETX, null },
            { ControlFunctions.EOT, null },
            { ControlFunctions.ENQ, null },
            { ControlFunctions.ACK, null },
            { ControlFunctions.BEL, null },
            { ControlFunctions.BS, null },
            { ControlFunctions.HT, null },
            { ControlFunctions.LF, null },
            { ControlFunctions.VT, null },
            { ControlFunctions.FF, null },
            { ControlFunctions.CR, null },
            { ControlFunctions.SOorLS1, null },
            { ControlFunctions.S1orLS0, null },
            { ControlFunctions.DLE, null },
            { ControlFunctions.DC1, null },
            { ControlFunctions.DC2, null },
            { ControlFunctions.DC3, null },
            { ControlFunctions.DC4, null },
            { ControlFunctions.NAK, null },
            { ControlFunctions.SYN, null },
            { ControlFunctions.ETB, null },
            { ControlFunctions.CAN, null },
            { ControlFunctions.EM, null },
            { ControlFunctions.SUB, null },
            { ControlFunctions.ESC, new ESCControlFunctionParser() },
            { ControlFunctions.IS4, null },
            { ControlFunctions.IS3, null },
            { ControlFunctions.IS2, null },
            { ControlFunctions.IS1, null },
        };

        #region C0 set

        public static byte NUL = CharacterUtils.BitCombinations(00, 00);
        public static byte SOH = CharacterUtils.BitCombinations(00, 01);
        public static byte STX = CharacterUtils.BitCombinations(00, 02);
        public static byte ETX = CharacterUtils.BitCombinations(00, 03);
        public static byte EOT = CharacterUtils.BitCombinations(00, 04);
        public static byte ENQ = CharacterUtils.BitCombinations(00, 05);
        public static byte ACK = CharacterUtils.BitCombinations(00, 06);
        public static byte BEL = CharacterUtils.BitCombinations(00, 07);
        public static byte BS = CharacterUtils.BitCombinations(00, 08);
        public static byte HT = CharacterUtils.BitCombinations(00, 09);
        public static byte LF = CharacterUtils.BitCombinations(00, 10);
        public static byte VT = CharacterUtils.BitCombinations(00, 11);
        public static byte FF = CharacterUtils.BitCombinations(00, 12);
        public static byte CR = CharacterUtils.BitCombinations(00, 13);
        public static byte SOorLS1 = CharacterUtils.BitCombinations(00, 14);
        public static byte S1orLS0 = CharacterUtils.BitCombinations(00, 15);
        public static byte DLE = CharacterUtils.BitCombinations(01, 00);
        public static byte DC1 = CharacterUtils.BitCombinations(01, 01);
        public static byte DC2 = CharacterUtils.BitCombinations(01, 02);
        public static byte DC3 = CharacterUtils.BitCombinations(01, 03);
        public static byte DC4 = CharacterUtils.BitCombinations(01, 04);
        public static byte NAK = CharacterUtils.BitCombinations(01, 05);
        public static byte SYN = CharacterUtils.BitCombinations(01, 06);
        public static byte ETB = CharacterUtils.BitCombinations(01, 07);
        public static byte CAN = CharacterUtils.BitCombinations(01, 08);
        public static byte EM = CharacterUtils.BitCombinations(01, 09);
        public static byte SUB = CharacterUtils.BitCombinations(01, 10);
        public static byte ESC = CharacterUtils.BitCombinations(01, 11);
        public static byte IS4 = CharacterUtils.BitCombinations(01, 12);
        public static byte IS3 = CharacterUtils.BitCombinations(01, 13);
        public static byte IS2 = CharacterUtils.BitCombinations(01, 14);
        public static byte IS1 = CharacterUtils.BitCombinations(01, 15);

        #endregion

        public static bool IsControlFunction(byte c)
        {
            return ControlFunctionMap.ContainsKey(c);
        }

        /// <summary>
        /// 从一串字符中解析出所有ControlFunction从开头到结尾的字符
        /// 7位编码和8位编码通用
        /// </summary>
        /// <param name="chars">要解析的字符</param>
        /// <param name="functionBytes">解析出来的所有字符</param>
        /// <returns></returns>
        public static bool Parse(byte[] chars, out List<ControlFunctionParserResult> results)
        {
            results = new List<ControlFunctionParserResult>();
            int length = chars.Length;

            for (int idx = 0; idx < length; idx++)
            {
                byte c = chars[idx];
                ControlFunctionParser parser;
                if (ControlFunctionMap.TryGetValue(c, out parser))
                {
                    int endIdx;
                    ControlFunctionParserResult result;
                    if (!parser.Parse(chars, idx, out result, out endIdx))
                    {
                        return false;
                    }
                    idx = endIdx;
                    results.Add(result);
                }
            }

            return true;
        }
    }

    public struct ControlFunctionParserResult
    {
        /// <summary>
        /// 一段完整的ControlFunction数据
        /// </summary>
        public byte[] FunctionChars;

        /// <summary>
        /// Fe类型
        /// </summary>
        public byte FeChar;
    }


    /// <summary>
    /// 解析从ControlFunction开头到结尾的所有字符
    /// </summary>
    public abstract class ControlFunctionParser
    {
        /// <summary>
        /// 解析ControlFunction
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="offset">要解析的数据的偏移位置</param>
        /// <param name="result"></param>
        /// <param name="funcEndIdx">ControlFunction在chars里的最后一个字符的索引</param>
        /// <returns></returns>
        public abstract bool Parse(byte[] chars, int offset, out ControlFunctionParserResult result, out int funcEndIdx);
    }

    public class ESCControlFunctionParser : ControlFunctionParser
    {
        public override bool Parse(byte[] chars, int offset, out ControlFunctionParserResult result, out int funcEndIdx)
        {
            throw new NotImplementedException();
        }
    }
}