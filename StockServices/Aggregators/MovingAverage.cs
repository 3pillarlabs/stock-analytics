using StockInterface.DataProcessing;
using StockModel;

namespace StockServices.Aggregators
{
    public class MovingAverage : IAggregator<double, double>
    {
        int count = 0;
        double aggregation = 0;

        public double Aggregate(double currentVal)
        {
            aggregation += currentVal;
            return aggregation / ++count;
        }
    }
}
