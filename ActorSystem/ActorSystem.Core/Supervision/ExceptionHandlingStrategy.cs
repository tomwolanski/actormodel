namespace ActorSystem.Core.Supervision
{
	public enum ExceptionHandlingStrategy
	{
		SkipMessage,
		KillChild,
		RestartChild
	}
}
