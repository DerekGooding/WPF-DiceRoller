using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.Wpf;
using Rayfer.DiceRoller.WPF.Content;
using Rayfer.DiceRoller.WPF.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Rayfer.DiceRoller.WPF;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
        modelImporter = new ModelImporter();
        diceTypes = new(DiceDatas.Data);
        SelectedDice = diceTypes[0];
    }

    private readonly ModelImporter modelImporter;

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

    public Model3D DiceModel => SelectedDice.IsPercentile
                ? modelImporter.Load(SelectedDice.AlternativeModelPath)
                : (Model3D)modelImporter.Load(SelectedDice.ModelPath);

    public double OffsetX => SelectedDice.IsPercentile ? 1 : 0;
    public double OffsetZ => SelectedDice.IsPercentile ? -0.5 : 0;

    public double D100OffsetX => SelectedDice.IsPercentile ? 0.5 : 0;
    public double D100OffsetZ => SelectedDice.IsPercentile ? -0.5 : 0;

    public Model3D? DiceModelD100 => SelectedDice.IsPercentile ? (Model3D)modelImporter.Load(SelectedDice.ModelPath) : null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiceModel))]
    [NotifyPropertyChangedFor(nameof(DiceModelD100))]
    [NotifyPropertyChangedFor(nameof(OffsetX))]
    [NotifyPropertyChangedFor(nameof(OffsetZ))]
    [NotifyPropertyChangedFor(nameof(D100OffsetX))]
    [NotifyPropertyChangedFor(nameof(D100OffsetZ))]
    private DiceData selectedDice;

    [ObservableProperty]
    private ObservableCollection<DiceData> diceTypes;

    #region Commands

    [RelayCommand]
    private void MoveXAngle(MouseWheelEventArgs mouseWheelMovement) => AngleX += Math.Sign(mouseWheelMovement.Delta) * 0.5;

    [RelayCommand]
    private void MoveYAngle(MouseWheelEventArgs mouseWheelMovement) => AngleY += Math.Sign(mouseWheelMovement.Delta) * 0.5;

    [RelayCommand]
    private void MoveZAngle(MouseWheelEventArgs mouseWheelMovement) => AngleZ += Math.Sign(mouseWheelMovement.Delta) * 0.5;

    [RelayCommand]
    private void RollDice()
    {
        if (SelectedDice.IsPercentile)
        {
            RollPercentile();
        }
        else
        {
            RollStandard();
        }
    }

    #endregion Commands

    #region Helper Methods

    private void RollPercentile()
    {
        RollResult = Random.Shared.Next(0, 100);
        HandleUnitsDie();
        HandlePercentileDie();
    }

    private void HandleUnitsDie()
    {
        const int Variability = 10;
        int i = RollResult % 10;
        double[] orientations = DiceTypes.First(x=>x.Name == "D10").Rotations[i];

        Storyboard storyboard = (Storyboard)Application.Current.MainWindow.FindResource("DiceRollStoryboard");
        AnimateDice(orientations, Variability, storyboard);
    }

    private void HandlePercentileDie()
    {
        const int Variability = 10;
        int i = RollResult / 10;
        double[] orientations = SelectedDice.Rotations[i];

        Storyboard storyboard = (Storyboard)Application.Current.MainWindow.FindResource("D100DiceRollStoryboard");
        AnimateDice(orientations, Variability, storyboard);
    }

    private void RollStandard()
    {
        RollResult = Random.Shared.Next(1, SelectedDice.FaceCount + 1);

        double[] orientations = SelectedDice.Rotations[RollResult];
        int Variability = SelectedDice.Variability;

        Storyboard storyBoard = (Storyboard)Application.Current.MainWindow.FindResource("DiceRollStoryboard");
        AnimateDice(orientations, Variability, storyBoard);
    }

    private static void AnimateDice(double[] orientations, double variability, Storyboard storyBoard)
    {
        for (int i = 0; i <= 3; i++)
        {
            DoubleAnimation rotation = (DoubleAnimation)storyBoard.Children[i];
            if (i < 3)
            {
                rotation.From = Random.Shared.Next(-720, -360);
                rotation.To = orientations[i] + ((Random.Shared.NextDouble() - 0.5D) * variability);
            }
            rotation.Duration = new Duration(TimeSpan.FromSeconds(1).Add(TimeSpan.FromSeconds(Random.Shared.NextDouble() * 0.75)));
        }

        storyBoard.Begin();
    }

    #endregion Helper Methods
}