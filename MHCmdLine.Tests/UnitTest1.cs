using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSS.Util.CmdLine;

namespace MHCmdLine.Tests {

   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void CmdWithoutOptPasses() {
         CmdFlag[] allowedFlags = { new CmdFlag("i", true), new CmdFlag("o", true), new CmdFlag("v", false) };
         string[] args = { @"-ie:\data\dl\test.txt", @"-oe:\data\dl\test.out" };
         var cmd = ProcessParms(allowedFlags, args);
         Assert.IsNotNull(cmd,"cmd is null");
         Assert.IsTrue(cmd.ParmExists("o"),"parm o does not exist");
         Assert.AreEqual(@"e:\data\dl\test.out",cmd.GetParm("o"),"parm 'o' does not match");
         Assert.IsTrue(cmd.ParmExists("i"),"parm o does not exist");
         Assert.AreEqual(@"e:\data\dl\test.txt",cmd.GetParm("i"),"parm 'i' does not match");
         Assert.IsTrue(cmd.ParmExists("v") == false,"parm v exists");
         Assert.IsTrue(cmd.ParmExists("O") == false,"parm O exists");
      }

      private CmdLine ProcessParms(IEnumerable<CmdFlag> flagset, string[] args) {
         CmdLine cmd = new CmdLine();  // take default leading chars of - (dash) and / (forward slash)
         foreach (var flag in flagset) {
            cmd.AddFlag(flag.Flag, flag.IsRequired);
         }
         try {
            cmd.ProcessCmdLine(args);  // parse the command line
            return cmd;
         }
         catch (ArgumentException) {
            return null;
         }
      }
   }

}
