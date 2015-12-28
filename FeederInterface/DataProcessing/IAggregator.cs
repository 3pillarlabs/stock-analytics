namespace StockInterface.DataProcessing
{
    public interface IAggregator<T,V>
    {
        T Aggregate(V currentVal);
    }
}
