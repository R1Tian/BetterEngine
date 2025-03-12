using Better.Collections;
using NUnit.Framework;

public class BArrayTest
{
    [Test]
    public void BArrayTest1()
    {
        var a = BArrayPool<int>.Default.Get(7);
        
        a.ResetLength(13);
    }
}
