using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using HelixToolkit.Wpf;

namespace Rayfer.DiceRoller.WPF;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ModelImporter modelImporter;
    private readonly Dictionary<DiceFaces, Dictionary<int, double[]>> diceModelFaceRotations;

    private readonly string[] diceModels =
    [
        "Resources\\D4.obj",
        "Resources\\D6.obj",
        "Resources\\D8.obj",
        "Resources\\D10.obj",
        "Resources\\D12.obj",
        "Resources\\D20.obj",
        "Resources\\D100.obj",
    ];

    private readonly int[] diceFacesNumbers = [5, 7, 9, 11, 13, 21, 101];

    private readonly Dictionary<int, double[]> d4FaceRotationsDictionary = new()
    {
        { 1, [-60, 95, 15] },
        { 2, [-5, -20, 60] },
        { 3, [180, 40, 15] },
        { 4, [60, 25, -10] }
    };

    private readonly Dictionary<int, double[]> d6FaceRotationsDictionary = new()
    {
        { 1, [35, 210, 0] },
        { 2, [-240, 170, -70] },
        { 3, [-50, 140, 50] },
        { 4, [135, -45, 45] },
        { 5, [-105, 140, -180] },
        { 6, [-30, -20, -90] }
    };

    private readonly Dictionary<int, double[]> d8FaceRotationsDictionary = new()
    {
        { 1, [20, -15, -15] },
        { 2, [165, 168, -15] },
        { 3, [50, 70, 40] },
        { 4, [175, -115, -18] },
        { 5, [-22, -190, -20] },
        { 6, [190, -20, -20] },
        { 7, [-4, -115, -20] },
        { 8, [200, 75, 0] }
    };

    private readonly Dictionary<int, double[]> d10FaceRotationsDictionary = new()
    {
        { 0, [0, -175, 0] },
        { 1, [180, 165, 0] },
        { 2, [0, 110, 0] },
        { 3, [180, 95, 0] },
        { 4, [0, 40, 0] },
        { 5, [-180, 20, 0] },
        { 6, [0, -33, 0] },
        { 7, [-180, -50, 0] },
        { 8, [0, -102, 0] },
        { 9, [-180, -125, 0] },
        { 10, [0, -175, 0] },
    };

    private readonly Dictionary<int, double[]> d12FaceRotationsDictionary = new()
    {
        { 1, [111, -165, 134] },
        { 2, [-159, -167, -157] },
        { 3, [170, -3.5, -30.5] },
        { 4, [-21, -61, 97] },
        { 5, [115, 162, 70] },
        { 6, [104, -146, 20] },
        { 7, [125, -24, 163] },
        { 8, [-177, -26, 83] },
        { 9, [100.5, 22, -149.5] },
        { 10, [168, 153, 148.5] },
        { 11, [151, 65, -56] },
        { 12, [179, 52, 88.5] },
    };

    private readonly Dictionary<int, double[]> d20FaceRotationsDictionary = new()
    {
        { 1, [84, 38.5, 36] },
        { 2, [55.5, 160, 78] },
        { 3, [-242.5, -18.5, 15] },
        { 4, [124.5, 159, 133] },
        { 5, [16, 27.5, -115] },
        { 6, [-18, -25, 67.5] },
        { 7, [91, 35, -32.5] },
        { 8, [85.5, -140, -3] },
        { 9, [-52.5, -102.5, -68.5] },
        { 10, [14, -147, 48] },
        { 11, [57.5, -16.5, 139.5] },
        { 12, [56, 163, 6.5] },
        { 13, [59.5, -19, -148] },
        { 14, [-61, 20, 33.5] },
        { 15, [93.5, 34, -103] },
        { 16, [-89.5, -39, 71] },
        { 17, [164.5, 31, 46.5] },
        { 18, [109, 98.5, 161.5] },
        { 19, [-54, -165.5, -100.5] },
        { 20, [232.5, 16, -28.5] },
    };

    private readonly Dictionary<int, double[]> d100FaceRotationsDictionary = new()
    {
        { 0, [180, 110, 0] },
        { 1, [0, -50, 0] },
        { 2, [180, 40, 0] },
        { 3, [-5, -123, -5] },
        { 4, [180, -33, 0] },
        { 5, [0, 163, 0] },
        { 6, [180, -104, 0] },
        { 7, [0, 93, 0] },
        { 8, [180, 180, 0] },
        { 9, [0, 23, 0] },
    };

    [ObservableProperty]
    private double angleX;

    [ObservableProperty]
    private double angleY;

    [ObservableProperty]
    private double angleZ;

    [ObservableProperty]
    private double d100AngleX;

    [ObservableProperty]
    private double d100AngleY;

    [ObservableProperty]
    private double d100AngleZ;

    [ObservableProperty]
    private int rollResult;

    public Model3D DiceModel => SelectedDice is DiceFaces.D100
                ? modelImporter.Load(diceModels[(int)DiceFaces.D10])
                : (Model3D)modelImporter.Load(diceModels[(int)SelectedDice]);

    public double OffsetX => SelectedDice is DiceFaces.D100 ? 1 : 0;
    public double OffsetZ => SelectedDice is DiceFaces.D100 ? -0.5 : 0;

    public double D100OffsetX => SelectedDice is DiceFaces.D100 ? 0.5 : 0;
    public double D100OffsetZ => SelectedDice is DiceFaces.D100 ? -0.5 : 0;

    public Model3D? DiceModelD100 => SelectedDice is not DiceFaces.D100 ? null : (Model3D)modelImporter.Load(diceModels[(int)SelectedDice]);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiceModel))]
    [NotifyPropertyChangedFor(nameof(DiceModelD100))]
    [NotifyPropertyChangedFor(nameof(OffsetX))]
    [NotifyPropertyChangedFor(nameof(OffsetZ))]
    [NotifyPropertyChangedFor(nameof(D100OffsetX))]
    [NotifyPropertyChangedFor(nameof(D100OffsetZ))]
    private DiceFaces selectedDice;

    [ObservableProperty]
    private ObservableCollection<DiceFaces> diceTypes;

    [RelayCommand]
    private void RollDice()
    {
        if (SelectedDice is DiceFaces.D100)
        {
            RollResult = Random.Shared.Next(0, 100);

            var unitsRolledNumber = RollResult % 10;
            var d10FaceOrientation = diceModelFaceRotations[DiceFaces.D10][unitsRolledNumber];
            const int Variability = 10;

            var tensRolledNumber = RollResult / 10;
            var d100FaceOrientation = diceModelFaceRotations[DiceFaces.D100][tensRolledNumber];

            var diceRollStoryboard = (Storyboard)Application.Current.MainWindow.FindResource("DiceRollStoryboard");
            AnimateDice(d10FaceOrientation, Variability, diceRollStoryboard);

            var d100DiceRollStoryboard = (Storyboard)Application.Current.MainWindow.FindResource("D100DiceRollStoryboard");
            AnimateDice(d100FaceOrientation, Variability, d100DiceRollStoryboard);
        }
        else
        {
            RollResult = Random.Shared.Next(1, diceFacesNumbers[(int)SelectedDice]);

            var diceFaceOrientation = diceModelFaceRotations[SelectedDice][RollResult];
            var Variability = SelectedDice == DiceFaces.D6 ? 20 : 10;

            var diceRollStoryboard = (Storyboard)Application.Current.MainWindow.FindResource("DiceRollStoryboard");
            AnimateDice(diceFaceOrientation, Variability, diceRollStoryboard);
        }
    }

    private static void AnimateDice(double[] faceOrientations, double orientationVariability, Storyboard diceRollStoryboard)
    {
        for (int i = 0; i <= 3; i++)
        {
            DoubleAnimation rotation = (DoubleAnimation)diceRollStoryboard.Children[i];
            if (i < 3)
            {
                rotation.From = Random.Shared.Next(-720, -360);
                rotation.To = faceOrientations[i] + ((Random.Shared.NextDouble() - 0.5D) * orientationVariability);
            }
            rotation.Duration = new Duration(TimeSpan.FromSeconds(1).Add(TimeSpan.FromSeconds(Random.Shared.NextDouble() * 0.75)));
        }

        diceRollStoryboard.Begin();
    }

    [RelayCommand]
    private void MoveXAngle(MouseWheelEventArgs mouseWheelMovement) => AngleX += Math.Sign(mouseWheelMovement.Delta) * 0.5;

    [RelayCommand]
    private void MoveYAngle(MouseWheelEventArgs mouseWheelMovement) => AngleY += Math.Sign(mouseWheelMovement.Delta) * 0.5;

    [RelayCommand]
    private void MoveZAngle(MouseWheelEventArgs mouseWheelMovement) => AngleZ += Math.Sign(mouseWheelMovement.Delta) * 0.5;

    public MainWindowViewModel()
    {
        modelImporter = new ModelImporter();
        diceTypes =
        [
            DiceFaces.D4,
            DiceFaces.D6,
            DiceFaces.D8,
            DiceFaces.D10,
            DiceFaces.D12,
            DiceFaces.D20,
            DiceFaces.D100
        ];
        SelectedDice = DiceFaces.D4;
        diceModelFaceRotations = new()
        {
            {DiceFaces.D4, d4FaceRotationsDictionary },
            {DiceFaces.D6, d6FaceRotationsDictionary },
            {DiceFaces.D8, d8FaceRotationsDictionary },
            {DiceFaces.D10, d10FaceRotationsDictionary },
            {DiceFaces.D12, d12FaceRotationsDictionary },
            {DiceFaces.D20, d20FaceRotationsDictionary },
            {DiceFaces.D100, d100FaceRotationsDictionary },
        };
    }
}
