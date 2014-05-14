using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbolSource.Integration.NuGet.PackageExplorer
{
    public partial class Form1 : Form
    {
        private readonly ISymbolServerChecker checker;

        public Form1(ISymbolServerChecker checker)
        {
            this.checker = checker;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var pair in checker.Check(textBox1.Text))
                textBox2.Text += string.Format("{0}: {1}\r\n", pair.Key, pair.Value);
        }
    }
}
