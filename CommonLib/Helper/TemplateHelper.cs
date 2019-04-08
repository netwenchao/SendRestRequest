using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
Random(0-100)
[A-Z0-9]{}
*/
namespace CommonLib.Helper
{
    /// <summary>
    /// Rand(1,2)
    /// Rand(1,2){3}
    /// Rand(1,2){3,5}
    /// Letter{10,20}
    /// LetterU{10,20}
    /// LetterL{10,20}
    /// </summary>
    public class TemplateHelper
    {
        #region MetaFunc

        protected static Func<Random, int, int, string> RandInt = (r, min, max) => { return r.Next(min, max).ToString(); };
        protected static Func<Random, int, int, string> RandLetter = (r, min, max) => {
            var charCode = r.Next(65, 122);
            if (charCode >= 90 && charCode <= 93) charCode = 90;
            if (charCode > 93 && charCode <= 97) charCode = 97;
            return ((char)charCode).ToString();
        };
        protected static Func<Random, int, int, string> RandUpperLetter = (r, min, max) => {
            var charCode = r.Next(65, 90);
            return ((char)charCode).ToString();
        };
        protected static Func<Random, int, int, string> RandLowerLetter = (r, min, max) => {
            var charCode = r.Next(97, 122);
            return ((char)charCode).ToString();
        };
        
        public static string Range(string first, string lst, string repeatMin = "1", string repeatMax = "1",
            Func<Random, int, int, string> randFunc = null)
        {
            randFunc = randFunc == null ? RandInt : randFunc;
            var min = 0;
            var max = 0;
            var rep = 1;
            var repM = 1;
            var ran = new Random(DateTime.Now.Millisecond);
            min = int.TryParse(first, out min) ? min : 0;
            max = int.TryParse(lst, out max) ? max : 0;
            rep = int.TryParse(repeatMin, out rep) ? rep : 1;
            repM = int.TryParse(repeatMax, out repM) ? repM : 1;
            rep = ran.Next(Math.Min(rep, repM), Math.Max(rep, repM));
            var output = new StringBuilder();
            while ((rep--) >= 0)
            {
                output.Append(randFunc(ran, Math.Min(min, max), Math.Max(min, max)));
            }
            return output.ToString();
        }
        #endregion

        public static Dictionary<Regex, MatchEvaluator> predefined = new Dictionary<Regex, MatchEvaluator>() {
            { new Regex(@"Rand\((\d+),(\d+)\)(\{(\d+)(,(\d+))?\})?",RegexOptions.Multiline),(Match m)=>{
                    if(m.Groups.Count==3){
                        return Range(m.Groups[1].Value,m.Groups[2].Value);
                    }

                    if(m.Groups.Count==5)
                    {
                        return Range(m.Groups[1].Value,m.Groups[2].Value,m.Groups[4].Value);
                    }

                    if(m.Groups.Count==7){
                        return Range(m.Groups[1].Value,m.Groups[2].Value,m.Groups[4].Value,m.Groups[6].Value);
                    }
                    return "";
                }
            },
            { new Regex(@"Letter\{(\d+)(,(\d+))?\}",RegexOptions.Multiline),(Match m)=>{
                    var ran=new Random(DateTime.Now.Millisecond);
                    if(m.Groups.Count==2){
                        return Range("","",m.Groups[1].Value,m.Groups[1].Value,RandLetter);
                    }
                    if(m.Groups.Count==4){
                        return Range("","",m.Groups[1].Value,m.Groups[3].Value,RandLetter);
                    }
                    return "";
                }
            },
            { new Regex(@"LetterU\{(\d+)(,(\d+))?\}",RegexOptions.Multiline),(Match m)=>{
                    var ran=new Random(DateTime.Now.Millisecond);
                    if(m.Groups.Count==2){
                        return Range("","",m.Groups[1].Value,m.Groups[1].Value,RandUpperLetter);
                    }
                    if(m.Groups.Count==4){
                        return Range("","",m.Groups[1].Value,m.Groups[3].Value,RandUpperLetter);
                    }
                    return "";
                }
            },
            { new Regex(@"LetterL\{(\d+)(,(\d+))?\}",RegexOptions.Multiline),(Match m)=>{
                    var ran=new Random(DateTime.Now.Millisecond);
                    if(m.Groups.Count==2){
                        return Range("","",m.Groups[1].Value,m.Groups[1].Value,RandLowerLetter);
                    }
                    if(m.Groups.Count==4){
                        return Range("","",m.Groups[1].Value,m.Groups[3].Value,RandLowerLetter);
                    }
                    return "";
                }
            }
        };

        public static string Exec(string tmpl)
        {
            if (string.IsNullOrWhiteSpace(tmpl)) return "";
            var rslt = tmpl;
            foreach (var key in predefined.Keys)
            {
                rslt = key.Replace(rslt, predefined[key]);
            }
            return rslt;
        }

    }
}
