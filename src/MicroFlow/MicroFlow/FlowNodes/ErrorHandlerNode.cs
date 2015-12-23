namespace MicroFlow
{
    public class ErrorHandlerNode<TActivity> : ActivityNode<TActivity>, IErrorHandlerNode
        where TActivity : class, IErrorHandler
    {
        public void SubscribeToErrorsOf(IActivityNode node)
        {
            node.AssertNotNull("node != null");
            Bind(x => x.Exception).ToExceptionOf(node);
        }
    }
}