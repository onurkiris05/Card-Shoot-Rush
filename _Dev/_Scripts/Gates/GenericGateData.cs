namespace Game.Gates
{
    public class GenericGateData
    {
        public GenericGateType Type;
        public float Amount;

        public GenericGateData(GenericGateType type, float amount)
        {
            Type = type;
            Amount = amount;
        }
    }

    public enum GenericGateType
    {
        Rate,
        Range
    }
}