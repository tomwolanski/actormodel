namespace ActorSystem.Core.Supervision
{
	public enum ExceptionHandelingStrategy
	{
		SkipMessage,
		KillChild,
		RestartChild
	}
}
