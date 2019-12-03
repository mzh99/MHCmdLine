using System;
using System.Linq;
using System.Collections.Generic;

namespace OCSS.Util.CmdLine {

   /// <summary>Class for parsing, handling, and processing command-line parameters using conventions below.</summary>
   /// <remarks>
   ///   Assumptions and Conventions:
   ///      1) a space delimited (Windows) style command-line.
   ///      2) Flags are not positional and can be used anywhere in the command-line.
   ///      3) a convention of dashes (or other user-defined characters) for switches followed immediately by the associated parameter.
   ///         ex) /fc:\temp\OutputFolder
   ///      4) Flags are case-sensitive (-m is different than -M).
   ///
   ///   Other Notes:
   ///      When providing command line values with spaces (eg. long file names), the entire option (leading flag + switch + parameter) must be enclosed in quotes.
   ///         ex) "/fc:\A path with spaces\output.txt"
   ///      Command-line parameters without switches are assumed to be positional and can be obtained with a call to GetPositionalParms().
   ///      Command-line parameters starting with a leading character that don't match one of the allowed flags can be obtained with a call to GetExtraneousParms().
   /// </remarks>
   /// <example>See github samples</example>

   public class CmdLine {

      /// <summary>Default preamble for error message in GetMissingSwitchesAsErrorMessage()</summary>
      public const string DEFAULT_MISSING_SWITCH_PREAMBLE = "Missing required command line switches: ";
      /// <summary>Default prefixes for flags</summary>
      public static readonly char[] DEFAULT_FLAG_PREFIX = new char[] { '-', '/' };
      /// <summary>List of characters recognized as a valid prefix for parameters</summary>
      public readonly char[] LeadingList;
      public int CmdCnt { get { return flagList.Count; } }

      private readonly List<string> positionals = new List<string>();
      private readonly List<string> extraneousSwitches = new List<string>();
      private readonly Dictionary<string, CmdFlag> flagList;

#region Constructors
      /// <summary>Constructor using default prefix list (dash and forward slash)</summary>
      public CmdLine(): this(DEFAULT_FLAG_PREFIX) { }

      /// <summary>Constructor</summary>
      /// <param name="leadChars">String containing the leading characters to allow</param>
      public CmdLine(char[] leadChars): this(leadChars, null) { }

      /// <summary>Constructor using default prefix list (dash and forward slash) and supplied flags</summary>
      /// <param name="flags">list of flags recognized as parameters</param>
      public CmdLine(IEnumerable<CmdFlag> flags): this(DEFAULT_FLAG_PREFIX, flags) { }

      /// <summary>Constructor</summary>
      /// <param name="leadChars">valid leading characters for parameters</param>
      /// <param name="flags">list of flags recognized as parameters</param>
      public CmdLine(char[] leadChars, IEnumerable<CmdFlag> flags) {
         LeadingList = leadChars;
         flagList = new Dictionary<string, CmdFlag>();
         if (flags != null) {
            foreach (var flag in flags) {
               AddFlag(flag);
            }
         }
      }
#endregion

      /// <summary>Adds one command line flag to allowed list</summary>
      /// <param name="flag">String containing the leading characters for a command line parameter</param>
      /// <param name="isReq">Whether the command line option is required</param>
      public void AddFlag(string flag, bool isReq = true) {
         AddFlag(new CmdFlag(flag, isReq));
      }

      /// <param name="flag">The Command Flag instance to add</param>
      public void AddFlag(CmdFlag flag) {
         flagList.Add(flag.Flag, flag);
      }

      public void AddFlags(IEnumerable<CmdFlag> flags) {
         foreach (var flag in flags) {
            AddFlag(flag);
         }
      }

      public bool ParmExists(string flag) {
         var ret = false;
         if (flagList.ContainsKey(flag)) {
            ret = flagList[flag].ExistsOnInput;
         }
         return ret;
      }

      public string GetParm(string flag) {
         return (flagList.ContainsKey(flag)) ? flagList[flag].Parm : string.Empty;
      }

      /// <summary>Parses command-line arguments and matches them up with flags</summary>
      /// <param name="args"></param>
      /// <returns>True if all required flags are matched up with arguments. Otherwise, false.</returns>
      public bool ProcessCmdLine(IEnumerable<string> args) {
         positionals.Clear();
         extraneousSwitches.Clear();
         // set the parameters for each flag
         foreach (string arg in args) {
            // does first character match
            if (LeadingList.Contains(arg[0])) {
               string oneArg = arg.Substring(1);   // Ignore leading flag and get the rest of the parameter
               var match = flagList.Where((k, v) => oneArg.StartsWith(k.Key)).FirstOrDefault().Value;
               if (match == null) {
                  extraneousSwitches.Add(oneArg);
               }
               else {
                  match.Parm = oneArg.Substring(match.Flag.Length);
                  match.ExistsOnInput = true;
               }
            }
            else {
               positionals.Add(arg);
            }
         }
         // Check required paramters and throw an exception is any required flags are missing
         var flagsMissing = GetMissingSwitchesRequired().ToArray();
         return (flagsMissing.Length == 0);
      }

      public IEnumerable<string> GetPositionalParms() {
         return positionals.AsReadOnly();
      }

      public IEnumerable<string> GetExtraneousParms() {
         return extraneousSwitches.AsReadOnly();
      }

      public IEnumerable<string> GetMissingSwitchesRequired() {
         return flagList.Where((k, v) => k.Value.IsRequired && k.Value.ExistsOnInput == false).Select(r => r.Value.Flag);
      }

      public string GetMissingSwitchesAsErrorMessage(string msgPreamble = DEFAULT_MISSING_SWITCH_PREAMBLE, string switchSeparator = ", ") {
         var switches = GetMissingSwitchesRequired().ToArray();
         return (switches.Length == 0) ? string.Empty : msgPreamble + string.Join(switchSeparator, switches);
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
