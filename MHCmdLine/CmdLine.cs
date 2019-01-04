using System;
using System.Collections.Generic;

namespace OCSS.Util.CmdLine {

   /// <summary>Class for testing/handling Command-line parameters</summary>
   /// <remarks>
   ///   Assumes a space delimited (Windows) style command line.
   ///   Assumes convention of dashes or similar for switches followed immediately by the associated parameter
   ///      ex) /fc:\temp\OutputFolder
   ///   To pass spaces in an option (eg. long file names), the option must be enclosed in quotes.
   ///   Flags are case-sensitive.
   ///   Extraneous flags from command line that don't match the accepted list are ignored.
   /// </remarks>
   /// <example>See sample in comment below</example>
   ///
   /*
      private CmdLine ProcessParms(IEnumerable<CmdFlag> flagset, string[] args) {
         CmdLine cmd = new CmdLine();  // take default leading chars of - (dash) and / (forward slash)
         foreach (var f in flagset) {
            cmd.AddFlag(f.Flag, f.IsRequired);
         }
         try {
            cmd.ProcessCmdLine(args);  // parse the command line
            return cmd;
         }
         catch (ArgumentException e) {
            return null;
         }
      }
   }
   */
   public class CmdLine {

      public static string LEADING_FLAGS = @"-/";

      public readonly string LeadingList;
      public string ExeName { get; private set; }
      public int CmdCnt { get { return pFlagList.Count; } }

      private List<CmdFlag> pFlagList;

      public CmdLine(): this(LEADING_FLAGS) { }

      /// <summary>Constructor</summary>
      /// <param name="leadChars">String containing the leading characters to allow</param>
      public CmdLine(string leadChars) {
         LeadingList = leadChars;
         ExeName = string.Empty;
         pFlagList = new List<CmdFlag>();
      }

      /// <summary>Adds one command line flag to allowed list</summary>
      /// <param name="flag">String containing the characters for a command line option</param>
      /// <param name="isReq">Whether the command line option is required</param>
      public void AddFlag(string flag, bool isReq = true) {
         AddFlag(new CmdFlag(flag, isReq));
      }

      /// <param name="flag">The Command Flag instance to add</param>
      public void AddFlag(CmdFlag flag) {
         if (pFlagList.Contains(flag))
            throw new ArgumentException("Duplicate flag: " + flag.Flag);
         pFlagList.Add(flag);
      }

      public bool ParmExists(string flag) {
         return pFlagList.Exists(f => f.Flag == flag && f.ExistsOnInput);
      }

      public string GetParm(string flag) {
         var ndx = pFlagList.FindIndex(f => f.Flag == flag);
         return (ndx >= 0) ? pFlagList[ndx].Parm : string.Empty;
      }

      public void ProcessCmdLine(IEnumerable<string> args) {
         // set the parameters for each flag
         foreach (string arg in args) {
            if (LeadingList.Contains(arg.Substring(0, 1))) {
               string oneArg = arg.Substring(1);   // Ignore leading flag
               foreach (CmdFlag cf in pFlagList) {
                  if (oneArg.StartsWith(cf.Flag)) {
                     cf.Parm = oneArg.Substring(cf.Flag.Length);
                     cf.ExistsOnInput = true;
                  }
               }
            }
         }
         // Check required paramters and throw an exception is any required flags are missing
         foreach (CmdFlag cf in pFlagList) {
            if (cf.IsRequired && cf.ExistsOnInput == false) {
               throw new ArgumentException("Required command line parameter " + cf.Flag + " is missing.");
            }
         }
      }

   }

   public class CmdFlag: IEquatable<CmdFlag> {
      public readonly string Flag;
      public readonly bool IsRequired;

      public string Parm { get; set; }
      public bool ExistsOnInput { get; internal set; }

      public CmdFlag(string flag, bool isReq) {
         Flag = flag;
         IsRequired = isReq;
         ExistsOnInput = false;
         Parm = string.Empty;
      }

      public bool Equals(CmdFlag flag) {
         // we are not comparing the IsRequired flag
         return (Flag == flag.Flag);
      }
   }
}
