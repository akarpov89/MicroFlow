using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MicroFlow.Test
{
  [TestFixture]
  public class BlockSelfContainednessValidatorTest
  {
    [Test, TestCaseSource(nameof(Cases))]
    public void RunCase(
      Action<FlowBuilder, IFlowNode> createInnerAndConnectToOuter,
      int errorsCount)
    {
      // Arrange
      var builder = new FlowBuilder();
      var a = builder.DummyActivity();

      var block = builder.Block("test", (_, b) => { createInnerAndConnectToOuter(b, a); });

      var validator = new BlockSelfContainednessValidator(block, null, null);

      // Act
      validator.Validate();

      // Assert
      Assert.That(validator.Result.GetErrorsOf(a).Count(), Is.EqualTo(errorsCount));
    }

    public static IEnumerable<TestCaseData> Cases
    {
      get
      {
        yield return Case("Activity", (b, a) => b.DummyActivity().ConnectTo(a), 1);
        yield return Case("Condition", (b, a) => b.Condition().ConnectFalseTo(a).ConnectTrueTo(a), 2);
        yield return Case("Switch", (b, a) =>
          b.SwitchOf<int>().ConnectCase(0).To(a).ConnectCase(1).To(a).ConnectDefaultTo(a),
          3);
        yield return Case("ForkJoin", (b, a) => b.ForkJoin().ConnectTo(a), 1);
        yield return Case("Block", (b, a) => b.Block().ConnectTo(a), 1);
      }
    }

    public static TestCaseData Case(
      string name,
      Action<FlowBuilder, IFlowNode> createInnerAndConnectToOuter,
      int errorsCount)
    {
      return new TestCaseData(createInnerAndConnectToOuter, errorsCount).SetName(name);
    }
  }
}