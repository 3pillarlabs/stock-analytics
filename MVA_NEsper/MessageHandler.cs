namespace MVA_NEsper
{
    internal delegate void UpdateHandler(double mva, int currentSymbolId);

    internal class MessageHandler
    {
        public event UpdateHandler UpdatePrediction;

        public MessageHandler()
        {
        }

        public void Update(double mva, int currentSymbolId)
        {
            if (UpdatePrediction != null)
                UpdatePrediction(mva, currentSymbolId);
        }
    }
}
