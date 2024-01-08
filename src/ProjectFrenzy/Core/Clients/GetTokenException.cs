using System;

namespace ProjectFrenzy.Core.Clients
{
  public class GetTokenException : InvalidOperationException
  {
    public GetTokenException(Exception inner) : base("Can't get token", inner)
    {
    }
  }
}