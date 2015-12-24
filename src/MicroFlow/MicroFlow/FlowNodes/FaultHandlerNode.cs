namespace MicroFlow
{
    public class FaultHandlerNode<TActivity> : ActivityNode<TActivity>, IFaultHandlerNode
        where TActivity : class, IFaultHandlerActivity
    {
        public void SubscribeToExceptionsOf(IActivityNode node)
        {
            node.AssertNotNull("node != null");
            Bind(x => x.Exception).ToExceptionOf(node);
        }
    }
}