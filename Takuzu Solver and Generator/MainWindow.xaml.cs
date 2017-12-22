﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Takuzu_Solver_and_Generator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // Vars
        readonly Button[] playCells, solveCells;
        private event Action<int> update;
        private Int64 count = 0;
        private Int64 maxCount = 0;

        // INIT
        public MainWindow() {
            InitializeComponent();

            // Cells
            playCells = new Button[]{
                cellPlay0, cellPlay1, cellPlay2, cellPlay3, cellPlay4, cellPlay5, cellPlay6, cellPlay7, cellPlay8, cellPlay9,
                cellPlay10, cellPlay11, cellPlay12, cellPlay13, cellPlay14, cellPlay15, cellPlay16, cellPlay17, cellPlay18, cellPlay19,
                cellPlay20, cellPlay21, cellPlay22, cellPlay23, cellPlay24, cellPlay25, cellPlay26, cellPlay27, cellPlay28, cellPlay29,
                cellPlay30, cellPlay31, cellPlay32, cellPlay33, cellPlay34, cellPlay35
            };
            solveCells = new Button[]{
                cellSolve0, cellSolve1, cellSolve2, cellSolve3, cellSolve4, cellSolve5, cellSolve6, cellSolve7, cellSolve8, cellSolve9,
                cellSolve10, cellSolve11, cellSolve12, cellSolve13, cellSolve14, cellSolve15, cellSolve16, cellSolve17, cellSolve18, cellSolve19,
                cellSolve20, cellSolve21, cellSolve22, cellSolve23, cellSolve24, cellSolve25, cellSolve26, cellSolve27, cellSolve28, cellSolve29,
                cellSolve30, cellSolve31, cellSolve32, cellSolve33, cellSolve34, cellSolve35
            };
            tbCode_Play.Text = "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";
            tbOutput_Play.AppendText("Welcome! \n"); 
        }

        // GEN
        private void BtnGenerate_Gen_Click(object sender, RoutedEventArgs e) {

        }

        private void BtnSave_Gen_Click(object sender, RoutedEventArgs e) {

        }

        // SOL
        private void BtnSolve_Sol_Click(object sender, RoutedEventArgs e) {
            count = 0;
            maxCount = (Int64)Math.Round(Math.Pow(2, (tbCode_Sol.Text.Length - tbCode_Sol.Text.Replace("e", "").Length)));
            tbSolution_Sol.Text = "";

            update += updatePbSolve;
            trySolve(tbCode_Sol.Text);
            update -= updatePbSolve;
            updatePbSolve(100);
        }

        private void BtnFill_Sol_Click(object sender, RoutedEventArgs e) {
            if (InsertCodeIntoGrid(tbCode_Sol.Text, solveCells)) {
                btnSolve_Sol.IsEnabled = true;
                tbOutput_Sol.AppendText("Cell fill succses! \n");
            } else {
                btnSolve_Sol.IsEnabled = false;
                EmptyGrid(solveCells);
                tbOutput_Sol.AppendText("Cell fill failed, instead just cleared...\n");
            }
        }

        // PLAY
        private void BtnCheck_Play_Click(object sender, RoutedEventArgs e) {
            var errors = CheckCodeIsSolution(tbCurrent_Play.Text);

            if (errors.Count == 0) {
                tbOutput_Play.AppendText("Congrats its all good!\n");
                foreach (var cell in playCells) cell.IsEnabled = false;
            } else {
                tbOutput_Play.AppendText("Whoops some errors found:\n");
                foreach (var error in errors) tbOutput_Play.AppendText(error + "\n");
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                var cell = "" + button.Content as string;
                button.Content = (cell == "") ? "0" : ((cell == "0") ? "1" : "0");
                UpdateCode(playCells, tbCurrent_Play);
                tbOutput_Play.AppendText("Cell edited\n");
            }
        }

        private void BtnFill_Play_Click(object sender, RoutedEventArgs e) {
            if(InsertCodeIntoGrid(tbCode_Play.Text, playCells)) {
                btnCheck_Play.IsEnabled = true;
                tbCurrent_Play.Text = tbCode_Play.Text;
                tbOutput_Play.AppendText("Cell fill succses! \n");
            } else {
                btnCheck_Play.IsEnabled = false;
                tbCurrent_Play.Text = "";
                EmptyGrid(playCells);
                tbOutput_Play.AppendText("Cell fill failed, instead just cleared...\n");
            }
        }

        // HELPERS
        private void trySolve(string input) {
            StringBuilder s = new StringBuilder(input);

            // End point?
            if ((input.Length - input.Replace("e", "").Length) == 0) {
                update((int)((++count * 100) / maxCount));
                var errors = CheckCodeIsSolution(input.ToString());
                if (errors.Count == 0) tbSolution_Sol.AppendText("Sol: " + input.ToString() + "; ");
                return;
            }

            // RECURSE ME
            int i = input.IndexOf('e');
            s[i] = '0';
            trySolve(s.ToString());
            s[i] = '1';
            trySolve(s.ToString());
        }

        private void UpdateCode(Button[] cells, TextBox code) {
            StringBuilder newCode = new StringBuilder("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
            for (int i = 0; i < cells.Length; i++) {
                string cell = "" + cells[i].Content as string;
                newCode[i] = (cell == "") ? 'e' : cell[0];
            }
            code.Text = newCode.ToString();
        }

        private void EmptyGrid(Button[] grid) {
            foreach (var c in grid) c.Content = "";
        }

        private bool InsertCodeIntoGrid(string code, Button[] grid) {
            // Check if code is correct length
            if (code.Length != 36) return false;

            // Check if code has correct chars
            foreach (char c in code) if (!"01e".Contains(c)) return false;

            // All ok so fill
            for (int i = 0; i < code.Length; i++) {
                grid[i].Content = (code[i] == 'e') ? "" : "" + code[i];
                if (code[i] != 'e') grid[i].IsEnabled = false;
                else grid[i].IsEnabled = true;
            }
            return true;
        }

        private List<string> CheckCodeIsSolution(string code) {
            List<string> errors = new List<string>();

            // Length
            if (code.Length != 36) {
                errors.Add("Code has wrong length..." + code.Length);
                return errors;
            }

            // Filled
            if (code.Contains('e')) {
                errors.Add("Code has empty spots...");
                return errors;
            }

            // Duplicates
            List<string> hors = new List<string>();
            List<string> vers = new List<string>();
            for (int i = 0; i < 6; i++) {
                // Hors
                string subHor = code.Substring(i * 6, 6);
                if (hors.Contains(subHor)) errors.Add("Duplicate sub found " + subHor);
                hors.Add(subHor);

                // Vers
                string subVer =" " + code[i] + code[i + 6] + code[i + 12] + code[i + 18] + code[i + 24] + code[i + 30];
                if (vers.Contains(subVer)) errors.Add("Duplicate sub found " + subVer);
                vers.Add(subVer);
            }

            // Count
            foreach (var sub in hors.Concat(vers)) if ((sub.Length - sub.Replace("0", "").Length) != 3) errors.Add("Not equal 0's and 1's found in " + sub);

            // Chains
            foreach (var sub in hors.Concat(vers)) if (sub.Contains("000") || sub.Contains("111")) errors.Add("Chain found in " + sub);

            // Return errors
            return errors;
        }

        void updatePbSolve(int progress) {
            pbSolution_Sol.Value = progress;
        }
    }
}
