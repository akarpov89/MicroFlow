namespace MicroFlow.Meta
{
  public interface IVariableManager
  {
    string GetVariableName();
    void AddVariable(NodeInfo node, string variableName);
    string GetVariableOf(NodeInfo node);
  }
}