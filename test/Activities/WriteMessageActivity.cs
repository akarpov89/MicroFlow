namespace MicroFlow.Test
{
  public class WriteMessageActivity : SyncActivity
  {
    private readonly IWriter myWriter;

    public WriteMessageActivity(IWriter writer)
    {
      myWriter = writer;
    }

    [Required]
    public string Message { get; set; }

    protected override void ExecuteActivity()
    {
      myWriter.Write(Message);
    }
  }
}