namespace NConfiguration.Serialization.SimpleTypes.Parsing
{
	public interface IParser<T>
	{
		bool TryParse(string rawInput, out T result);
	}
}
