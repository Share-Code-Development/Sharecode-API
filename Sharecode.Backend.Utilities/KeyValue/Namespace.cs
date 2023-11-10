namespace Sharecode.Backend.Utilities.KeyValue;

public class Namespace
{
    private readonly List<KeyValue> _keyValues;

    public Namespace(List<KeyValue> keyValues)
    {
        _keyValues = keyValues;
    }

    public Namespace()
    {
        _keyValues = new List<KeyValue>();
    }

    public KeyValue? Of(string key)
    {
        return _keyValues.FirstOrDefault(x => x.Key == key);
    }

    public IEnumerable<KeyValue> KeyValues => new List<KeyValue>(_keyValues);

    public IEnumerable<KeyValue> YieldKeyValues()
    {
        foreach (var keyValue in _keyValues)
        {
            yield return keyValue;
        }
    }
}