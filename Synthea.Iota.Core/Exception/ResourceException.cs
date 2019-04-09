namespace Synthea.Iota.Core.Exception
{
  using System;

  using Hl7.Fhir.Model;

  public class ResourceException : Exception
  {
    public ResourceException(OperationOutcome outcome)
    {
      this.Outcome = outcome;
    }

    public OperationOutcome Outcome { get; }
  }
}