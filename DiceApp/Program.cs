using System.Drawing;
using System.Windows.Forms;

namespace DiceApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new DiceForm());
    }
}

public class DiceForm : Form
{
    private readonly int[] _diceSides = { 4, 6, 8, 10, 12, 20 };
    private readonly NumericUpDown _diceCountInput;
    private readonly FlowLayoutPanel _dicePanel;
    private readonly TextBox _resultsBox;
    private readonly Button _rollButton;
    private readonly Button _resetButton;
    private readonly List<DieRow> _dieRows = new();
    private readonly Random _random = new();

    private sealed class DieRow
    {
        public required ComboBox TypeSelector { get; init; }
        public required Label ResultLabel { get; init; }
        public required Button RerollButton { get; init; }
        public int LastRoll { get; set; }
        public bool IsRolled { get; set; }
    }

    public DiceForm()
    {
        Text = "Simple Dice App";
        Size = new Size(520, 520);
        StartPosition = FormStartPosition.CenterScreen;

        var root = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };

        var topRow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(10)
        };

        var countLabel = new Label
        {
            Text = "Number of dice:",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(FontFamily.GenericSansSerif, 10f),
            Margin = new Padding(0, 7, 5, 0)
        };

        _diceCountInput = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 20,
            Value = 1,
            Width = 60,
            Margin = new Padding(0, 5, 10, 0)
        };
        _diceCountInput.ValueChanged += (_, _) => BuildDiceRows((int)_diceCountInput.Value);

        _rollButton = new Button
        {
            Text = "Roll",
            AutoSize = true,
            Margin = new Padding(0, 5, 5, 0),
            Enabled = false
        };
        _rollButton.Click += (_, _) => RollDice();

        _resetButton = new Button
        {
            Text = "Reset",
            AutoSize = true,
            Margin = new Padding(0, 5, 0, 0),
            Enabled = false
        };
        _resetButton.Click += (_, _) => ResetRolls();

        topRow.Controls.AddRange([
            countLabel,
            _diceCountInput,
            _rollButton,
            _resetButton
        ]);

        _dicePanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(10, 0, 10, 10),
            Size = new Size(480, 240)
        };

        _resultsBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font(FontFamily.GenericMonospace, 10f),
            Height = 120,
            Width = 480,
            Margin = new Padding(10),
            Text = "Pick the number of dice and start rolling."
        };

        root.Controls.Add(topRow);
        root.Controls.Add(_dicePanel);
        root.Controls.Add(_resultsBox);

        Controls.Add(root);

        BuildDiceRows(1);
    }

    private void BuildDiceRows(int count)
    {
        _dicePanel.Controls.Clear();
        _dieRows.Clear();

        for (var i = 0; i < count; i++)
        {
            var row = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(5)
            };

            var dieLabel = new Label
            {
                Text = $"Die {i + 1}:",
                Width = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 6, 0, 0)
            };

            var typeSelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 80,
                Margin = new Padding(0, 4, 0, 0)
            };

            foreach (var sides in _diceSides)
            {
                typeSelector.Items.Add($"{sides} sides");
            }

            typeSelector.SelectedIndex = 1;

            var resultLabel = new Label
            {
                Text = "Result: -",
                Width = 100,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 6, 0, 0)
            };

            var rerollButton = new Button
            {
                Text = "Reroll",
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 0),
                Enabled = false
            };

            var dieRow = new DieRow
            {
                TypeSelector = typeSelector,
                ResultLabel = resultLabel,
                RerollButton = rerollButton,
                LastRoll = 0,
                IsRolled = false
            };

            rerollButton.Click += (_, _) => RerollOneDie(dieRow);

            row.Controls.Add(dieLabel);
            row.Controls.Add(typeSelector);
            row.Controls.Add(resultLabel);
            row.Controls.Add(rerollButton);
            _dicePanel.Controls.Add(row);
            _dieRows.Add(dieRow);
        }

        _rollButton.Enabled = count > 0;
        _resetButton.Enabled = true;
        _resultsBox.Text = "Click Roll to throw.";
    }

    private void RollDice()
    {
        var total = 0;
        foreach (var row in _dieRows)
        {
            total += RollDie(row);
        }

        _resultsBox.Text = $"Total: {total}";
    }

    private int RollDie(DieRow row)
    {
        var sides = _diceSides[row.TypeSelector.SelectedIndex];
        var roll = _random.Next(1, sides + 1);
        row.LastRoll = roll;
        row.IsRolled = true;
        row.ResultLabel.Text = $"Result: {roll}";
        row.RerollButton.Enabled = true;
        return roll;
    }

    private void RerollOneDie(DieRow row)
    {
        if (!row.IsRolled)
        {
            return;
        }

        row.LastRoll = _random.Next(1, _diceSides[row.TypeSelector.SelectedIndex] + 1);
        row.ResultLabel.Text = $"Result: {row.LastRoll}";
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        var total = 0;
        foreach (var row in _dieRows)
        {
            if (row.IsRolled)
            {
                total += row.LastRoll;
            }
        }

        _resultsBox.Text = $"Total: {total}";
    }

    private void ResetRolls()
    {
        foreach (var row in _dieRows)
        {
            row.IsRolled = false;
            row.LastRoll = 0;
            row.ResultLabel.Text = "Result: -";
            row.RerollButton.Enabled = false;
        }

        _resultsBox.Text = "Roll to throw.";
    }
}
