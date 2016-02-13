namespace MicroFlow.Test
{
  public class SumActivity : SyncActivity
  {
    private readonly IWriter myWriter;

    public SumActivity(IWriter writer)
    {
      myWriter = writer;
    }

    [Required]
    public int A { get; set; }

    [Required]
    public int B { get; set; }

    [Required]
    public int C { get; set; }

    protected override void ExecuteActivity()
    {
      myWriter.Write($"{A} + {B} + {C} = {A + B + C}");
    }
  }
}