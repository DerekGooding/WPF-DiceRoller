using Rayfer.DiceRoller.WPF.Model;

namespace Rayfer.DiceRoller.WPF.Builders;

public static class DiceBuilder
{
    public static ISetFaceCount Name(string name) => new Builder().Name(name);
    public interface ISetModelPath
    {
        ISetFaceCount Name(string name);
    }

    public interface ISetFaceCount
    {
        ISetRotations FaceCount(int count);
    }

    public interface ISetRotations
    {
        ISetRotations CustomVariability(int value);
        ISetRotations IsPercentile(string alternativeDice);
        DiceData Rotations(Dictionary<int, double[]> values);
    }

    private class Builder : ISetFaceCount, ISetModelPath, ISetRotations
    {
        private readonly DiceData _diceData = new();

        public ISetFaceCount Name(string name)
        {
            _diceData.ModelPath = $"Resources\\{name}.obj";
            _diceData.Name = name;
            return this;
        }

        public ISetRotations CustomVariability(int value)
        {
            _diceData.Variability = value;
            return this;
        }
        public ISetRotations IsPercentile(string AlternativeDice)
        {
            _diceData.IsPercentile = true;
            _diceData.AlternativeModelPath = $"Resources\\{AlternativeDice}.obj";
            return this;
        }
        public ISetRotations FaceCount(int count)
        {
            _diceData.FaceCount = count;
            return this;
        }
        public DiceData Rotations(Dictionary<int, double[]> values)
        {
            _diceData.Rotations = values;
            return _diceData;
        }
    }
}
