using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DatabaseFiller.Properties;

namespace DatabaseFiller
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        private int id;
        public Dialog()
        {
            id = (int)Settings.Default["Id"];
            InitializeComponent();
            hey.Text = "Unesi inicijalni id. Trenutna vrijednost je = " + id;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var id = -1;
            try
            {
                id = int.Parse(IdText.Text);
                if(id < -1) throw new Exception();
            }
            catch
            {
                MessageBox.Show("Invalid value.");
                return;
            }

            Settings.Default["Id"] = id;
            Settings.Default.Save();
            Close();
        }
    }
}
