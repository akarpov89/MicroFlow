using System;

namespace MicroFlow.Test
{
  public class ThrowService
  {
    public ThrowService()
    {
      throw new Exception("Exception from ThrowService::ctor");
    }
  }
}