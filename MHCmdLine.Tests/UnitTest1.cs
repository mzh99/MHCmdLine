using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSS.Util.CmdLine;

namespace MHCmdLine.Tests {

   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void CmdWithoutOptPasses() {
         CmdFlag[] allowedFlags = { new CmdFlag("i", true), new CmdFlag("o", true), new CmdFlag("v", false) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { @"-ie:\data\dl\test.txt", @"-oe:\data\dl\test.out" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line

         Assert.IsTrue(success, "success is false");
         Assert.IsTrue(cmd.ParmExists("o"), "parm o does not exist");
         Assert.AreEqual(@"e:\data\dl\test.out", cmd.GetParm("o"), "parm 'o' does not match");
         Assert.IsTrue(cmd.ParmExists("i"), "parm o does not exist");
         Assert.AreEqual(@"e:\data\dl\test.txt", cmd.GetParm("i"), "parm 'i' does not match");
         Assert.IsFalse(cmd.ParmExists("v"), "parm v exists");
         Assert.IsFalse(cmd.ParmExists("O"), "parm O exists");
      }

      [TestMethod]
      public void CmdWithoutWithMissingReqFails() {
         CmdFlag[] allowedFlags = { new CmdFlag("i", true), new CmdFlag("o", true), new CmdFlag("v", false) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { @"-oe:\data\dl\test.out" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line

         Assert.IsFalse(success, "success is true");
         Assert.IsTrue(cmd.ParmExists("o"), "parm o does not exist");
         Assert.AreEqual(@"e:\data\dl\test.out", cmd.GetParm("o"), "parm 'o' does not match");
         Assert.IsFalse(cmd.ParmExists("i"), "parm i exists");
         Assert.IsFalse(cmd.ParmExists("v"), "parm v exists");
         Assert.IsFalse(cmd.ParmExists("O"), "parm O exists");
      }
   }

}
