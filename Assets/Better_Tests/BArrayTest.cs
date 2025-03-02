using Better.Collections;
using NUnit.Framework;

public class BArrayTest
{
    [Test]
    public void BArrayTest1()
    {
        var bArray = BArrayPool<int>.Default.Get(3);
        
        BArrayPool<int>.Default.Recycle(bArray);
        
        var bArray2 = BArrayPool<int>.Default.Get(3);
    }
}
