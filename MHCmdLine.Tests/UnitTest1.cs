using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSS.Util.CmdLine;

namespace MHCmdLine.Tests {

   [TestClass]
   public class CmdLineTests {
      [TestMethod]
      public void WithoutOptionalFlagIsSuccessful() {
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
      public void MissingSwitchesWhenAllSuppliedIsCorrect() {
         CmdFlag[] allowedFlags = { new CmdFlag("i", true), new CmdFlag("o", true), new CmdFlag("v", false) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { @"-ie:\data\dl\test.txt", @"-oe:\data\dl\test.out" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line
         Assert.IsTrue(success, "success is false");
         var switches = cmd.GetMissingSwitchesRequired().ToArray();
         Assert.AreEqual(0, switches.Length, "switches has elements");
      }

      [TestMethod]
      public void WithoutRequiredFlagsFails() {
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

      [TestMethod]
      public void CmdWithPositionalsSucceeds() {
         CmdFlag[] allowedFlags = { new CmdFlag("a", true) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { "-aReqFlag", "Pos1", "$Pos2", "+Pos3" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line
         Assert.IsTrue(success, "success is false");
         var positionals = cmd.GetPositionalParms().ToList();
         Assert.AreEqual(3, positionals.Count, "Positionals not 3");
         Assert.AreEqual("Pos1", positionals[0], "Pos[0] not Pos1");
         Assert.AreEqual("$Pos2", positionals[1], "Pos[1] not $Pos2");
         Assert.AreEqual("+Pos3", positionals[2], "Pos[2] not +Pos3");
      }

      [TestMethod]
      public void CmdWithoutPositionalsSucceeds() {
         CmdFlag[] allowedFlags = { new CmdFlag("a", true) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { "-aReqFlag" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line
         Assert.IsTrue(success, "success is false");
         var positionals = cmd.GetPositionalParms().ToList();
         Assert.AreEqual(0, positionals.Count, "Positionals not zero");
      }

      [TestMethod]
      public void CmdWithExtraneousParmsSucceeds() {
         CmdFlag[] allowedFlags = { new CmdFlag("a", true) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { "-aReqFlag", "-bParm2", "-cParm3" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line
         Assert.IsTrue(success, "success is false");
         var extra = cmd.GetExtraneousParms().ToList();
         Assert.AreEqual(2, extra.Count, "Extraneous count not 2");
         Assert.AreEqual("bParm2", extra[0], "extra[0] not bParm2");
         Assert.AreEqual("cParm3", extra[1], "extra[1] not cParm3");
      }

      [TestMethod]
      public void CmdWithoutExtraneousSwitchesSucceeds() {
         CmdFlag[] allowedFlags = { new CmdFlag("a", true) };
         CmdLine cmd = new CmdLine(allowedFlags);
         string[] args = { "-aReqFlag" };
         bool success = cmd.ProcessCmdLine(args);  // parse the command line
         Assert.IsTrue(success, "success is false");
         var extra = cmd.GetExtraneousParms().ToList();
         Assert.AreEqual(0, extra.Count, "Extra parm count not zero");
      }

   }

}
