namespace Rayfer.DiceRoller.WPF.Model;

public class DiceData
{
    public string Name { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
    public int FaceCount { get; set; }
    public Dictionary<int, double[]> Rotations { get; set; } = [];

    public int Variability { get; set; } = 10;

    public bool IsPercentile { get; set; }
    public string AlternativeModelPath { get; set; } = string.Empty;

    public override string ToString() => Name;
}
