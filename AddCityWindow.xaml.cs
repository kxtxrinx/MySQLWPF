using MySql.Data.MySqlClient;
using System;
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
using System.Windows.Shapes;

namespace DI_UT1_Ejemplo24_MySQL_ContextMenu_Settings
{
    /// <summary>
    /// Lógica de interacción para AddCityWindow.xaml
    /// </summary>
    public partial class AddCityWindow : Window
    {
        MySqlConnection dbconn = null;

        public AddCityWindow(MySqlConnection dbconn)
        {
            InitializeComponent();
            this.dbconn = dbconn;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            GuardarCiudad();
            DialogResult = true;
            this.Close();
        }

        private void GuardarCiudad()
        {
            
            try
            {
                //Abrimos la connex
                dbconn.Open();
                //Preparamos la sentencia SQL
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = dbconn;

                //enumerar siempre las columnas
                string query = "INSERT INTO city (Name,CountryCode,District,Population) VALUES('" + txBName.Text + "', '" + txBCode.Text + "', '" + txBCountry.Text + "', '" + txBPopulation.Text + "')";
                
                cmd.CommandText = query;
                //Ejecutamos la consulta
                cmd.ExecuteNonQuery(); //Esto para queries que no devuelvan resultados, es decir para operaciones CUD

                //Cerramos la consulta
                dbconn.Close();
            }catch(MySqlException ex)
            {
                MessageBox.Show("Check your cnnection with the DB" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal void SetCodigoPais(string codigo)
        {
            txBCode.Text = codigo;
        }
    }
}
